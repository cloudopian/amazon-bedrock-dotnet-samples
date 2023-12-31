﻿using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;
using Amazon.Runtime.Credentials.Internal;
using Microsoft.SemanticKernel.Text;
using MyBedrockTest.Model;
using Newtonsoft.Json.Linq;
using Npgsql;
using Pgvector;
using Pgvector.Npgsql;
using System.Collections.Concurrent;
using ShellProgressBar;

using MyBedrockTest.KBSources;
using MyBedrockTest.KBSources.Dickens;
using MyBedrockTest.KBSources.Wikipedia;
using MyBedrockTest.DataStore;

namespace MyBedrockTest.Samples
{

    // example class where you can get embeddings from Amazon bedrock and save them in Postgres as pg_vector
    internal class Sample5 : ISample
    {
        ProgressBarOptions _progressBarOption=new ProgressBarOptions()
        {
            ProgressCharacter = '-',
            BackgroundColor = ConsoleColor.Yellow,
            ForegroundColor = ConsoleColor.Red,
            ForegroundColorDone=ConsoleColor.Green,
            CollapseWhenFinished=true
        };
        AWSCredentials _credentials;

        internal Sample5(AWSCredentials aWSCredentials)
        {
            _credentials = aWSCredentials;
        }
        public void Run()
        {
            Console.WriteLine($"Running {this.GetType().Name} ###############");
            IKBProvider dickensProvider = new CharlesDickensBookProvider();
            IKBProvider wikipediaProvider = new WikipediaProvider();

            List<KBArticle> selectedKBList = new List<KBArticle>();

            //To save time, we only analyze 3 Charles Dickens books
            selectedKBList.AddRange(dickensProvider.GetKBArticles().Take(3).ToList());
            //To save time, we only analuze all sample wiki articles
            selectedKBList.AddRange(wikipediaProvider.GetKBArticles());


            string connectionString = ConnectionStringProvider.GetDBConnectionString();

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.UseVector();

            var parallelOption = new ParallelOptions { MaxDegreeOfParallelism = 4 };


            using (var dataSource = dataSourceBuilder.Build())
            {
                using (var connection = dataSource.OpenConnection())
                {
                    using (var pbLevel1 = new ProgressBar(selectedKBList.Count, "Total progress",_progressBarOption))
                    {
                        Parallel.ForEach(selectedKBList, parallelOption, (kb, state, index) =>
                        {
                            using(var pbLevel2=pbLevel1.Spawn(2,$"Title:{kb.Title}", _progressBarOption))
                            {
                                List<ParagraphEmbeddingInfo> embeddings = GetEmbeddings(kb.Content, pbLevel2);
                                pbLevel2.Tick();
                                SaveEmbeddingToDB(embeddings, kb, connection, pbLevel2);
                                pbLevel2.Tick();
                            }
                            pbLevel1.Tick();
                        });
                        pbLevel1.WriteLine("Done");

                    }

             
                }
                Console.WriteLine($"End of {this.GetType().Name} ############");
            }
        }
        private List<ParagraphEmbeddingInfo> GetEmbeddings(string content, IProgressBar progressBar)
        {
            ConcurrentBag<ParagraphEmbeddingInfo> embeddingBag = new ConcurrentBag<ParagraphEmbeddingInfo>();

            List<string> lines = TextChunker.SplitPlainTextLines(content, 100);
            List<string> paragraphList = TextChunker.SplitPlainTextParagraphs(lines, 500);
            var parallelOption = new ParallelOptions { MaxDegreeOfParallelism = 10 };

            using (var pbChild = progressBar.Spawn(paragraphList.Count, $"Getting embeddings for {paragraphList.Count} paragraph(s)", _progressBarOption))
            {
                Parallel.ForEach(paragraphList, parallelOption, (paragraph, state, index) =>
                {
                    if(!string.IsNullOrEmpty(paragraph))
                    {
                        AmazonBedrockRuntimeClient client = new AmazonBedrockRuntimeClient(_credentials, Amazon.RegionEndpoint.USEast1);
                        InvokeModelRequest request = new InvokeModelRequest();
                        request.ModelId = "amazon.titan-embed-text-v1";
                        request.ContentType = "application/json";
                        request.Accept = "application/json";

                        string body = "{\"inputText\":" + Newtonsoft.Json.JsonConvert.ToString(paragraph) + "}";
                        request.Body = Utility.GetStreamFromString(body);

                        var result = client.InvokeModelAsync(request).Result;
                        string stringResult = Utility.GetStringFromStream(result.Body);

                        JObject jsonResult = JObject.Parse(stringResult);
                        if (jsonResult["embedding"] != null)
                        {
                            var array = jsonResult["embedding"].ToObject<float[]>();
                            ParagraphEmbeddingInfo embeddingInfo = new ParagraphEmbeddingInfo();
                            embeddingInfo.ParagraphId = (int)index;
                            embeddingInfo.Paragraph = paragraph;
                            embeddingInfo.Embedding = array;
                            embeddingBag.Add(embeddingInfo);
                        }
                    }
                   
                    pbChild.Tick();
                });
            }
            return embeddingBag.OrderBy(e => e.ParagraphId).ToList();
        }
        private void SaveEmbeddingToDB(List<ParagraphEmbeddingInfo> embeddingList, KBArticle kbArticle, NpgsqlConnection connection, IProgressBar progressBar)
        {
            string tabs = new string(' ', 256);
            lock (this)
            {
                using (var pbChild = progressBar.Spawn(embeddingList.Count, $"Saving {embeddingList.Count} row(s)", _progressBarOption))
                {
                    foreach (var item in embeddingList.Select((embeddingInfo, index) => (embeddingInfo, index)))
                    {
                        using (var cmd = new NpgsqlCommand("INSERT INTO kb_article (title,paragraph_id,paragraph,source,embedding) VALUES (:title,:paragraph_id,:paragraph,:source,:embedding)", connection))
                        {
                            cmd.Parameters.AddWithValue("title", kbArticle.Title);
                            cmd.Parameters.AddWithValue("paragraph_id", item.embeddingInfo.ParagraphId);
                            cmd.Parameters.AddWithValue("paragraph", item.embeddingInfo.Paragraph);
                            cmd.Parameters.AddWithValue("source", kbArticle.Source);
                            var embedding = new Vector(item.embeddingInfo.Embedding);
                            cmd.Parameters.AddWithValue("embedding", embedding);
                            cmd.ExecuteNonQuery();
                            pbChild.Tick();
                        }
                    }
                }
            }


        }
     


    }
}

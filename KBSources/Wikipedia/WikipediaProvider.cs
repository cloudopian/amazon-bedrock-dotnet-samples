using MyBedrockTest.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBedrockTest.KBSources.Wikipedia
{
    internal class WikipediaProvider : IKBProvider
    {
        public IEnumerable<KBArticle> GetKBArticles()
        {
            string filePath = @"C:\Dev\Cloudopian\amazon-bedrock-dotnet-samples\_SampleData\Wikipedia\fec7d46b-b2ba-4a87-b74f-ff6855ffb734.json";
            using(StreamReader reader = new StreamReader(filePath))
            {
                string content=reader.ReadToEnd();      
                var articleList = JsonConvert.DeserializeObject<List<WikiJasonElement>>(content);
                foreach(var article in articleList)
                {
                    KBArticle kbArticle = new KBArticle();
                    kbArticle.Title = article.Title;
                    kbArticle.Source = filePath;
                    kbArticle.Content = article.Text;
                    yield return kbArticle;
                }
              
            }   
            throw new NotImplementedException();
        }


    }
}

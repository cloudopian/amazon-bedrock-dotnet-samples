using Amazon.BedrockRuntime.Model;
using Amazon.BedrockRuntime;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;

namespace MyBedrockTest.Samples
{
    //an example class to generate embedding vectors
    internal class Sample4 : ISample
    {
        AWSCredentials _credentials;
        internal Sample4(AWSCredentials aWSCredentials)
        {
            _credentials = aWSCredentials;
        }
        public void Run()
        {
            Console.WriteLine($"Running {this.GetType().Name} ###############");
            AmazonBedrockRuntimeClient client = new AmazonBedrockRuntimeClient(_credentials, Amazon.RegionEndpoint.USEast1);
            InvokeModelRequest request = new InvokeModelRequest();
            request.ModelId = "amazon.titan-embed-text-v1";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            string body = "{\"inputText\":\"who are the last presidents of united state?\"}";
            request.Body = Utility.GetStreamFromString(body);

            var result = client.InvokeModelAsync(request).Result;
            string stringResult = Utility.GetStringFromStream(result.Body);

            JObject jsonResult = JObject.Parse(stringResult);
            if (jsonResult["embedding"] != null)
            {
                var array = jsonResult["embedding"].ToObject<double[]>();
                Console.Write(array);

            }
 
            Console.WriteLine($"End of {this.GetType().Name} ############");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBedrockTest
{
    internal class Utility
    {
        //Given a string convert it to a stream
        public static MemoryStream GetStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        //Converts a stream to a string
        public static string GetStringFromStream(Stream stream)
        {
            stream.Position = 0;
            var str = new StringBuilder();
            var reader = new StreamReader(stream);
            string result= reader.ReadToEnd();
            stream.Position = 0;
            return result;
            
        }

        //Save a base64 encoded image to a file
        public static void SaveBase64EncodedImage(string base64Image,string fileName)
        {
            string dir=System.IO.Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            byte[] image=Convert.FromBase64String(base64Image);
            using(FileStream fs=new FileStream(fileName, FileMode.Create))
            {
                fs.Write(image);
            }

        }
    }
}

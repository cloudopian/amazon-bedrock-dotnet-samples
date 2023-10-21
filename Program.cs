using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using MyBedrockTest.Samples;

namespace MyBedrockTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AWSCredentials creds = GetCredentials();
            ISample s1 = new Sample1(creds);
            ISample s2 = new Sample2(creds);
            ISample s3 = new Sample3(creds);
            s1.Run();
            s2.Run();
            s3.Run();
        }

       
        static AWSCredentials GetCredentials()
        {
            //Make sure you create a profile using AWS CLI and save access key & secrete key
            //watch https://www.youtube.com/watch?v=fwtmTMf53Ek for more information
            string profileName = "mydevprofile";
            var chain = new CredentialProfileStoreChain();
            AWSCredentials awsCredentials;
            if (!chain.TryGetAWSCredentials(profileName, out awsCredentials))
            {
                Console.WriteLine($"No profile name {profileName}  is found");
            }
        
            return awsCredentials;
        }
    }
}
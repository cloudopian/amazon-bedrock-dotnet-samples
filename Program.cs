using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using MyBedrockTest.DataStore;
using MyBedrockTest.Samples;

namespace MyBedrockTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connection=ConnectionStringProvider.GetDBConnectionString(); 
            AWSCredentials creds = GetCredentials();
            ISample s1 = new Sample1(creds);
            ISample s2 = new Sample2(creds);
            ISample s3 = new Sample3(creds);
            ISample s4 = new Sample4(creds);
            ISample s5 = new Sample5(creds);
            ISample s6 = new Sample6(creds);
            s1.Run();
            //s2.Run();
            //s3.Run();
            //s4.Run(); 
            //s5.Run(); 
            //s6.Run(); 
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
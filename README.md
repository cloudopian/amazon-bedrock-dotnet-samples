This code sample demonstrates how to use Amazon Bedrock with dotnet. 

## Setup permissions
First create an IAM user/role and attach the following policy.

```
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": "bedrock:*",
      "Resource": "*"
    }
  ]
}
```

This will give your IAM user/role full access to Bedrock related APIs. 

If you are using an IAM user, create an aws access key & secret key and save it under a profile called mydevprofile.  
`aws configure --profile mydevprofile`

If you are not familiar with IAM policy and permissions watch the following videos. 
https://www.youtube.com/watch?v=fwtmTMf53Ek


## Running the sample
Open the solution using Visual Studio Community or VS Code and run it. 

## Samples

- Sample1 shows how to use Anthropic Claud models. 
- Sample2 shows how to use Anthropic Claud models with streaming so that you get the results as quickly as possible. 
- Sample3 shows how to use stable diffusion to generate images and save them on file system.

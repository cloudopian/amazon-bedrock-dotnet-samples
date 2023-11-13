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

If you are not familiar with IAM policy and permissions watch [this](https://www.youtube.com/watch?v=fwtmTMf53Ek) video


## Running the sample
Open the solution using Visual Studio Community or VS Code and run it. 

## Samples

- **Sample1:** shows how to use Anthropic Claud models. 
- **Sample2:** shows how to use Anthropic Claud models with streaming so that you get the results as quickly as possible. 
- **Sample3:** shows how to use stable diffusion to generate images and save them on file system.
- **Sample4:** shows how to get embedding vector. You can use this embedding for tasks like similarity search & document clustering. 
- **Sample5:** shows how to read from a textual knowledge base, chunck long string values to smaller sizes to suit the max token limits of the model being used & get embeddings. It finally saves the embedding into PostgreSQL database.
  A couple of amazon cloudformation templates are provided if you want to spin up a VPC and a PostgreSQL RDS instance on AWS. 
  For that, first create a network with `Automation\dev-network.json` and then deploy Amazon Aurora PostgreSQL database on top of that network using `Automation\rds-postgres-db-public.json`. Alternatively you can use your own PostgreSQL instance with pg_vector support. 
- **Sample6:** shows how to put all the learnings into practice. We use **R**etrieval **A**ugmented **G**eneration (RAG) pattern to provide context into the question. We first search our knowledge base (in PostgreSQL) for matching paragraphs and then provide the context to the LLM so that it can provide an answer based on the context in which we ask the question. 

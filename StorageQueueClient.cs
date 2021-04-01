using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
namespace QueueApp
{
    class Program
    {
        private const string cn="DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=mastorageac;AccountKey=DLOqqlgXOu/85WnKx+mE5sjlAEsrDNK0TapFUYdHCs5cwZOYWHoF+NpbG2rLU8Ggah3W2VJVaQt2dwEbx0iYYQ==";
        
        static async Task Main(string[] args)
        {            
            if(args.Length>0)
            {
                //string val = string.Join("", args);
                //await SendArticleAsync(val);
                string val = await ReceiveArticleAsync();
                Console.WriteLine($"Received: {val}");
            }
        }

        static async Task SendArticleAsync(string message)
        {
         CloudStorageAccount storageAccount =  CloudStorageAccount.Parse(cn);

         CloudQueueClient queueClient =  storageAccount.CreateCloudQueueClient(); 
         CloudQueue queue =  queueClient.GetQueueReference("newsqueue");
         bool createQueue = await queue.CreateIfNotExistsAsync();

          if(createQueue)
          {
              Console.WriteLine("The queue of news article was created.");
          }

          CloudQueueMessage articleMessage = new CloudQueueMessage(message);
          await queue.AddMessageAsync(articleMessage);
        }

        static async Task<string> ReceiveArticleAsync()
        {
            CloudQueue queue = GetQueue();
            bool isPresent = await queue.ExistsAsync();

            if(isPresent){

                CloudQueueMessage retrievedArticle = await queue.GetMessageAsync();
                if(retrievedArticle!=null)
                {
                    string message = retrievedArticle.AsString;
                    await queue.DeleteMessageAsync(retrievedArticle);
                    return message;
                }
            }

            return "Queue is empty now";
        }

        static CloudQueue GetQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cn);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            return queueClient.GetQueueReference("newsqueue");           
        }


    }
}

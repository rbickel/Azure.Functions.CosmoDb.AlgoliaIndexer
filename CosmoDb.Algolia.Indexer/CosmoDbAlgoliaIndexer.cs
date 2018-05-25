using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Algolia.Search;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CosmoDb.Algolia.Indexer
{
    public static class CosmoDbAlgoliaIndexer
    {
        private static readonly string AlgoliaApiKey = Environment.GetEnvironmentVariable("Algolia:ApiKey");
        private static readonly string AlgoliaApplicationId = Environment.GetEnvironmentVariable("Algolia:ApplicationId");
        private static readonly string AlgoliaIndex = Environment.GetEnvironmentVariable("Algolia:Index");


        [FunctionName("DocumentUpdates")]
        public static async Task Run(
            [CosmosDBTrigger("database", "collection", ConnectionStringSetting = "CosmoDB")]
            IReadOnlyList<Document> documents)
        {

            var client = new AlgoliaClient(AlgoliaApplicationId, AlgoliaApiKey);
            var index = client.InitIndex(AlgoliaIndex);

            foreach (var document in documents)
            {
                document.SetPropertyValue("objectID", document.Id);
                await index.AddObjectAsync(document);
            }
            
        }
    }
}

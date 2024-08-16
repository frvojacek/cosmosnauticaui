using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace cosmonauticaui.Server.Services
{
    public class CosmosDBService
    {
        string _accountName = "cosmosnauticadb";
        CosmosClient _client;

        public CosmosDBService()
        {
			_client = GetClient(_accountName);
        }

		CosmosClient GetClient(string accountName)
		{
			CosmosClient client = new(
				$"https://{accountName}.documents.azure.com:443",
				new DefaultAzureCredential());

			return client;
		}

		public Container GetContainer(string databaseId, string containerId)
		{
			Container container = _client.GetContainer(databaseId, containerId);
			return container;
		}

		public async Task<ItemResponse<dynamic>> UploadDocument(
			Container container,
			dynamic document)
        {
            var item = await container.CreateItemAsync(document);
            return item;
        }
    }
}

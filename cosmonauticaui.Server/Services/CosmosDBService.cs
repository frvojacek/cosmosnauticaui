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

		public async Task<List<T>> QueryItems<T>(Container container, string query)
		{
			using FeedIterator<T> feed = container.GetItemQueryIterator<T>(
				queryText: query
			);

			var itemList = new List<T>();

			while (feed.HasMoreResults)
			{
				FeedResponse<T> response = await feed.ReadNextAsync();

				foreach (T item in response)
				{
					itemList.Add(item);
				}
			}

			return itemList;
		}

		public async Task<ItemResponse<T>> CreateItem<T>(Container container, T item)
        {
            return await container.CreateItemAsync(item);
        }

		public async Task<ItemResponse<T>> ReplaceItem<T>(Container container, T item, string id)
		{
			return await container.ReplaceItemAsync(item, id);
		}
    }
}

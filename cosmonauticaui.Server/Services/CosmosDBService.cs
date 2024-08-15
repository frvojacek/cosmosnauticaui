using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using cosmonauticaui.Server.Models;
using System.Text.Json;

namespace cosmonauticaui.Server.Services
{
    public class CosmosDBService
    {

        string _storageAccount = "cosmosnauticadb";
        CosmosClient _serviceClient;
        Container _containerClient;

        public CosmosDBService()
        {
            string uri = $"https://{_storageAccount}.documents.azure.com:443";
            _serviceClient = new(uri, new DefaultAzureCredential());
            _containerClient = _serviceClient.GetContainer("Documents", "Items");
        }

        public async Task<ItemResponse<Document>> UploadDocument(Document document)
        {
            var item = await _containerClient.CreateItemAsync((document));
            Console.WriteLine(JsonSerializer.Serialize(document));
            return item;
        }


    }
}

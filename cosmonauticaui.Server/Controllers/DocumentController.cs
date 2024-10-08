﻿using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using cosmonauticaui.Server.Models;
using cosmonauticaui.Server.Services;

namespace cosmonauticaui.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		AzureBlobService _blobService;
		BlobContainerClient _blobContainerClient;
		CosmosDBService _cosmosService;
		Container _cosmosContainer;

		public DocumentController(AzureBlobService blobService, CosmosDBService cosmosService)
		{
			_blobService = blobService;
			_blobContainerClient = _blobService.GetContainerClient("documents");

			_cosmosService = cosmosService;
			_cosmosContainer = _cosmosService.GetContainer("Documents", "Items");
		}

		[HttpGet]
		public async Task<IActionResult> Get(string? searchType, string? searchInput)
		{
			var query = "SELECT * FROM d";

			if (searchInput != null && searchType != null)
			{
				query += $" WHERE ARRAY_CONTAINS(d.{searchType}, '{searchInput}')";
			}

			var documents = await _cosmosService.QueryItems<Document>(_cosmosContainer, query);
			return Ok(documents);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(string id)
		{
			var query = $"SELECT * FROM d WHERE d.id = '{id}'";
			var cosmosDocuments = await _cosmosService.QueryItems<Document>(_cosmosContainer, query);
			var document = cosmosDocuments[0];
			var file = await _blobService.DownloadBlob(_blobContainerClient, document.Versions[0].fileId);
			var byteArray = file.Value.Content.ToArray();
			return File(byteArray, "application/octet-stream", document.Versions[0].fileName);
		}

		[HttpPost]
		public async Task<IActionResult> Post(IFormCollection form)
		{
			Document document = (FormCollection)form;

			var versionId = Guid.NewGuid().ToString();
			var fileId = Guid.NewGuid().ToString();
			var file = form.Files[0];
			var fileName = Path.GetFileName(file.FileName);
			var validFrom = form["validFrom"];
			var validTo = form["validTo"];

			await _blobService.UploadBlob(_blobContainerClient, file, fileId);

			var version = new DocumentVersion(
				versionId,
				fileId,
				fileName,
				validFrom,
				validTo
			);

			document.Versions.Add(version);

			await _cosmosService.CreateItem(_cosmosContainer, document);

			return Ok(document.id);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Put(string id, IFormCollection form)
		{
			Document document = (FormCollection)form;
			document.id = id;

			var query = $"SELECT * FROM d WHERE d.id = '{document.id}'";
			var cosmosDocuments = await _cosmosService.QueryItems<Document>(_cosmosContainer, query);
			document.Versions = cosmosDocuments[0].Versions;

			if (form.Files.Count > 0)
			{
				var versionId = Guid.NewGuid().ToString();
				var file = form.Files[0];
				var fileId = Guid.NewGuid().ToString();
				var fileName = Path.GetFileName(file.FileName);
				var validFrom = form["validFrom"];
				var validTo = form["validTo"];

				await _blobService.UploadBlob(_blobContainerClient, file, fileId, true);

				var version = new DocumentVersion(
					versionId,
					fileId,
					fileName,
					validFrom,
					validTo
				);

				document.Versions.Add(version);
			}

			await _cosmosService.ReplaceItem(_cosmosContainer, document, document.id);

			return Ok(document.id);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(string id)
		{
			var query = $"SELECT * FROM d WHERE d.id = '{id}'";
			var cosmosDocuments = await _cosmosService.QueryItems<Document>(_cosmosContainer, query);
			var document = cosmosDocuments[0];

            foreach (var version in document.Versions)
			{
				await _blobService.DeleteBlob(_blobContainerClient, version.fileId);
			}
			await _cosmosService.DeleteItem<Document>(_cosmosContainer, id);
			return Ok();
		}
	}
}

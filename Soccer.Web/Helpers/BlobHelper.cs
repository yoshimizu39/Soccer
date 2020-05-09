using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly CloudBlobClient _blobClient;

        public BlobHelper(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"]; //obtenemos el connectionstring
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<string> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            string name = $"{Guid.NewGuid()}"; //obtiene el nombre del blob
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName); //obtiene el container
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(stream); //subimos el blob

            return name;
        }

        public async Task<string> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream memoryStream = new MemoryStream(file); //obtiene el strema
            string name = $"{Guid.NewGuid()}"; //obtiene el nombre del blob
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName); //obtiene el container
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(memoryStream); //subimos el blob

            return name;
        }

        public async Task<string> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            string name = $"{Guid.NewGuid()}";
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            await blockBlob.UploadFromStreamAsync(stream);

            return name;
        }
    }
}

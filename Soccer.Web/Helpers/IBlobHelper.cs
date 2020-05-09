using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface IBlobHelper
    {
        // IFormFile  Lo vamos a utilizer desde la interface del usuario        
        Task<string> UploadBlobAsync(IFormFile file, string containerName);

        // byte[] arrays de bites para usar desde el api
        Task<string> UploadBlobAsync(byte[] file, string containerName);

        // image lo vamos a usar desde el Seader
        Task<string> UploadBlobAsync(string image, string containerName);

    }
}

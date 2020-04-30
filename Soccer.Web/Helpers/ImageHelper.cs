﻿using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public string UploadImage(byte[] pictureArray, string folder)
        {
            MemoryStream stream = new MemoryStream(pictureArray);
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";

            try
            {
                //guardamos los archivos en un folder
                stream.Position = 0;
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{folder}", file);
                File.WriteAllBytes(path, stream.ToArray());
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return $"~/images/{folder}/{file}"; //devuelve la ruta de datos
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";
            string path = Path.Combine(Directory.GetCurrentDirectory(),
                          $"wwwroot\\images\\{folder}", file);

            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/images/{folder}/{file}";
        }
    }
}

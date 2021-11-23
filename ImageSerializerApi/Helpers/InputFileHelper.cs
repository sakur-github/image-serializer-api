using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSerializerApi.Helpers
{
    public class InputFileHelper
    {
        public static async Task<byte[]> GetRetrivaFileAsync(IFormFile inputFile)
        {
            byte[] bytes = new byte[inputFile.Length];

            using (Stream fileStream = inputFile.OpenReadStream())
                await fileStream.ReadAsync(bytes, 0, (int)inputFile.Length);

            return bytes;
        }
    }
}

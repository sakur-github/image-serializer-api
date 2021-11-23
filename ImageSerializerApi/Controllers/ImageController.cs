using ImageSerializerApi.Helpers;
using ImageSerializerApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sakur.WebApiUtilities.Models;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSerializerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        [HttpPost("upload/file")]
        public async Task<ActionResult<object>> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null)
                    return new ApiResponse("Fil saknas", System.Net.HttpStatusCode.BadRequest);

                Image image = new Image(SixLabors.ImageSharp.Image.Load<Rgba32>(await InputFileHelper.GetRetrivaFileAsync(file)));

                StringBuilder stringBuilder = new StringBuilder();
                byte[] bytes = image.Bytes;

                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i]);
                    stringBuilder.Append(", ");
                }

                return new ApiResponse(new { bytes = stringBuilder.ToString() });
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [HttpPost("upload/string")]
        public async Task<ActionResult<object>> UploadString(UploadStringBody body)
        {
            try
            {
                if (!body.Valid)
                    return new ApiResponse("Invalid body of request, please provide: width, height, content", System.Net.HttpStatusCode.BadRequest);

                string imageData = body.Content.Trim();
                string[] numbers = imageData.Split(',');
                byte[] bytes = new byte[numbers.Length];

                for (int i = 0; i < numbers.Length; i++)
                {
                    string currentByteString = numbers[i].Trim();
                    if (!string.IsNullOrEmpty(currentByteString))
                        bytes[i] = byte.Parse(currentByteString);
                }

                Image image = new Image(bytes, body.Width, body.Height);

                using (MemoryStream ms = new MemoryStream())
                {
                    image.Bitmap.Save(ms, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                    Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
                    return File(ms.ToArray(), "application/octet-stream", "image.png");
                }
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }
    }
}

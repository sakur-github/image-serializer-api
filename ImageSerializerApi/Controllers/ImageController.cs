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
    [Route("")]
    public class ImageController : ControllerBase
    {
        [HttpPost("serialize")]
        public async Task<ActionResult<object>> UploadFile(IFormFile file, bool smoothBrightness)
        {
            try
            {
                if (file == null)
                    return new ApiResponse("File missing", System.Net.HttpStatusCode.BadRequest);

                SixLabors.ImageSharp.Image<Rgba32> imageFile = SixLabors.ImageSharp.Image.Load<Rgba32>(await InputFileHelper.GetRetrivaFileAsync(file));

                if(imageFile.Height % 8 != 0)
                    return new ApiResponse("Height needs to be a multiple of 8", System.Net.HttpStatusCode.BadRequest);

                Image image = new Image(imageFile, smoothBrightness);

                StringBuilder stringBuilder = new StringBuilder();
                byte[] bytes = image.Bytes;

                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i]);
                    stringBuilder.Append(", ");
                    if ((1 + i) % imageFile.Width == 0) stringBuilder.Append("\n");
                }

                return new ApiResponse(new { content = stringBuilder.ToString() });
            }
            catch (ApiException exception)
            {
                return new ApiResponse(exception);
            }
        }

        [HttpPost("deserialize")]
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

                if(bytes.Length < (body.Width / 8) * body.Height)
                    return new ApiResponse("Width and height does not match amount of bytes provided", System.Net.HttpStatusCode.BadRequest);

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

        [HttpGet("ping")]
        public async Task<ActionResult<object>> Ping()
        {
            return new ApiResponse("pong", System.Net.HttpStatusCode.OK);
        }
    }
}

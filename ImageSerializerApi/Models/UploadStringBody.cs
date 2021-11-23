using Sakur.WebApiUtilities.BaseClasses;

namespace ImageSerializerApi.Models
{
    public class UploadStringBody : RequestBody
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Content { get; set; }

        public override bool Valid { get { return !string.IsNullOrEmpty(Content); } }
    }
}

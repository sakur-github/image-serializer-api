using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSerializerApi.Models
{
    public class Image
    {
        public SixLabors.ImageSharp.Image Bitmap { get { if (_bitmap == null) GenerateBitmap(); return _bitmap; } }
        private SixLabors.ImageSharp.Image _bitmap;

        public byte[] Bytes { get { if (_bytes == null) GenerateByteArray(); return _bytes; } }
        private byte[] _bytes;

        public int Width { get; private set; }
        public int Height { get; private set; }

        private bool[][] pixelData;

        public Image(Image<Rgba32> bitmap)
        {
            InitPixelData(bitmap.Width, bitmap.Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (bitmap.Width > x && bitmap.Height > y)
                    {
                        pixelData[x][y] = bitmap[x, y].G > 128 ? false : true;
                    }
                }
            }
        }

        public Image(byte[] bytes, int width, int height)
        {
            InitPixelData(width, height);

            int length = Width * Height;
            int scanX = 0;
            int scanY = 0;
            int scannedPixels = 0;

            while (scannedPixels < length)
            {
                byte currentByte = bytes[scanX + (scanY * width) / 8];
                BitArray bitArray = new BitArray(new byte[] { currentByte });

                for (int i = 0; i < 8; i++)
                {
                    bool bit = bitArray[i];
                    pixelData[scanX][scanY + i] = bit;
                }

                scanX += 1;
                if (scanX >= Width)
                {
                    scanY += 8;
                    scanX = 0;
                }

                scannedPixels += 8;
            }
        }

        private void InitPixelData(int width = 128, int height = 32)
        {
            Width = width;
            Height = height;

            pixelData = new bool[Width][];
            for (int x = 0; x < Width; x++)
            {
                pixelData[x] = new bool[Height];
            }
        }

        private void GenerateBitmap()
        {
            Image<Rgba32> bitmap = new Image<Rgba32>(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bitmap[x, y] = ColorFromBool(pixelData[x][y]);
                }
            }

            _bitmap = bitmap;
        }

        private void GenerateByteArray()
        {
            List<byte> bytes = new List<byte>();

            int length = Width * Height;
            int scanX = 0;
            int scanY = 0;
            int scannedPixels = 0;

            while (scannedPixels < length)
            {
                bool[] bits = new bool[8];

                for (int i = 0; i < 8; i++)
                {
                    bits[i] = pixelData[scanX][scanY + i];
                }

                byte[] currentBytes = new byte[1];
                (new BitArray(bits)).CopyTo(currentBytes, 0);

                bytes.Add(currentBytes[0]);

                scanX += 1;
                if (scanX >= Width)
                {
                    scanY += 8;
                    scanX = 0;
                }

                scannedPixels += 8;
            }

            _bytes = bytes.ToArray();
        }

        private Color ColorFromBool(bool turnedOn)
        {
            return Color.FromRgb(!turnedOn ? (byte)255 : (byte)0, !turnedOn ? (byte)255 : (byte)0, !turnedOn ? (byte)255 : (byte)0);
        }
    }
}

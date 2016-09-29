using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace RocksmithToolkitLib.Extensions
{
    public static class ImageHandler
    {
        public static byte[] ImageToBytes(this Image image, ImageFormat format)
        {
            byte[] xret = null;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                xret = ms.ToArray();
            }
            return xret;
        }

        public static bool IsValidImage(this string fileName)
        {
            //Supported image types in DLC Package Converter
            var mimeByteHeaderList = new Dictionary<string, byte[]>();
            mimeByteHeaderList.Add(".dds", new byte[] { 68, 68, 83, 32, 124 });
            mimeByteHeaderList.Add(".gif", new byte[] { 71, 73, 70, 56 });
            mimeByteHeaderList.Add(".jpg", new byte[] { 255, 216, 255 });
            mimeByteHeaderList.Add(".jpeg", new byte[] { 255, 216, 255 });
            mimeByteHeaderList.Add(".bmp", new byte[] { 66, 77 });
            mimeByteHeaderList.Add(".png", new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 });

            var extension = Path.GetExtension(fileName);
            if (mimeByteHeaderList.ContainsKey(extension))
            {
                byte[] mime = mimeByteHeaderList[extension];
                byte[] file = File.ReadAllBytes(fileName);

                return file.Take(mime.Length).SequenceEqual(mime);
            }
            else
                return false;
        }

        public static Image PreSizeImage(string picPngPath, string imageSize)
        {
            if (picPngPath == null) return null;

            // resize image
            Image img = new Bitmap(Image.FromFile(picPngPath), Size2IntX(imageSize), Size2IntY(imageSize));
            return img;
        }

        public static void Save(this string inputFilePath, int maxWidth, int maxHeight, int quality, string saveFilePath)
        {
            using (var image = new Bitmap(inputFilePath))
            {
                // Get the image's original width and height
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                // To preserve the aspect ratio
                float ratioX = (float)maxWidth / (float)originalWidth;
                float ratioY = (float)maxHeight / (float)originalHeight;
                float ratio = Math.Min(ratioX, ratioY);

                // New width and height based on aspect ratio
                int newWidth = (int)(originalWidth * ratio);
                int newHeight = (int)(originalHeight * ratio);

                // Convert other formats (including CMYK) to RGB.
                using (Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb))
                {
                    // Draws the image in the specified size with quality mode set to HighQuality
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                    }

                    // Get an ImageCodecInfo object that represents the JPEG codec.
                    ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

                    // Create an Encoder object for the Quality parameter.
                    Encoder encoder = Encoder.Quality;

                    // Create an EncoderParameters object. 
                    EncoderParameters encoderParameters = new EncoderParameters(1);

                    // Save the image as a JPEG file with quality level.
                    EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                    encoderParameters.Param[0] = encoderParameter;
                    newImage.Save(saveFilePath, imageCodecInfo, encoderParameters);
                }
            }
        }

        public static Int32 Size2IntX(string imageSize)
        {
            string xSize = imageSize.Split(new[] { "x" }, StringSplitOptions.None)[0];
            return Int32.Parse(xSize);
        }

        public static Int32 Size2IntY(string imageSize)
        {
            string ySize = imageSize.Split(new[] { "x" }, StringSplitOptions.None)[1];
            return Int32.Parse(ySize);
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

    }
}

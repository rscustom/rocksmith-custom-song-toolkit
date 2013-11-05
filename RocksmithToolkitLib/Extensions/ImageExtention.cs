using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace RocksmithToolkitLib.Extensions
{
    public static class ImageHandler
    {
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
                using (Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb)) {
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

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
    }
}

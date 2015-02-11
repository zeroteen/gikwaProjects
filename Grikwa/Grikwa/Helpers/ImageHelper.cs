using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace Grikwa.Helpers
{
    public static class ImageHelper
    {
        public static ImageFormat GetImageFormatType(string description)
        {
            var extention = description.ToLower();
            if (extention == "jpeg" || extention == "jpg" || extention == "jpe" || extention == "jfif")
            {
                return ImageFormat.Jpeg;
            }
            else if (extention == "png")
            {
                return ImageFormat.Png;
            }
            else if (extention == "gif")
            {
                return ImageFormat.Gif;
            }

            return ImageFormat.Png;
        }
    }
}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace OMTB.Component.Util
{
    public class Thumbnail
    {
        public static byte[] CreateThumbnail(byte[] data1, ref double limitW, ref double limitH, ImageFormat format)
        {
            using (Image image = Image.FromStream(new MemoryStream(data1)) as Bitmap)
            {
                if (image == null)
                    return null;

                var size = new SizeF(image.Width, image.Height) { Width = (float)limitW, Height = (float)limitH };

                if (image.Width > image.Height)
                {
                    //宽度优先
                    if (image.Width >= limitW)
                    {
                        size.Width = (float)limitW;
                        size.Height = image.Height * size.Width / image.Width;
                    }
                    else
                    {
                        size.Width = image.Width;
                        size.Height = image.Height;
                    }
                }
                else
                {
                    //高度优先
                    if (image.Height >= limitH)
                    {
                        size.Width = image.Width * size.Height / image.Height;
                        size.Height = (float)limitH;
                    }
                    else
                    {
                        size.Width = image.Width;
                        size.Height = image.Height;
                    }
                }

                using (Image bitmap = new Bitmap(Convert.ToInt32(size.Width), Convert.ToInt32(size.Height)))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.InterpolationMode = InterpolationMode.High;
                        //设置高质量,低速度呈现平滑程度             
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.Clear(Color.Transparent);
                        var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        g.DrawImage(image, rect, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                        using (var ms = new MemoryStream())
                        {
                            limitW = size.Width;
                            limitH = size.Height;
                            bitmap.Save(ms, format);
                            return ms.ToArray();
                        }
                    }
                }
            }
        }
    }
}

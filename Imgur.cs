using System;
using System.Drawing;
using System.IO;
using System.Net;
using EasyImgur;

/*** Nuget Dependency
 *EasiestImgur 
 * System.Drawing.Common
 * 
 * **Useage (static call)
 * usingnamespace Imgur
 * Imgur.Upload(Base64String/byte[]Image/Image) => src / false
 * Imgur.UploadFile(ImageFileString) => src / false
 * Imgur.UploadSrc(ImageSrc) => src/ false
 * Imgur.Download(src) => Image
 * Imgur.Download(src, savepath)
 */
namespace Imgur
{
    class Imgur
    {
        private const string clientId = "5dbc58f57d7dd3a";
        public static string ImageToBase64(byte[] imagebytes)
        {
            string base64String = Convert.ToBase64String(imagebytes);
            return base64String;
        }
        public static string ImageToBase64(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                byte[] imageBytes = ms.ToArray();

                return ImageToBase64(imageBytes);
            }
        }
        public static string ImageToBase64(string file)
        {
            Image img = Image.FromFile(file);
            return ImageToBase64(img);
        }
        public static string srcToBase64(string src)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(src);
                return ImageToBase64(data);
            }
        }
        public static byte[] DownloadBytes(string imgsrc)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(imgsrc);
                return data;
            }
        }
        public static Image Download(string imgsrc)
        {
            using (var ms = new MemoryStream(DownloadBytes(imgsrc)))
            {
                return Image.FromStream(ms);
            }
        }
        public static void Download(string imgsrc,string savepath)
        {
            Download(imgsrc).Save(savepath);
        }
        public static string Upload(string base64)
        {
            return Uploader.Upload(clientId, base64);
        }
        public static string Upload(byte[] bytearray)
        {
            return Upload(ImageToBase64(bytearray));
        }
        public static string Upload(Image image)
        {
            return Upload(ImageToBase64(image));
        }
        public static string UploadFile(string file)
        {
            return  Upload(ImageToBase64(file));
        }
        public static string UploadSrc(string src)
        {
            return Upload(srcToBase64(src));
        }


    }
}

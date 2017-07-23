using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Aliyun.OpenServices;
using Aliyun.OpenServices.OpenStorageService;

namespace OMTB.Component.Util
{
    using System.IO;

    public enum FileUsage
    {
        Article = 0,
        Consult,
        PrintQr,
        EmployeeHeadImg,
        StudentHeadImg,
        LoginImg
    }


    public class OssHelper
    {
        private readonly string bktName;
        private readonly string myAccId;
        private readonly string mySecId;
        private readonly string ossBase;
        private readonly string ossHost;
        private readonly string ossFolder;

        public OssHelper()
        {
            bktName = ConfigHelper.GetConfigString("bucketName");
            myAccId = ConfigHelper.GetConfigString("accessId");
            mySecId = ConfigHelper.GetConfigString("secId");
            ossBase = ConfigHelper.GetConfigString("ossBaseUrl");
            ossHost = ConfigHelper.GetConfigString("ossHost");
            ossFolder = ConfigHelper.GetConfigString("ossFolder");
        }

        public string UploadFile(byte[] bytes, string name, FileUsage fileUsage)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return UploadFile(stream, name, fileUsage);
        }

        public string UploadFile(Stream inputStream, string name, FileUsage fileUsage)
        {
            if (string.IsNullOrEmpty(this.bktName))
            {
                throw new Exception("bktName is null");
            }

            //文件名示例:20130729143455-ABCDE.jpg
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss-") + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 5) + Path.GetExtension(name);
            Uri uri = new Uri(this.ossBase);
            var oc = new OssClient(uri, this.myAccId, this.mySecId);
            string key = ossFolder + fileUsage.ToString() + "/" + fileName;
            PutObjectResult result = oc.PutObject(this.bktName, key, inputStream, new ObjectMetadata());
            AccessControlList accs = oc.GetBucketAcl(this.bktName);

            //上传成功替换路径为oss路径
            string fileUrl = string.Format("http://{0}/{1}", ossHost, key);
            return fileUrl;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileUsage"></param>
        /// <returns></returns>
        public string UploadFile(HttpPostedFileBase file, FileUsage fileUsage)
        {
            if (string.IsNullOrEmpty(this.bktName))
            {
                throw new Exception("bktName is null");
            }

            //文件名示例:20130729143455-ABCDE.jpg
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss-") + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 5) + Path.GetExtension(file.FileName);

            Stream stream = file.InputStream;

            var oc = new OssClient(this.ossBase, this.myAccId, this.mySecId);
            string key = ossFolder + fileUsage.ToString() + "/" + fileName;
            var metadata = new ObjectMetadata();
            //metadata.CacheControl = "No-Cache";
            metadata.ContentLength = stream.Length;
            PutObjectResult result = oc.PutObject(this.bktName, key, stream, metadata);
            AccessControlList accs = oc.GetBucketAcl(this.bktName);
            //上传成功替换路径为oss路径
            string fileUrl = string.Format("http://{0}/{1}", ossHost, key);
            stream.Dispose();
            return fileUrl;
        }

        public string UploadImage(HttpPostedFileBase file, FileUsage fileUsage, double limitW = 1000, double limitH = 1000)
        {
            double w = limitW;
            double h = limitH;
            return UploadImage(file, fileUsage, ref w, ref h);
        }

        public string UploadImage(HttpPostedFileBase file, FileUsage fileUsage, ref double limitW, ref double limitH)
        {
            if (string.IsNullOrEmpty(this.bktName))
            {
                throw new Exception("bktName is null");
            }

            //文件名示例:20130729143455-ABCDE.jpg
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss-") + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 5) + Path.GetExtension(file.FileName);

            System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
            byte[] bytes = new byte[file.InputStream.Length];
            file.InputStream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            file.InputStream.Seek(0, SeekOrigin.Begin);
            var img = System.Drawing.Imaging.ImageFormat.Jpeg;
            if (Path.GetExtension(file.FileName).ToLower().Contains("png"))
            {
                img = System.Drawing.Imaging.ImageFormat.Png;
            }
            var data = Thumbnail.CreateThumbnail(bytes, ref limitW, ref limitH, img);
            Stream stream = new MemoryStream(data);

            var oc = new OssClient(this.ossBase, this.myAccId, this.mySecId);
            string key = ossFolder + fileUsage.ToString() + "/" + fileName;
            var metadata = new ObjectMetadata();
            //metadata.CacheControl = "No-Cache";
            metadata.ContentLength = stream.Length;
            PutObjectResult result = oc.PutObject(this.bktName, key, stream, metadata);
            AccessControlList accs = oc.GetBucketAcl(this.bktName);
            //上传成功替换路径为oss路径
            string fileUrl = string.Format("http://{0}/{1}", ossHost, key);
            stream.Dispose();
            return fileUrl;
        }
    }
}

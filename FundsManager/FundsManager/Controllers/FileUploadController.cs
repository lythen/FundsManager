using Lythen.Common;
using System;
using io = System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Lythen.Controllers
{
    public class FileUploadController : Controller
    {
        private string[] extension = MyConfiguration.GetCanUploadExtendsion();//允许上传的附件格式
        private string attachmentTempPath = MyConfiguration.GetAttachmentTempPath();//上传附件的暂存地址
        [HttpPost]
        public JsonResult FileUpload()
        {
            ViewModels.BaseJsonData json = new ViewModels.BaseJsonData();
            var file = Request.Files["data"];
            if (file == null)
            {
                json.state = 0;
                json.msg_text = "没有文件，请重新上传。";
            }
            if (!extension.Contains(io.Path.GetExtension(file.FileName).ToLower()))
            {
                json.state = 0;
                json.msg_text = "上传文件不在允许的格式范围内。";
            }
            string dtStr = DateTime.Now.ToString("yyyyMMdd");
            string TempDir = string.Format("{0}{1}\\", attachmentTempPath, dtStr);
            if (!io.Directory.Exists(TempDir)) io.Directory.CreateDirectory(TempDir);
            string file_name = string.Format("{0}{1}", TempDir, file.FileName);
            string temp_file = SetSameName(file_name);
            file.SaveAs(temp_file);
            string fileName = io.Path.GetFileName(temp_file);
            json.state = 1;
            json.data = string.Format("{0}/{1},{2}\\{3},{4}", dtStr, fileName, dtStr, fileName, fileName);
            return Json(json);
        }
        Random random;
        string SetSameName(string file_Path)
        {
            if (random == null) random = new Random();
            if (io.File.Exists(file_Path)){
                string filename = io.Path.GetFileNameWithoutExtension(file_Path);
                string newName = string.Format("{0}_{1}", filename, random.Next(1000, 9999));
                return file_Path.Replace(filename, newName);
            } else return file_Path;
        }
    }
}
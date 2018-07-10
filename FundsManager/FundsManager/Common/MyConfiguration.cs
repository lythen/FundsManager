using System;
using System.Configuration;

namespace FundsManager.Common
{
    public static class MyConfiguration
    {
        private static string[] _extendsion = null;
        private static string _controllerPath = null;
        private static string _attachmentPath = null;
        private static string _attachmentTempPath = null;
        private static string _logPath = null;
        private static string _photoPath = null;
        private static string _tempPhotoPath = null;
        public static string[] GetCanUploadExtendsion()
        {
            if (_extendsion == null)
            {
               try
                {
                    _extendsion = ConfigurationManager.AppSettings["extendsion"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }catch(Exception ex)
                {
                    ErrorUnit.WriteErrorLog(ex.ToString(), "GetCanUploadExtendsion");
                }
            }
            return _extendsion;
        }
        public static string GetAttachmentPath()
        {
            if (_attachmentPath == null)
            {
                try
                {
                    _attachmentPath = ConfigurationManager.AppSettings["attachmentPath"];
                }
                catch (Exception ex)
                {
                    ErrorUnit.WriteErrorLog(ex.ToString(), "GetAttachmentPath");
                }
            }
            return _attachmentPath;
        }
        public static string GetAttachmentTempPath()
        {
            if (_attachmentTempPath == null)
            {
                try
                {
                    _attachmentTempPath = ConfigurationManager.AppSettings["attachmentTempPath"];
                }
                catch (Exception ex)
                {
                    ErrorUnit.WriteErrorLog(ex.ToString(), "GetAttachmentTempPath");
                }
            }
            return _attachmentTempPath;
        }
    }
}
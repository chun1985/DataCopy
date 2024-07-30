
using System;
using System.Configuration;
using System.IO;

namespace DataCopy.Class
{
    public class Task
    {
        /// <summary>
        /// Task建構子
        /// private限制建立新物件
        /// </summary>
        private Task(){
        
        }
        
        /// <summary>
        /// Task統一物件
        /// </summary>
        private static Task commonObj = null;

        /// <summary>
        /// 提供Task物件
        /// </summary>
        /// <returns></returns>
        public static Task useCommonObj() {
            if (commonObj == null) {
                commonObj = new Task();
            }
           return commonObj;
        }

        /// <summary>
        /// 暫存訊息
        /// </summary>
        private string _message;

        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string Message
        {
            get { return _message; }
        }

        /// <summary>
        /// 是否包含跟目錄
        /// </summary>
        private bool isIncludeRootFolder = ConfigurationManager.AppSettings["isIncludeRootFolder"] == "true";

        /// <summary>
        /// 執行程式
        /// </summary>
        public void run() {
            string sourcePath = ConfigurationManager.AppSettings["sourcePath"];
            string targetPath = ConfigurationManager.AppSettings["targetPath"];

            int y = commonObj.Copy(sourcePath, targetPath, isIncludeRootFolder);
            if (y == -1)
            {
                Console.WriteLine(@"更新檔案到目的資料夾失敗，可能檔案被開啟或是無權限修改:" + commonObj.Message);
            }
        }

        /// <summary>
        /// 複製資料夾
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_target"></param>
        /// <param name="recursive"></param>
        /// <returns>成功或失敗代碼</returns>
        public int Copy(string _source, string _target, bool recursive)
        {
            try
            {
                if (!Directory.Exists(_source))
                {
                    _message = "來源資料夾不存在!";
                    return -1;
                }
                if (!Directory.Exists(_target))
                {
                    // 目的資料夾不存在就建立
                    Directory.CreateDirectory(_target);
                }

                // 先取得來源目露的所有檔案
                string[] files = Directory.GetFiles(_source);
                foreach (var fi in files)
                {
                    string fn = Path.GetFileName(fi);
                    // 複製檔案併覆蓋目的檔案
                    File.Copy(fi, Path.Combine(_target, fn), true);
                }
            }
            catch (Exception e)
            {
                _message = e.Message;
                return -1;
            }

            //繼續處理子目錄
            if (recursive)
            {
                // 取得子目錄
                string[] dirs = Directory.GetDirectories(_source);
                foreach (var di in dirs)
                {
                    string dn = Path.GetFileName(di);
                    // 遞迴呼叫
                    if (commonObj.Copy(di, Path.Combine(_target, dn), isIncludeRootFolder) == -1)
                    {
                        _message = commonObj.Message;
                        return -1;
                    }
                }
            }

            return 1;
        }
    }
}

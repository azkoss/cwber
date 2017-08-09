using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace WinFormDemo
{
    class FtpUtil
    {
        /// <summary>
        /// 从ftp服务器上下载文件的功能 
        /// </summary>
        /// <param name="filePath">ftp文件路径</param>
        public void Download( string filePath)
        {
            try
            {
                string downLoadName = filePath.Split('/')[filePath.Split('/').Length -1];
                string startPath = Application.StartupPath;
                FileStream outputStream = new FileStream(startPath + "\\pdfFiles\\" + downLoadName, FileMode.Create);
                FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(filePath));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Properties.Settings.Default.ftpUserName, Properties.Settings.Default.ftpPassword);
                reqFTP.UsePassive = false;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                if (ftpStream != null)
                {
                    int readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                }
                if (ftpStream != null) ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }  

    }
}

//2016.11.08, czs, create in hongqing, ���� FTP ��ɾ�Ĳ�

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Gdp.Utils
{
    /// <summary>
    /// ���繤��
    /// </summary>
    public static class NetUtil
    {
        static Gdp.IO.ILog log = Gdp.IO.Log.GetLog(typeof(NetUtil));
        
        #region  FTP ����
        /// <summary>
        ///  ftp���ϴ�����
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="ftpPath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        static public void FtpUpload(string localFilePath, string ftpPath, string userName = "Anonymous", string password = "User@")  
       {
           log.Info("�����ϴ� " + localFilePath + " �� " + ftpPath);

           ParseFtpPath(ref ftpPath, ref userName, ref password);

           FileInfo fileInf = new FileInfo(localFilePath);  
           FtpWebRequest reqFTP;  
           // ����uri����FtpWebRequest����   
           reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath + fileInf.Name));  
           // ftp�û���������  
           reqFTP.Credentials = new NetworkCredential(userName, password);  
  
           reqFTP.UsePassive = false;  
           // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�  
           // ��һ������֮��ִ��  
           reqFTP.KeepAlive = false;  
           // ָ��ִ��ʲô����  
           reqFTP.Method = WebRequestMethods.Ftp.UploadFile;  
           // ָ�����ݴ�������  
           reqFTP.UseBinary = true;  
           // �ϴ��ļ�ʱ֪ͨ�������ļ��Ĵ�С  
           reqFTP.ContentLength = fileInf.Length;  
           // �����С����Ϊ2kb  
           int buffLength = 2048;  
           byte[] buff = new byte[buffLength];  
           int contentLen;  
           // ��һ���ļ��� (System.IO.FileStream) ȥ���ϴ����ļ�  
           FileStream fs = fileInf.OpenRead();  
           try  
           {  
               // ���ϴ����ļ�д����  
               Stream strm = reqFTP.GetRequestStream();  
               // ÿ�ζ��ļ�����2kb  
               contentLen = fs.Read(buff, 0, buffLength);  
               // ������û�н���  
               while (contentLen != 0)  
               {  
                   // �����ݴ�file stream д�� upload stream  
                   strm.Write(buff, 0, contentLen);  
                   contentLen = fs.Read(buff, 0, buffLength);  
               }  
               // �ر�������  
               strm.Close();  
               fs.Close();  
           }  
           catch (Exception ex)  
           {
               log.Error("FTP �ϴ�����" + ex.Message + "\r\n" + localFilePath);
           }  
       }  
   
        /// <summary>
        /// ɾ��ָ���ļ�
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        static public void Delete(string ftpPath, string userName = "Anonymous", string password = "User@")  
        {
           ParseFtpPath(ref ftpPath, ref userName, ref password);

           FtpWebRequest reqFTP;  
           try  
           {   
               reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));  
               reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;  
               reqFTP.UseBinary = true;  
               reqFTP.Credentials = new NetworkCredential(userName, password);  
               reqFTP.UsePassive = false;  
               FtpWebResponse listResponse = (FtpWebResponse)reqFTP.GetResponse();  
               string sStatus = listResponse.StatusDescription;  
           }  
           catch (Exception ex)  
           {  
               throw ex;  
           }  
       }  

        /// <summary>
        /// ��ȡFTP�ļ���ַ��
        /// </summary>
        /// <param name="ftpFolderOrFilePath"></param>
        /// <param name="extension"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static List<string> GetFtpFileUrls(string ftpFolderOrFilePath, string extension, string userName, string password)
        {
            List<string> fileUrlPathes = new List<string>();
            //Uri uri = new Uri(ftpFolderOrFilePath);
            if (PathUtil.IsFile(ftpFolderOrFilePath))
            {
                fileUrlPathes.Add(ftpFolderOrFilePath);
            }
            else
            {
                fileUrlPathes.AddRange(NetUtil.GetFtpFileList(ftpFolderOrFilePath, extension, userName, password));
            }
            return fileUrlPathes;
        }
        /// <summary>
        /// ��ȡָ��Ŀ���µ��ļ�·����
        /// </summary>
        /// <param name="ftpFolderPath">Ŀ¼·������IP��ַ�Ͷ˿ڣ�����"/"��β</param>
        /// <param name="extension">�������Էֺŷָ����ƥ������</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        static public List<string> GetFtpFileList(string ftpFolderPath, string extension = "*.*", string userName = "anonymous", string password = "")
        {
            ParseFtpPath(ref ftpFolderPath, ref userName, ref password);

            Uri uri = new Uri(ftpFolderPath);
             
            FtpClient client = new FtpClient(uri, userName, password);
            client.RemotePath =uri.AbsolutePath;//.GetLeftPart(UriPartial.Path); 
            var results = client.GetFileList(extension);

            List<string> pathes = new List<string>();
            foreach (var item in results)
            {
                pathes.Add(Path.Combine(ftpFolderPath, item));
            }

            return pathes;
        }

        /// <summary>
        /// ���أ����ر���·�������������ܻ��Ĵ����������û��ָ���û��������룬���Զ����� Anonymous �û�����
        /// </summary>
        /// <param name="ftpFolderOrFilePath">·������IP��ַ�Ͷ˿ڣ�����Ŀ¼������"/"��β</param>
        /// <param name="extension">����Ŀ¼�������ã������Էֺŷָ����ƥ������</param>
        /// <param name="localFolder"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static List<string> DownloadFtpDirecotryOrFile(string ftpFolderOrFilePath, string extension = "*.*", string localFolder = @"C:\GnsserTemp\", string userName = "anonymous", string password = "User@", bool IsOverwrite = false, bool throwException = false)
        {
            ParseFtpPath(ref ftpFolderOrFilePath, ref userName, ref password);

            List<string> fileUrlPathes = new List<string>();
            //Uri uri = new Uri(ftpFolderOrFilePath);
            if (PathUtil.IsFile(ftpFolderOrFilePath))
            {
                fileUrlPathes.Add(ftpFolderOrFilePath);
            }
            else
            {
                fileUrlPathes.AddRange(GetFtpFileList(ftpFolderOrFilePath, extension, userName, password));
            }


            List<string> localFilePathes = new List<string>();
            foreach (var url in fileUrlPathes)
            {
                var localPath = Path.Combine(localFolder, Path.GetFileName(url));
                if (FtpDownload(url, localPath, IsOverwrite, userName, password, throwException))
                {
                    localFilePathes.Add(localPath);

                }
            }

            return localFilePathes;
        }
        /// <summary>
        /// ����FTP·������ȡ�û���������
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        private static void ParseFtpPath(ref string ftpPath, ref string userName, ref string password)
        {
            //�����Դ����û��������룬��δָ��
            if (ftpPath.Contains("@"))
            {
                var startIndex = ftpPath.IndexOf("//");
                var endIndex = ftpPath.IndexOf("@");
                var strs = ftpPath.Substring(startIndex + 1, endIndex - startIndex - 1);
                var strarray = strs.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                userName = strarray[0];
                password = strarray[1];

                ftpPath = "ftp://" + ftpPath.Substring(endIndex + 1);
            }
        }

        /// <summary>
        /// FTP ����,���ļ��Ե��ļ������ء�
        /// </summary>
        /// <param name="urlpath">Url ��ַ</param>
        /// <param name="savePath">�����ַ</param>
        /// <param name="userName">ftp�û���</param>
        /// <param name="password">ftp����</param>
        public static bool FtpDownload(string urlpath, string savePath,  bool IsOverwrite = false, string userName = "anonymous", string password = "User@", bool throwException = false)
        {
            log.Info("���ڳ������� " + urlpath + " �� " + savePath);

            ParseFtpPath(ref urlpath, ref userName, ref password);

            if (String.IsNullOrEmpty(userName)) { userName = "anonymous"; }
            if (String.IsNullOrEmpty(password)) { password = "User@"; }

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(urlpath);
            if (userName != "Anonymous") { request.Credentials = new NetworkCredential(userName, password); }
            //���������������Լ���
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            //request.ProtocolVersion = HttpVersion.Version10;
            //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.UseBinary = true;
          //  request.UsePassive = false;
            request.UsePassive = true;
            request.Timeout = 20 * 1000;
            try
            {
                //���Ŀ¼�Ĵ����ԣ������� 
                Gdp.Utils.FileUtil.CheckOrCreateDirectory(Path.GetDirectoryName(savePath));  

                //����ļ��Ƿ���ڣ���������ǣ��ļ����ڣ��Ҵ�СΪ10byte���ϣ������ء�
                if (!IsOverwrite && File.Exists(savePath))
                {
                    var fileInfo = new FileInfo(savePath);
                    if (fileInfo.Length > 10L)
                    {
                        log.Info("�Ѿ����� " + savePath + "��������Ϊ�����ǣ��������� ");
                        return true;
                    }
                }
                

                FtpWebResponse respose = (FtpWebResponse)request.GetResponse();
                
            using (Stream ftpStream = respose.GetResponseStream())
                {
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                    {
                        int size = 1024;
                        byte[] buffer = new byte[size];
                        int count = 0;
                        while ((count = ftpStream.Read(buffer, 0, size)) > 0)
                        {
                            fileStream.Write(buffer, 0, count);
                            fileStream.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("����ʧ��! " + urlpath + " " + ex.Message);

                if (throwException) { throw ex; }
               
                return false;
            }
            log.Info("�ɹ����� " + urlpath + " �� " + savePath);
            return true;
        }
        #endregion

        #region �������緽��
        /// <summary>
        /// ����WebClientֱ������
        /// </summary>
        /// <param name="urlpath"></param>
        /// <param name="savePath"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static bool Download(string urlpath, string savePath, bool throwException = false)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(urlpath, savePath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

                return false;
            }
            return true;
        }
        public static bool DownloadAsync(string urlpath, string savePath, bool throwException = false)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFileAsync( new Uri( urlpath), savePath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

                return false;
            }
            return true;
        }
        /// <summary>
        /// ���������ַ���
        /// </summary>
        /// <param name="urlpath"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static string GetNetString(string urlpath, bool throwException = false)
        {
            try
            {
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(urlpath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

            }
            return "";
        }
        /// <summary>
        /// ���������ַ���,������ΪDouble
        /// </summary>
        /// <param name="urlpath"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static double GetNetDouble(string urlpath, bool throwException = false)
        {
            try
            { 
                var val = GetNetString(urlpath, throwException);
                var va= val.Replace("\"", "");
                return double.Parse(va);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                if (throwException) { throw ex; }

            }
            return 0;
        }

        /// <summary>
        /// Http ����
        /// </summary>
        /// <param name="urlpath">Url ��ַ</param>
        /// <param name="savePath">�����ַ</param>
        /// <param name="throwException">�Ƿ��׳��쳣</param> 
        public static bool HttpDownload(string urlpath, string savePath, bool throwException = false)
        {
            HttpWebRequest request = (HttpWebRequest)FtpWebRequest.Create(urlpath); 
            try
            {
                //���Ŀ¼�Ĵ����ԣ�������
                var saveDir = Path.GetDirectoryName(savePath); 
                Gdp.Utils.FileUtil.CheckOrCreateDirectory(saveDir);

                HttpWebResponse respose = (HttpWebResponse)request.GetResponse();
            using (Stream ftpStream = respose.GetResponseStream())
                {
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                    {
                        int size = 512;
                        byte[] buffer = new byte[size];
                        int count = 0;
                        while ((count = ftpStream.Read(buffer, 0, size)) > 0)
                        {
                            fileStream.Write(buffer, 0, count);
                            fileStream.Flush();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (throwException) { throw ex; }

                return false;
            }
            return true;
        }

        /// <summary>
        /// get the Ip of currentItem computor.
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            string ipStr = "";
            string hostName = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] addessList = System.Net.Dns.GetHostEntry(hostName).AddressList;
            for (int i = 0; i < addessList.Length; i++)
            {
                ipStr += addessList[i].ToString() + ", ";
            }
            return ipStr;
        }
        public static string GetFirstIp()
        { 
            string hostName = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] addessList = System.Net.Dns.GetHostEntry(hostName).AddressList;
            return addessList[0].ToString();
        }
        public static bool IsIp(string ip)
        {
            Match m = Regex.Match(ip, "(\\d{1,3}\\.){3}\\d{1,3}");
            return m.Success;
        }

        public static IPStatus PingResult(string hostNmeOrAddress)
        {
            try
            {
                IPAddress ip = Dns.GetHostAddresses(hostNmeOrAddress)[0];
                System.Net.NetworkInformation.Ping ping = new Ping();
                PingOptions pingOptions = new PingOptions();
                pingOptions.DontFragment = true;

                string data = "aa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                //Watie fraction for a reply.
                int timeout = 1000;
                PingReply reply = ping.Send(ip, timeout, buffer, pingOptions);
                return reply.Status;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return IPStatus.NoResources;
            }
        }


        #region using dll
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>
        /// �����Ƿ����
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected()
        {
            int i = 0; 
            bool state = InternetGetConnectedState(out i, 0);
            return state;
        }
        /// <summary>
        /// �Ƿ������˹��ʻ�������ͨ���ٶ���վ���ԡ�
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedToInternet(){
          bool isConnected =Gdp.Utils.NetUtil.IsConnected();
          if (isConnected)
          {
              IPStatus status = NetUtil.PingResult("www.baidu.com");
              if (status != IPStatus.Success)
              {
                  return false;
              }
              return true;
          }
          return false;
        }

        #endregion
        #endregion

    }
}

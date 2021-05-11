using System;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;
using System.Windows;
using System.Diagnostics;

namespace ADM
{
    class Utils
    {
        public static string username = "";
        public static string password = "";
        public static string router = "";
        public static string port = "";
        public static string router_url = "";
        public static string asus_cooike = ""; // cookies AuthByPasswd
        public static string path = ""; // cookies path
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36";
        public static string direct_download = ConfigurationManager.AppSettings["direct_download"];
        public static string http_post_url;
        public static string http_post_data;
        public static DateTime d1; // 创建时间对象
        public static TimeSpan timeSpan; //创建时间差
        public static Boolean isInit = false;


        public static void init()
        {
            
            // 获取路由器地址和端口
            router = ConfigurationManager.AppSettings["router"];
            port = ConfigurationManager.AppSettings["port"];
            

            // 获取账号密码
            username = ConfigurationManager.AppSettings["username"];
            password = ConfigurationManager.AppSettings["password"];



            // 防止router,port,username,password字段为空
            if (router != "" && port != "" && username != ""&& password!="")
            {
                router_url =  "http://"+router + ":" + port;
                // 账号密码转换为BASE64→URL加密
                username = Base64_Convert(username);
                password = Base64_Convert(password);

                // 获取登录cookies
                http_post_url = router_url + "/check.asp";
                http_post_data = "flag=&login_username=" + username + "&login_passwd=" + password + "&directurl=%2Fdownloadmaster%2Findex.asp";
                HttpPost(http_post_url, http_post_data);
                // 记录获取到cookies的系统时间,用于更新cookies 防止超时
                d1 = DateTime.Now;
            }
            else
            {
                MessageBox.Show("请输入网址,端口,用户名,密码","提示");
                new setting().ShowDialog();
                Environment.Exit(0);
            }
            isInit = true;

        }

        public static  string HttpGet(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                //request.Timeout = 5000;
                request.KeepAlive = true;
                request.UserAgent = DefaultUserAgent;
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Cookie("AuthByPasswd", asus_cooike, @path, Utils.router));
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                if (res == null)
                {
                    return "";
                }
                else if (res.StatusCode == HttpStatusCode.OK) //200
                { return ""; }
                else if (res.StatusCode == HttpStatusCode.BadRequest)//400
                { return ""; }
                else if (res.StatusCode == HttpStatusCode.NotFound)//404
                { return ""; }
                else if (res.StatusCode == HttpStatusCode.InternalServerError)//500
                {
                    return "";
                }
                else if (res.StatusCode == HttpStatusCode.Unauthorized)//401
                {
                    return "";
                }
                else
                { return ""; }
            }
            
        }

        /*
         *  获取初始cookies
         * 
         */
        public static void HttpPost(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                CookieContainer myCookieContainer = new CookieContainer();
                request.Method = "POST";
              //  request.Timeout = 5000;
                request.KeepAlive = true;
                request.UserAgent = DefaultUserAgent;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postDataStr.Length;
                request.CookieContainer = myCookieContainer;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(postDataStr);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                writer.Close();
                reader.Close();
                //          CookieCollection cookies = response.Cookies;
                WebHeaderCollection headers = response.Headers;
                string[] set_cookies_list = headers.Get(0).Split(';');
                for (int i = 0; i < set_cookies_list.Length - 1; i++)
                {
                    if (set_cookies_list[i].Contains("AuthByPasswd="))
                    {
                        asus_cooike = set_cookies_list[i].Replace("AuthByPasswd=", "").Trim();
                    }
                    else if (set_cookies_list[i].Contains("path="))
                    {
                        path = set_cookies_list[i].Replace("path=", "").Trim();
                    }
                }

                //     String a = response.Cookies["AuthByPasswd"].Value;
            }
            catch(WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                if (res == null)
                {
                    MessageBox.Show("网址或端口错误,退出程序", "警告");
                    new setting().ShowDialog();
                    Environment.Exit(0);
                }
                else if (res.StatusCode == HttpStatusCode.OK) //200
                { }
                else if (res.StatusCode == HttpStatusCode.BadRequest)//400
                { }
                else if (res.StatusCode == HttpStatusCode.NotFound)//404
                { }
                else if (res.StatusCode == HttpStatusCode.InternalServerError)//500
                {
                    
                }
                else if (res.StatusCode == HttpStatusCode.Unauthorized)//401
                {
                    MessageBox.Show("账号密码错误或者可能是账号密码错误次数过多,退出程序", "警告");
                    new setting().ShowDialog();
                    Environment.Exit(0); }
                else
                { }
            }
        }
        public static string Base64_Convert(string text)
        {
            byte[] bytes = Encoding.Default.GetBytes(text);
            string str = Convert.ToBase64String(bytes);
            string urlEncode_str = System.Web.HttpUtility.UrlEncode(str);
            return urlEncode_str;
        }

        // 删除[ ] 根据","分段
        public static string[] delete_brackets(string text)
        {
            string tmp_string = text.TrimStart('[').TrimEnd(']');
            return tmp_string.Split("\",\"");
        }

        // 调试用
       public static void WriteMessage(string msg)
        {
            using (FileStream fs = new FileStream(@"d:\test.txt", FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("{0}\n", msg, DateTime.Now);
                    sw.Flush();
                }
            }
        }
        // 杀死进程
        public static void killProcess(string processName)
        {
            Process[] p = Process.GetProcessesByName(processName);
            foreach (Process ps in p)
            {
                ps.Kill();
            }
        }
    }
}

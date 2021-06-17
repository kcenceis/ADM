using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using Microsoft.Win32;

namespace ADM
{
    public partial class setting : Window
    {
        public setting()
        {
            InitializeComponent();
            // 读取App.config
            string router = Utils.router;
            string port = Utils.port;
            string username = Utils.raw_username;
            string password = Utils.raw_password;
            textbox_router.Text = router;
            textbox_port.Text = port;
            textbox_username.Text = username;
            textbox_password.Text = password;
            if (Utils.http_protocol == "https")
            {
                checkbox_http_protocol.IsChecked = true;
            }
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            if (textbox_router.Text!=""&& textbox_port.Text!=""&& textbox_username.Text!=""&& textbox_password.Text!="")
            {
                // 写入App.config
                Configuration cmo = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cmo.AppSettings.Settings["router"].Value = textbox_router.Text;
                cmo.AppSettings.Settings["port"].Value = textbox_port.Text;
                cmo.AppSettings.Settings["username"].Value = textbox_username.Text;
                cmo.AppSettings.Settings["password"].Value = textbox_password.Text;
                cmo.Save();
                Utils.router = textbox_router.Text;
                Utils.port = textbox_port.Text;
                Utils.raw_username = textbox_username.Text;
                Utils.raw_password = textbox_password.Text;
                //Utils.http_post_data = 
                // // 退出并重启程序
                // System.Reflection.Assembly.GetEntryAssembly();
                // // string location = System.Reflection.Assembly.GetEntryAssembly().Location;
                // string fileName = Process.GetCurrentProcess().MainModule.FileName; // 获取完整目录文件名*.exe
                // // string startpath = System.IO.Directory.GetCurrentDirectory(); //获取目录
                // // System.Diagnostics.Process.Start(startpath + "\\ASUS_DM.exe"); 
                // System.Diagnostics.Process.Start(fileName); // 启动程序
                // Application.Current.Shutdown(); //关闭程序
                Utils.init();
                this.Close();
            }
            else
            {
                MessageBox.Show("数据不能为空");
            }

        }

        private void default_downloader_btn(object sender, RoutedEventArgs e)
        {

            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);
            if (windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                RegistryKey classroot = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("magnet");
                //RegistryKey magnetURL = classroot.CreateSubKey("@", true);
                classroot.SetValue("", "URL:Magnet link");
                classroot.SetValue("Content Type", "application/x-magnet");
                classroot.SetValue("URL Protocol", "");
               
                RegistryKey shellKey = classroot.CreateSubKey("shell", true);
                RegistryKey openKey = shellKey.CreateSubKey("open", true);
                RegistryKey commandKey = openKey.CreateSubKey("command", true);
                string DirectoryName = System.Environment.CurrentDirectory;


                commandKey.SetValue("", "\"" + DirectoryName + "\\ADM.exe\" \"%1\"");

                // 配置torrent注册表
                RegistryKey torrent = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(".torrent");
                torrent.SetValue("", "ADM");
                torrent.SetValue("Content Type", "application/x-bittorrent");
                // 设置torrent默认程序
                var Key = Registry.ClassesRoot.OpenSubKey(".torrent");
                var Type = Key.GetValue("");
                string command = "\"" + DirectoryName + "\\ADM.exe\" \"%1\"";
                string keyName = Type + @"\shell\Open\command";
                using (var key = Registry.ClassesRoot.CreateSubKey(keyName))
                {
                    key.SetValue("", command);
                }
            }

        }

        private void checkbox_http_protocol_Checked(object sender, RoutedEventArgs e)
        {
            Configuration cmo = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (checkbox_http_protocol.IsChecked==true)
            {
                Utils.http_protocol = "https";
                cmo.AppSettings.Settings["http_protocol"].Value = "https";
            }
            else
            {
                Utils.http_protocol = "http";
                cmo.AppSettings.Settings["http_protocol"].Value = "http";
            }
            cmo.Save();
        }
    }
}

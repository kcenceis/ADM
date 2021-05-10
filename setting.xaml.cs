using System.Configuration;
using System.Diagnostics;
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
            string router = ConfigurationManager.AppSettings["router"];
            string port = ConfigurationManager.AppSettings["port"];
            string username = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];
            textbox_router.Text = router;
            textbox_port.Text = port;
            textbox_username.Text = username;
            textbox_password.Text = password;
        }

        private void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            // 写入App.config
            Configuration cmo = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cmo.AppSettings.Settings["router"].Value = textbox_router.Text;
            cmo.AppSettings.Settings["port"].Value = textbox_port.Text;
            cmo.AppSettings.Settings["username"].Value = textbox_username.Text;
            cmo.AppSettings.Settings["password"].Value = textbox_password.Text;
            cmo.Save();
            // 退出并重启程序
            System.Reflection.Assembly.GetEntryAssembly();
            // string location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string fileName = Process.GetCurrentProcess().MainModule.FileName; // 获取完整目录文件名*.exe
            // string startpath = System.IO.Directory.GetCurrentDirectory(); //获取目录
            // System.Diagnostics.Process.Start(startpath + "\\ASUS_DM.exe"); 
            System.Diagnostics.Process.Start(fileName); // 启动程序
            Application.Current.Shutdown(); //关闭程序
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
            }

        }
    }
}

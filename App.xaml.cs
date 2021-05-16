using System;
using System.Diagnostics;
using System.Windows;

namespace ADM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void command_download_link(object sender, StartupEventArgs e)
        {

            //MessageBox.Show(e.Args[0].ToString());
            if (e.Args.Length != 0)
            {
                for (int i = 0; i < e.Args.Length; i++)
                {
                    if (e.Args[i].Contains("magnet:?xt=urn:btih:"))
                    {
                        if (!Utils.isInit)
                        {
                            Utils.init();
                        }
                        string url = Utils.router_url + "/downloadmaster/dm_apply.cgi";
                        string data = "action_mode=DM_ADD&download_type=5&again=no&usb_dm_url=" + e.Args[i];
                        Utils.HttpGet(url, data);
                    }
                }
            }

            // 检查是否已经存在ADM进程 存在则不再新建窗口
            Process[] app = Process.GetProcessesByName("ADM");
            if (app.Length > 1)
             {
                // Utils.WriteMessage("已经启动");
                Environment.Exit(0);
             }
             else
             {
                 // Utils.WriteMessage("需要启动");
                 new MainWindow().ShowDialog();
             }
            //Process[] vProcesses = Process.GetProcesses();
            //foreach (Process vProcess in vProcesses)
            //{
            //    if (vProcess.ProcessName.Equals("ADM",StringComparison.OrdinalIgnoreCase))
            //    {
            //        Utils.WriteMessage("asdasdasdasd");
            //        break;
            //    }
            //}
        }
    }
}

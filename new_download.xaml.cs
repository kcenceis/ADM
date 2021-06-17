using System.Configuration;
using System.Windows;

namespace ADM
{
    /// <summary>
    /// new_download.xaml 的交互逻辑
    /// </summary>
    public partial class new_download : Window
    {
        public new_download()
        {
            InitializeComponent();
            /*
             * 判断是否连接中包含磁链头部 
             * 再判断是否选择了直接下载(简化
             * 
             */
            if (Utils.direct_download == "True")
            {
                CheckBox_direct_download.IsChecked = true;
            }
            else
            {
                CheckBox_direct_download.IsChecked = false;
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string clipboard_text = Clipboard.GetText(TextDataFormat.Text);
                    string magnet = "magnet:?xt=urn:btih:";
                    if (clipboard_text.Contains(magnet))
                    {
                        download_link.Text = clipboard_text;
                    }
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = Utils.router_url + "/downloadmaster/dm_apply.cgi";
            string data = "action_mode=DM_ADD&download_type=5&again=no&usb_dm_url=" + download_link.Text;
            Utils.HttpGet(url, data);
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBox_direct_download.IsChecked == true)
            {
                Configuration cmo = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cmo.AppSettings.Settings["direct_download"].Value = "True";
                cmo.Save();
                Utils.direct_download = "True";
            }
            else
            {
                Configuration cmo = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cmo.AppSettings.Settings["direct_download"].Value = "False";
                cmo.Save();
                Utils.direct_download = "False";
            }
        }
    }
}

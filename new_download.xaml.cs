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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = Utils.router_url +"/downloadmaster/dm_apply.cgi";
            string data = "action_mode=DM_ADD&download_type=5&again=no&usb_dm_url="+download_link.Text;
            Utils.HttpGet(url,data);
            this.Close();
        }
    }
}

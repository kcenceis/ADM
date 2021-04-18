using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ADM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<Person> s = new BindingList<Person>();
        int s_count = 0;
        public MainWindow()
        {
            InitializeComponent();
            
            
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                Utils.init();
                
                // 初始化列表,取数据
                string result = Utils.HttpGet(Utils.router_url +"/downloadmaster/dm_print_status.cgi", "action_mode=All").Trim();
                // 若无数据则不生成列表数据
                if (result.Length > 0)
                {
                    string[] new_list = result.Split("\n,");
                    for (int i = 0; i < new_list.Length; i++)
                    {
                        string tmp_string = new_list[i].TrimStart('[').TrimEnd(']');
                        string[] new_list_t = tmp_string.Split("\",\"");
                        s.Add(new Person
                        {
                            table_id = new_list_t[0].Replace("\"", ""),
                            title = new_list_t[1],
                            percent = new_list_t[2],
                            size = new_list_t[3],
                            status = new_list_t[4],
                            type = new_list_t[5],
                            time = new_list_t[6],
                            download_speed = new_list_t[7],
                            upload_speed = new_list_t[8],
                            peers = new_list_t[9]
                        });
                        //list_view.Items.Add(s);
                    }
                }

                s_count = s.Count; // 记录当前列表数,用于刷新时对比是否有增加减少
                list_view.ItemsSource = s; // 数据源
                // 定时
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(5000);
                timer.Tick += timer1_Tick;
                timer.Start();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string result = Utils.HttpGet(Utils.router_url + "/downloadmaster/dm_print_status.cgi", "action_mode=All").Trim();
            // 防止无数据
            if (result.Length > 0)
            {
                string[] new_list = result.Split("\n,");
                // 检查是否有新的下载增加
                if (s_count != new_list.Length)
                {
                    for (int i = s_count; i < new_list.Length; i++)
                    {
                        string tmp_string = new_list[i].TrimStart('[').TrimEnd(']');
                        string[] new_list_t = tmp_string.Split("\",\"");
                        s.Add(new Person
                        {
                            table_id = new_list_t[0].Replace("\"", ""),
                            title = new_list_t[1],
                            percent = new_list_t[2],
                            size = new_list_t[3],
                            status = new_list_t[4],
                            type = new_list_t[5],
                            time = new_list_t[6],
                            download_speed = new_list_t[7],
                            upload_speed = new_list_t[8],
                            peers = new_list_t[9]
                        });
                    }
                    s_count = s.Count;
                }
                // 没有新的下载增加,直接进行刷新操作
                else
                {
                    for (int i = 0; i < new_list.Length; i++)
                    {
                        string tmp_string = new_list[i].TrimStart('[').TrimEnd(']');
                        string[] new_list_t = tmp_string.Split("\",\"");
                        Person per = list_view.Items[i] as Person;
                        per.table_id = new_list_t[0].Replace("\"", "");
                        per.title = new_list_t[1];
                        per.percent = new_list_t[2];
                        per.size = new_list_t[3];
                        per.status = new_list_t[4];
                        per.type = new_list_t[5];
                        per.time = new_list_t[6];
                        per.download_speed = new_list_t[7];
                        per.upload_speed = new_list_t[8];
                        per.peers = new_list_t[9];
                    }
                }
            }
        }

        private void list_view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(list_view.SelectedItem.ToString());
            
            //按钮单击事件里写
            // Person per = list_view.Items[0] as Person;
            //   per.title = "AA";
        }
        private void new_button_Click(object sender, RoutedEventArgs e)
        {
            new_download nd = new new_download();
            nd.ShowDialog();
        }
        private void clean_Finish_Click(object sender, RoutedEventArgs e)
        {
            string url = Utils.router_url+"/downloadmaster/dm_apply.cgi";
            string data = "action_mode=DM_CTRL&dm_ctrl=clear&task_id=undefined&download_type=undefined";
            Utils.HttpGet(url, data);
        }

        /*
         *  删除按钮点击触发
         */
        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            // 防止无数据
            if (list_view.SelectedIndex > 0)
            {
                // 获取选取的列表数
                int list_view_num = list_view.SelectedIndex;
                // 获取列表内容
                Person per = list_view.Items[list_view_num] as Person;

                string url = Utils.router_url + "/downloadmaster/dm_apply.cgi";
                string data = "action_mode=DM_CTRL" +
                    "&dm_ctrl=cancel" +
                    "&task_id=" + per.table_id +
                    "&download_type=" + per.type;
                Utils.HttpGet(url, data);
                // 删除列表中的数据
                s.RemoveAt(list_view.SelectedIndex);
                s_count -= s_count;
            }
        }

        private void setting_button_Click(object sender, RoutedEventArgs e)
        {
            setting st = new setting();
            st.ShowDialog();
        }
    }
}

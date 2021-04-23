using System;
using System.ComponentModel;
using System.Threading;
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
        Thread myThread;
        BindingList<Person> s = new BindingList<Person>();
        public MainWindow()
        {
            InitializeComponent();
           // myThread = new Thread(doWork);
            Utils.init();
            // 初始化列表,取数据
            string result = Utils.HttpGet(Utils.router_url + "/downloadmaster/dm_print_status.cgi", "action_mode=All").Trim();

            // 若无数据则不生成列表数据
            if (result.Length > 0)
            {
                string[] new_list = result.Split("\n,");
                for (int i = 0; i < new_list.Length; i++)
                {
                    string[] new_list_t = Utils.delete_brackets(new_list[i]);
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
            list_view.ItemsSource = s; // 绑定数据源
                // 定时
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(5000);                // 5秒刷新一次列表
                timer.Tick += timer1_Tick;
                timer.Start();
            

        }

        private void doWork(object obj)
        {
            Utils.init();
            // 初始化列表,取数据
            string result = Utils.HttpGet(Utils.router_url + "/downloadmaster/dm_print_status.cgi", "action_mode=All").Trim();

            // 若无数据则不生成列表数据
            if (result.Length > 0)
            {
                string[] new_list = result.Split("\n,");
                for (int i = 0; i < new_list.Length; i++)
                {
                    string[] new_list_t = Utils.delete_brackets(new_list[i]);
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
            throw new NotImplementedException();
        }

        // 定时器方法
        private void timer1_Tick(object sender, EventArgs e)
        {
            string result = Utils.HttpGet(Utils.router_url + "/downloadmaster/dm_print_status.cgi", "action_mode=All").Trim();
            // 防止无数据
            if (result.Length > 0)
            {
                string[] new_list = result.Split("\n,");
                /*
                 * s.Count 为 当前列表的列表数 new_list为HTTP GET获取到的数据条目数
                 * 当前列表数 小于 获取数据条目数 则进行增加操作
                 * 
                 * 检测到传回来的下载条目增加
                 */
                if (s.Count < new_list.Length)
                {
                    for (int i = s.Count; i < new_list.Length; i++)
                    {
                        string[] new_list_t = Utils.delete_brackets(new_list[i]);
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
                }
                /* 
                 * 检测到传回来的下载条目减少
                 * 逻辑:
                 * table_id和title相同 则判定该条目没有被删除,continue跳出
                 * 若table_id对上 则代表该条目已经被代替,则进行更新
                 * 若table_id和titile对不上 则代表该条目已失效 删除
                 */
                else if (s.Count > new_list.Length)
                {
                    for (int i = 0; i < new_list.Length; i++)
                    {
                        string tmp_string = new_list[i].TrimStart('[').TrimEnd(']');
                        string[] new_list_t = tmp_string.Split("\",\"");
                        Person per = list_view.Items[i] as Person;
                        string new_list_table_id = new_list_t[0].Replace("\"", "");
                        string new_list_title = new_list_t[1];
                        if (per.table_id == new_list_table_id && per.title == new_list_title)
                        {
                            continue;
                        }
                        else if(per.table_id == new_list_table_id)
                        {
                            per.title = new_list_title;
                            per.percent = new_list_t[2];
                            per.size = new_list_t[3];
                            per.status = new_list_t[4];
                            per.type = new_list_t[5];
                            per.time = new_list_t[6];
                            per.download_speed = new_list_t[7];
                            per.upload_speed = new_list_t[8];
                            per.peers = new_list_t[9];
                        }
                        else
                        {
                            s.RemoveAt(i);
                        }
                    }
                }
                /*
                 * 没有新的下载增加,直接进行刷新操作
                 * 读取数据 刷新每一个条目
                 */ 
                else
                {
                    for (int i = 0; i < new_list.Length; i++)
                    {
                        string[] new_list_t = Utils.delete_brackets(new_list[i]);
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
            for ( int i = 0; i < s.Count; i++)
            {
                Person per = list_view.Items[i] as Person;
                if (per.status=="Seeding")
                {
                    s.RemoveAt(i);
                }
            }
        }

        /*
         *  删除按钮点击触发
         */
        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            // 防止无数据
            if (list_view.SelectedIndex >= 0)
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
            }
        }

        private void setting_button_Click(object sender, RoutedEventArgs e)
        {
            setting st = new setting();
            st.ShowDialog();
        }
    }
}

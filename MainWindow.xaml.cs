using System;
using System.ComponentModel;
using System.Configuration;
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
        BindingList<List_> s = new BindingList<List_>();
        GridView gridView;
        private static string magnet_temp = "";
        string[] gv_list_name = { "list_title_width",
                "list_percent_width",
                "list_size_width",
                "list_status_width",
                "list_time_width",
                "list_type_width",
                "list_download_speed_width",
                "list_upload_speed_width",
                "list_peers_width"
            };
        private static Boolean isCleaned = true;
        public MainWindow()
        {
            InitializeComponent();
            gridView = list_view.View as GridView;
            
            // 读取配置文件中的窗口大小
            int main_width= Convert.ToInt32(ConfigurationManager.AppSettings["Main_Width"]);
            int main_height= Convert.ToInt32(ConfigurationManager.AppSettings["Main_Height"]);
            this.Width = main_width;
            this.Height = main_height;

            for (int i = 0; i < gv_list_name.Length; i++)
            {
                string temp = ConfigurationManager.AppSettings[gv_list_name[i]];
                if (temp != "NaN")
                {
                    int temp_int = Convert.ToInt32(temp);
                    gridView.Columns[i+1].Width = temp_int;
                }
                else
                {
                    gridView.Columns[i+1].Width = gridView.Columns[i+1].ActualWidth;
                    gridView.Columns[i+1].Width = Double.NaN;
                }

            }

            if (Utils.isInit == false)
            {
                // 获取cookies 进行初始化
                Utils.init();
            }

            // 初始化列表,取数据
            string result = Utils.HttpGet(Utils.router_url + "/downloadmaster/dm_print_status.cgi", "action_mode=All").Trim();

            // 若无数据则不生成列表数据
            if (result.Length > 0)
            {
                string[] new_list = result.Split("\n,");
                for (int i = 0; i < new_list.Length; i++)
                {
                    string[] new_list_t = Utils.delete_brackets(new_list[i]);
                    s.Add(new List_
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
            // 5秒刷新一次列表
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);                
            timer.Tick += timer_Tick;
            timer.Start();
            // 1秒捕捉一次剪贴板,30分钟重新获取一次cookies
            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(1000);                
            timer1.Tick += timer1_Tick;
            timer1.Start();

        }



        // 捕捉剪贴板的timer方法
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime d2 = DateTime.Now; //获取当前时间
            Utils.timeSpan = d2 - Utils.d1; // 计算时间差
            if (!Utils.isError)
            {
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string clipboard_text = Clipboard.GetText(TextDataFormat.Text);
                    string magnet = "magnet:?xt=urn:btih:";
                    if (clipboard_text.Contains(magnet) && clipboard_text != magnet_temp)
                    {
                        string url = Utils.router_url + "/downloadmaster/dm_apply.cgi";
                        string data = "action_mode=DM_ADD&download_type=5&again=no&usb_dm_url=" + clipboard_text;
                        Utils.HttpGet(url, data);
                        magnet_temp = clipboard_text;
                        Clipboard.Clear();
                    }
                }


                                                // 10分钟重新获取一次cookies
                if (TimeSpan.FromMinutes(10) <= Utils.timeSpan)
                {
                    Utils.HttpPost(Utils.http_post_url, Utils.http_post_data); //获取cookies
                    Utils.d1 = DateTime.Now; //刷新时间
                }
            }
        }


        // 定时器方法
        private void timer_Tick(object sender, EventArgs e)
        {
            if (!Utils.isError)
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
                            s.Add(new List_
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

                        for (int i = s.Count; i < new_list.Length; i++)
                        {
                            string[] new_list_t = Utils.delete_brackets(new_list[i]);
                            s.Add(new List_
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
                        //for (int i = 0; i < s.Count; i++)
                        //{
                        //    string[] new_list_t = Utils.delete_brackets(new_list[i]);
                        //    List_ per = list_view.Items[i] as List_;
                        //    string new_list_table_id = new_list_t[0].Replace("\"", "");
                        //    string new_list_title = new_list_t[1];
                        //    if (per.table_id == new_list_table_id && per.title == new_list_title)
                        //    {
                        //        continue;
                        //    }
                        //    else if(per.table_id == new_list_table_id)
                        //    {
                        //        per.title = new_list_title;
                        //        per.percent = new_list_t[2];
                        //        per.size = new_list_t[3];
                        //        per.status = new_list_t[4];
                        //        per.type = new_list_t[5];
                        //        per.time = new_list_t[6];
                        //        per.download_speed = new_list_t[7];
                        //        per.upload_speed = new_list_t[8];
                        //        per.peers = new_list_t[9];
                        //    }
                        //    else
                        //    {
                        //        s.RemoveAt(i);
                        //    }
                        //}
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
                            List_ per = list_view.Items[i] as List_;
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
        }

        private void list_view_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void new_button_Click(object sender, RoutedEventArgs e)
        {
            // 不包含磁链头部 则打开下载窗口
            if (Utils.direct_download == "True")
            {
                if (Clipboard.ContainsText(TextDataFormat.Text))
                {
                    string clipboard_text = Clipboard.GetText(TextDataFormat.Text);
                    // 包含磁链头部 则直接下载
                    string magnet = "magnet:?xt=urn:btih:";
                    if (clipboard_text.Contains(magnet))
                    {
                        string url = Utils.router_url + "/downloadmaster/dm_apply.cgi";
                        string data = "action_mode=DM_ADD&download_type=5&again=no&usb_dm_url=" + clipboard_text;
                        Utils.HttpGet(url, data);

                        Clipboard.Clear(); //清除剪贴板
                    }
                    else
                    {
                        new_download nd = new new_download();
                        nd.ShowDialog();
                    }
                }
                else
                {
                    new_download nd = new new_download();
                    nd.ShowDialog();
                }
            }
            else
            {
                new_download nd = new new_download();
                nd.ShowDialog();
            }
        }
        private void clean_Finish_Click(object sender, RoutedEventArgs e)
        {
            
            if (isCleaned == true)
            {
                isCleaned = false;
                new Thread(Clean_Finish).Start();
                //this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
                //{
                //    //Clean_Finish(); 
                //    //s.Clear();
                //    for ( int i = 0; i < list_view.Items.Count; i++)
                //    {
                //        List_ per = list_view.Items[i] as List_;
                //        if (per.status=="Seeding"||per.status=="Paused")
                //        {
                //            s.Clear();
                //            break;
                //        }
                //    }
                //});
                
                // 线程更新UI
            }
            //string url = Utils.router_url+"/downloadmaster/dm_apply.cgi";
            //string data = "action_mode=DM_CTRL&dm_ctrl=clear&task_id=undefined&download_type=undefined";
            //s.Clear();
            //Utils.HttpGet(url, data);
            //for ( int i = 0; i < s.Count; i++)
            //{
            //    List_ per = list_view.Items[i] as List_;
            //    if (per.status=="Seeding"||per.status=="Paused")
            //    {
            //        s.RemoveAt(i);
            //    }
            //}
        }

        private void Clean_Finish()
        {
            string url = Utils.router_url + "/downloadmaster/dm_apply.cgi";
            string data = "action_mode=DM_CTRL&dm_ctrl=clear&task_id=undefined&download_type=undefined";
            // s.Clear();
            Utils.HttpGet(url, data);
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                //Clean_Finish(); 
                //s.Clear();
                for (int i = 0; i < list_view.Items.Count; i++)
                {
                    List_ per = list_view.Items[i] as List_;
                    if (per.status == "Seeding" || per.status == "Paused")
                    {
                        s.Clear();
                        break;
                        //s.RemoveAt(i);
                    }
                }
            });
            isCleaned = true;
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
                List_ per = list_view.Items[list_view_num] as List_;

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

        // 窗体关闭监听事件
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            
            Configuration cmo = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // 存储窗口大小
            // 防止转换格式时出现错误
            if ((int)this.Width == -2147483648)
            {
                // 发生错误则保存为默认值
                cmo.AppSettings.Settings["Main_Width"].Value = "834";
            }
            else
            {
                cmo.AppSettings.Settings["Main_Width"].Value = ((int)this.Width).ToString();

            }
            if ((int) this.Height== -2147483648)
            {
                cmo.AppSettings.Settings["Main_Height"].Value = "452";
            }
            else
            {
                cmo.AppSettings.Settings["Main_Height"].Value = ((int)this.Height).ToString();
            }
                
            // 存储列表的宽度
            for(int i = 0; i < gv_list_name.Length; i++)
            {
                // 防止转换格式时出现错误
                if (((int)gridView.Columns[i+1].Width) == -2147483648)
                {
                    cmo.AppSettings.Settings[gv_list_name[i]].Value = "NaN";
                }
                else
                {
                    cmo.AppSettings.Settings[gv_list_name[i]].Value = ((int)gridView.Columns[i+1].Width).ToString();
                }
            }
            cmo.Save();

        }
    }
}

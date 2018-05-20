﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Ink;
using System.Drawing;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Drawing.Drawing2D;
using calibration_app.SetOption;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Data;

namespace calibration_app
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool isGather = false;

        /// <summary>
        /// 表格容器根据所选选项卡布置表格
        /// </summary>
        private Grid ChartZone = new Grid
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
           
        };


        bool first = true;


        public bool IsGather
        {
            get => isGather;
            set {
                if ( value != false && value != true)
                {
                    isGather = false;
                } 
                else
                {
                    isGather = value;
                }
            } 
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        static string strConn = "";


        private List<SeriesCollection> seriesCollection = new List<SeriesCollection>();

        private List<List<string>> labels = new List<List<string>>();

        private List<Func<double, string>> yFormatter = new List<Func<double, string>>();
        public List<SeriesCollection> SeriesCollection { get => seriesCollection; set => seriesCollection = value; }
        public List<List<string>> Labels { get => labels; set => labels = value; }
        public List<Func<double, string>> YFormatter { get => yFormatter; set => yFormatter = value; }

        public MainWindow()
        {
            
            
            InitializeComponent();
            for (int i = 0; i < 4; i++)
            {

                SeriesCollection temp= new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Series 1",
                        Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                    },
                    new ScatterSeries
                    {
                        Title = "Series 2",
                        Values = new ChartValues<ObservablePoint>
                        {
                            new ObservablePoint(0, 5),
                            new ObservablePoint(1, 7),
                            new ObservablePoint(2, 6),
                            new ObservablePoint(3, 4),
                            new ObservablePoint(4, 5)
                        },
                        
                        PointGeometry = DefaultGeometries.Triangle,
                    },
                   
                };
                SeriesCollection.Add(temp);

                List<string> temp_L = new List<string> { "Jan", "Feb", "Mar", "Apr", "May" };
                Labels.Add(temp_L);
                Func<double, string> temp_YFormatter = value => value.ToString("C");
                YFormatter.Add(temp_YFormatter);
                

            }

            

            
            DataContext = this;
        }
        

        private void ImportSource_Click(object sender, RoutedEventArgs e)
        {
            bool IsGathering = isGather;
            if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据","错误");
            }
            
        }

        private void ImportCalibration_Click(object sender, RoutedEventArgs e)
        {
            bool IsGathering = isGather;
            if (IsGathering)
            {
                MessageBox.Show("数据采集中，请结束采集后导入需校准数据", "错误");
            }
            else
            {
                //打开一个文件选择框
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Excel文件";
                ofd.FileName = "";
                ofd.Filter = "Excel文件(*.xls)|*";
                DataTable dt = new DataTable();
                string filePath = Application.StartupPath + @"\Skills_TreeView\Skills.xls";

                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;";
                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    conn.Open();
                    string strExcel = "select * from [sheet1$]";
                    OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
                    myCommand.Fill(dt);
                }
                return dt;
            }
        }
        //private void ImportDataMenu_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("导入校准数据");
        //}

        /// <summary>
        /// 菜单栏采集状态切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GatherMenu_Click(object sender, RoutedEventArgs e)
        {
            bool state = IsGather;
            // 没有开始采集时开始采集，并将说明文字更改为结束采集
            SwitchGather(state);
        }

        /// <summary>
        /// 工具栏采集状态切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GatherCB_Click(object sender, RoutedEventArgs e)
        {
            bool state = IsGather;
            // 没有开始采集时开始采集，并将说明文字更改为结束采集
            SwitchGather(state);
        }

        private void CloseWindowMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingMenu_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow window = new SettingWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow window = new SettingWindow()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void USBBtn_Click(object sender,RoutedEventArgs e)
        {

        }





        /// <summary>
        /// 切换采集开关
        /// </summary>
        /// <param name="state">当前是否在采集中</param>
        public void SwitchGather(bool state)
        {
            if (!state)
            {
                // 将开始采集时间点写入到文件做记录
                FileStream fs = new FileStream(@".\Gather.txt", FileMode.Create);
                string start = DateTime.Now.ToString() + "\r\n";
                //获得字节数组
                byte[] data = System.Text.Encoding.Default.GetBytes(start);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                GatherCB.Content = GatherMenu.Header = "结束采集";
                
            }
            else
            {
                // 结束采集时将时间记录并写入文件
                FileStream fs = new FileStream(@".\Gather.txt", FileMode.Append);
                string start = DateTime.Now.ToString() + "\r\n";
                //获得字节数组
                byte[] data = System.Text.Encoding.Default.GetBytes(start);
                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
                GatherCB.Content = GatherMenu.Header = "开始采集";
            }
            IsGather = !state;
        }

        


        /// <summary>
        /// 页面载入成功时的逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            first = false;
            // 读取本地设置文件中的设置
            Setting setting = DeserializeFromXml<Setting>(@".\Setting.xml");
            // 遍历用户所做配置的字段
            foreach (Column temp in setting.Gather.ColumnList)
            {
                // 根据字段名称布置选项卡并添加至ColTab内
                TabItem myDnymicTab = new TabItem() { Header = temp.Name, MaxHeight = 20, MaxWidth = 78 };
                ColTab.Items.Add(myDnymicTab);

            }
            // 页面载入成功后直接选择第一个选项卡
            ColTab.SelectedItem = ColTab.Items[0];


            // 初始化chartZone的Grid控件
            ChartZone.RowDefinitions.Add(new RowDefinition());
            ChartZone.RowDefinitions.Add(new RowDefinition());
            ChartZone.ColumnDefinitions.Add(new ColumnDefinition());
            ChartZone.ColumnDefinitions.Add(new ColumnDefinition());

            SeriesCollection tem = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Series 1",
                        Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                    },
                    new ScatterSeries
                    {
                        Title = "Series 2",
                        Values = new ChartValues<ObservablePoint>
                        {
                            new ObservablePoint(0, 5),
                            new ObservablePoint(1, 7),
                            new ObservablePoint(2, 6),
                            new ObservablePoint(3, 4),
                            new ObservablePoint(4, 5)
                        },

                        PointGeometry = DefaultGeometries.Triangle,
                    },

                };
            

            List<string> temp_L = new List<string> { "Jan", "Feb", "Mar", "Apr", "May" };
           
            Func<double, string> temp_YFormatter = value => value.ToString("C");
            

            CartesianChart cartesianChart = new CartesianChart
            {
                Series = SeriesCollection[0],
                LegendLocation = LegendLocation.Right,
                AxisY = new AxesCollection {
                    new Axis{
                        Title = "Sales",
                        LabelFormatter = temp_YFormatter,
                    }
                },
                AxisX = new AxesCollection
                {
                    new Axis
                    {
                        Title = "month",
                        Labels = temp_L
                    }
                }
            };
            ChartZone.Children.Add(cartesianChart);
            cartesianChart.SetValue(Grid.RowSpanProperty,2);
            cartesianChart.SetValue(Grid.ColumnSpanProperty,2);

            //string dataPath = setting.Gather.DataPath;
            //string[] data = File.ReadAllLines(dataPath, Encoding.Default);
            //int i = 1;
            //DateTime startTime;
            // 遍历源数据，并按照需校准数据进行调整
            //foreach (string line in data)
            //{
            //    int j = 0;

            //    if (i >= 5)
            //    {
            //        string a = line;
            //        char[] sp = { ',', '"' };
            //        string[] datas = a.Split(sp, StringSplitOptions.RemoveEmptyEntries);
            //        Column column = setting.Gather.ColumnList[0];
            //        // 数据采集时间

            //        // 获取当前周期内的起始时间
            //        string date_temp = string.Format("{0:g}", dateTime);
            //        date_temp += ":00";
            //        // 获取当前周期内的频率
            //        double fre = 60 / column.Frequency;
            //        DateTime date_plus = (Convert.ToDateTime(date_temp).AddMinutes(fre * j));
            //        if (DateTime.Compare(dateTime, date_plus) < 0)
            //        {

            //        }
            //        if (i == 5)
            //        {
            //            startTime = Convert.ToDateTime(datas[0]);
            //        }


            //    }
            //    i++;
            //}


            //List<CartesianChart> cartesianChart = new CartesianChart
            //{
            //    Series = SeriesCollection[0],
            //    LegendLocation = LegendLocation.Right,
            //};
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            //Bitmap bit = new Bitmap(Convert.ToInt32(this.ChartZone.Width), Convert.ToInt32(this.ChartZone.Height));//实例化一个和窗体一样大的bitmap
            //Graphics g = Graphics.FromImage(bit);
            //g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
            //g.CopyFromScreen(Convert.ToInt32(this.Left), Convert.ToInt32(this.Top), 0, 0, new System.Drawing.Size(Convert.ToInt32(this.ChartZone.Width), Convert.ToInt32(this.ChartZone.Height)));//保存整个窗体为图片
            ////g.CopyFromScreen(panel游戏区 .PointToScreen(Point.Empty), Point.Empty, panel游戏区.Size);//只保存某个控件（这里是panel游戏区）
            //bit.Save("weiboTemp.png");//默认保存格式为PNG，保存成jpg格式质量不是很好
        }

        /// <summary>     
        /// 从某一XML文件反序列化到某一类型   
        /// </summary>    
        /// <param name="filePath">待反序列化的XML文件名称</param>  
        /// <param name="type">反序列化出的</param>  
        /// <returns></returns>    
        public static T DeserializeFromXml<T>(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                    throw new ArgumentNullException(filePath + " not Exists");
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        private void ColTab_Change(object sender, SelectionChangedEventArgs e)
        {
            if (!first)
            {
                int i = 0;
                foreach (TabItem it in ColTab.Items)
                {
                    var item = ColTab.ItemContainerGenerator.ContainerFromItem(ColTab.Items[i]) as TabItem;
                    string header = item.Header.ToString();
                    string[] headers = header.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    item.Header = headers[0];
                    item.Content = null;
                    i++;
                }
                var temp = ColTab.ItemContainerGenerator.ContainerFromItem(ColTab.SelectedItem) as TabItem;
                temp.Header += "-selected";
                temp.Content = ChartZone;
                //MessageBox.Show(ColTab.SelectedIndex.ToString());
            }
            
        }
    }
}

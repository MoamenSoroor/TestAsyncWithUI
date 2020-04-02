using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestAsyncWithUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        Button _button = new Button { Content = "Async Go" };
        Button _button2 = new Button { Content = "Go Without Async" };
        Button _button3 = new Button { Content = "Calc All Pages Length" };
        TextBlock _results = new TextBlock();
        public MainWindow()
        {
            InitializeComponent();
            var panel = new StackPanel();
            panel.Children.Add(_button);
            panel.Children.Add(_button2);
            panel.Children.Add(_button3);
            panel.Children.Add(_results);
            Content = panel;
            _button.Click += (sender, args) => AsyncGo();
            _button2.Click += (sender, args) => Task.Run(()=>GoWithoutAsync());
            _button3.Click += (sender, args) => CalcAllPagesLength();
        }

        async void CalcAllPagesLength()
        {
            _button3.IsEnabled = false;
            _results.Text = "";
            string[] urls = "www.albahari.com www.oreilly.com www.linqpad.net".Split();
            int totalLength = 0;
            try
            {
                foreach (string url in urls)
                {
                    var uri = new Uri("http://" + url);
                    byte[] data = await new WebClient().DownloadDataTaskAsync(uri);
                    _results.Text += "Length of " + url + " is " + data.Length +
                    Environment.NewLine;
                    totalLength += data.Length;
                }
                _results.Text += "Total length: " + totalLength + Environment.NewLine;

            }
            catch (WebException ex)
            {
                _results.Text += "Error: " + ex.Message;
            }
            finally { _button3.IsEnabled = true; }
        }

        async void AsyncGo()
        {
            _button.IsEnabled = false;
            _results.Text = "";
            for (int i = 1; i < 5; i++)
                _results.Text += await GetPrimesCountAsync(i * 1000000, 1000000) +
                " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1) +
                Environment.NewLine;
            _results.Text += Environment.NewLine;
            _button.IsEnabled = true;
        }
        
        Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(()=> ParallelEnumerable.Range(start, count).Count(n =>
          Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));

        }
        
        void GoWithoutAsync()
        {
            for (int i = 1; i < 5; i++)
            {
                int result = GetPrimesCount(i * 1000000, 1000000);
                Dispatcher.BeginInvoke(new Action(() =>
              _results.Text += result + " primes between " + (i * 1000000) +
              " and " + ((i + 1) * 1000000 - 1) + Environment.NewLine));
            }
            Dispatcher.BeginInvoke(new Action(() => _button.IsEnabled = true));
        }

        int GetPrimesCount(int start, int count)
        {
            return ParallelEnumerable.Range(start, count).Count(n =>
          Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0));

        }


    }
}

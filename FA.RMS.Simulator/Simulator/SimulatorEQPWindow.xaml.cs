using FA.Automation.MessageBus;
using RabbitMQLibary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Simitor
{
    /// <summary>
    /// SimulateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SimulateEQPWindow : Window
    {
        public SimulateEQPWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dir = new DirectoryInfo($@"{Environment.CurrentDirectory + "/Types/"}");
            var allEqpDirnames = dir.GetDirectories().Select(t => t.Name).ToList();
            cbEQPTypes.ItemsSource = allEqpDirnames;
            cbEQPTypes.SelectedIndex = 0;
        }
        private RabbitMQMessageBusForEAP rabbitMqEAP = new RabbitMQMessageBusForEAP();

        /// <summary>
        /// 模拟机台
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimulator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnSimulator.Background == Brushes.Red)
                {
                    App.ModiftyEqpStatus(this.Title, false);

                    rabbitMqEAP.Dispose();

                    btnSimulator.Background = Brushes.Green;
                    btnSimulator.Content = "打开连接";
                }
                else
                {
                    var eqpId = cbEQP.Text;
                    if (string.IsNullOrEmpty(eqpId))
                    {
                        MessageBox.Show("EQPID is Empty!");
                        return;
                    }

                    App.ModiftyEqpStatus(eqpId, true);

                    rabbitMqEAP.InitMqEAP(eqpId);
                    rabbitMqEAP.OnRmsReciveEvent += RMSRabbitMq_OnRmsReciveEvent;

                    this.Title = eqpId;

                    btnSimulator.Background = Brushes.Red;
                    btnSimulator.Content = "关闭连接";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 处理RMS 请求的消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string RMSRabbitMq_OnRmsReciveEvent(string message)
        {
            Dispatcher.Invoke(() =>
            {
                if (tbRecive.Text.Length > 10000) tbRecive.Text = "";
                tbRecive.Text += DateTime.Now.ToString("HH:mm:ss") + "_来自RMS的消息: " + Environment.NewLine + Utill.FormatXml(message) + Environment.NewLine;
                tbRecive.ScrollToEnd();
            });

            try
            {
                var response = RMSMessageHandle.HandleRMSMessage(message);

                Dispatcher.Invoke(() =>
                {
                    if (tbSend.Text.Length > 10000) tbSend.Text = "";
                    tbSend.Text += DateTime.Now.ToString("HH:mm:ss") + "_回复RMS的消息： " + Environment.NewLine + Utill.FormatXml(response) + Environment.NewLine;
                    tbSend.ScrollToEnd();
                });

                return response;
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    if (tbSend.Text.Length > 10000) tbSend.Text = "";
                    tbSend.Text += DateTime.Now.ToString("HH:mm:ss") + "_处理RMS消息错误： " + ex.Message + Environment.NewLine;
                    tbSend.ScrollToEnd();
                });
                return null;
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                App.ModiftyEqpStatus(this.Title, false);
                rabbitMqEAP.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void cbEQPTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectValue = cbEQPTypes.SelectedValue.ToString();
            if (string.IsNullOrEmpty(selectValue))
                return;

            var dir = new DirectoryInfo($@"{Environment.CurrentDirectory + $"/Types/{selectValue}/"}");
            var allEqpDirnames = dir.GetDirectories().Where(t => t.Name != "ParseVersions").Select(t => t.Name).ToList();
            cbEQP.ItemsSource = allEqpDirnames;
            cbEQP.SelectedIndex = 0;
        }
    }
}

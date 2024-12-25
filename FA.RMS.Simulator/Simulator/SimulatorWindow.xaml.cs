using RabbitMQLibary;
using System.Windows;
using System.Windows.Controls;

namespace Simitor
{
    /// <summary>
    /// SimitorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SimitorWindow : Window
    {
        public SimitorWindow()
        {
            InitializeComponent();
        }

        private void cbSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (spEAP == null || spRMS == null)
                return;
            var conmobx = sender as ComboBox;

            if (conmobx != null)
            {
                if (conmobx.SelectedIndex == 0)
                {
                    spEAP.Visibility = Visibility.Collapsed;
                    spRMS.Visibility = Visibility.Visible;
                }
                else
                {
                    spEAP.Visibility = Visibility.Visible;
                    spRMS.Visibility = Visibility.Collapsed;
                }
            }
        }

        private RabbitMQMessageBusForEAP rabbitMqEAP = new RabbitMQMessageBusForEAP();
        private RabbitMQMessageBusForRMS rabbitMqRMS = new RabbitMQMessageBusForRMS();
        private string EAPRabbitMq_OnRmsReciveEvent(string arg)
        {
            Dispatcher.Invoke(() =>
            {
                tbRMSRecive.Text += DateTime.Now.ToString("HH:mm:ss") + "_RMS 接收到消息： " + arg + Environment.NewLine;
                tbRMSRecive.ScrollToEnd();
            });

            return arg + "_Callback";
        }

        private string RMSRabbitMq_OnRmsReciveEvent(string arg)
        {
            Dispatcher.Invoke(() =>
            {
                tbEAPRecive.Text += DateTime.Now.ToString("HH:mm:ss") + "_EAP 接收到消息： " + arg + Environment.NewLine;
                tbEAPRecive.ScrollToEnd();
            });

            return arg + "_Callback";
        }

        private async void ButtonRMS_Click(object sender, RoutedEventArgs e)
        {
            var sendContent = tbRMSSend.Text;
            var xxx = await rabbitMqRMS.RmsSendEapAsync(sendContent, tbRMSDeviceID.Text);
            tbRMSRecive.Text += DateTime.Now.ToString("HH:mm:ss") + "_RMS 接收到回复消息： " + xxx + Environment.NewLine;
            tbRMSRecive.ScrollToEnd();
        }

        private async void ButtonEAP_Click(object sender, RoutedEventArgs e)
        {
            var sendContent = tbEAPSend.Text;
            var xxx = await rabbitMqEAP.EapSendRmsAsync(sendContent);
            tbEAPRecive.Text += DateTime.Now.ToString("HH:mm:ss") + "_EAP 接收到回复消息： " + xxx + Environment.NewLine;
            tbEAPRecive.ScrollToEnd();
        }

        private void ButtonEAPInit_Click(object sender, RoutedEventArgs e)
        {
            var eqpId = tbEAPDeviceID.Text;
            if (string.IsNullOrEmpty(eqpId))
            {
                MessageBox.Show("EQPID is Empty!");
                return;
            }
            rabbitMqEAP.InitMqEAP(eqpId);
            rabbitMqEAP.OnRmsReciveEvent += RMSRabbitMq_OnRmsReciveEvent;
        }

        private void ButtonRMSInit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rabbitMqRMS.InitMqRMS();
                rabbitMqRMS.OnEapReciveEvent += EAPRabbitMq_OnRmsReciveEvent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                rabbitMqEAP.Dispose();
                rabbitMqRMS.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}

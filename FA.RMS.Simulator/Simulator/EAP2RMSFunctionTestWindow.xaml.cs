using FA.Automation.MessageBus;
using RabbitMQLibary;
using Simulator.Windows;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using static FA.Automation.MessageBus.Constant;
using static System.Reflection.Metadata.BlobBuilder;

namespace Simitor
{
    /// <summary>
    /// SimulateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EAP2RMSFunctionTestWindow : Window
    {
        public EAP2RMSFunctionTestWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dir = new DirectoryInfo($@"{Environment.CurrentDirectory + "/Types/"}");
            var allEqpDirnames = dir.GetDirectories().Select(t => t.Name).ToList();
            cbEQPTypes.ItemsSource = allEqpDirnames;
            cbEQPTypes.SelectedIndex = 0;

            tbLogs.Document = document;
        }
        private RabbitMQMessageBusForEAP rabbitMqEAP = new RabbitMQMessageBusForEAP();
        private string userId = "ADMIN";
        private string eqpId = string.Empty;
        private FlowDocument document = new FlowDocument();
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
                    eqpId = cbEQP.Text;
                    if (string.IsNullOrEmpty(eqpId))
                    {
                        MessageBox.Show("EQPID is Empty!");
                        return;
                    }
                    App.ModiftyEqpStatus(eqpId, true);
                    this.Title = eqpId;
                    rabbitMqEAP.InitMqEAP(eqpId);
                    rabbitMqEAP.OnRmsReciveEvent += RMSRabbitMq_OnRmsReciveEvent;

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
            LogInfo(message, "RMS->EAP", "RMS");

            var response = RMSMessageHandle.HandleRMSMessage(message);
            LogInfo(response, "EAP->RMS", "RMS");

            return response;
        }

        private bool sendFlag = false;

        private void btnSend2Rms_click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbEditMessage.Text))
            {
                MessageBox.Show("请输入消息！");
                return;
            }
            var sendMessage = tbEditMessage.Text;
            sendFlag = true;
            Task.Run(() =>
            {
                var responseFromRms = SendMessageToRms(sendMessage);
                var response2Rms = RMSMessageHandle.HandleRMSMessage(responseFromRms);

                // var callBackRespone = SendMessageToRms(response2Rms);
            });
        }

        public string SendMessageToRms(string message)
        {
            LogInfo(message, "EAP->RMS", "EAP");
            var response = rabbitMqEAP.EapSendRmsAsync(message).Result;
            LogInfo(response, "RMS->EAP", "EAP");

            return response;
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="messageContent">日志message</param>
        /// <param name="title">日志标题</param>
        /// <param name="messageDircetionCenter">日志消息走向 RMS->EAP EAP ->RMS RMS主动请求 和 EAP回复的 都为 Blue，EAP主动请求和RMS回复的都为 green</param>
        public void LogInfo(string messageContent, string title = "", string messageDircetion = "EAP")
        {
            Dispatcher.Invoke(() =>
            {
                if (document.Blocks.Count > 50) document.Blocks.Remove(document.Blocks.First());

                var messageBody = title + "_" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ":" + Environment.NewLine + Utill.FormatXml(messageContent);
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(messageBody) { Foreground = messageDircetion == "EAP" ? Brushes.Green : Brushes.Blue });
                document.Blocks.Add(paragraph);

                tbLogs.ScrollToEnd();
            });
        }

        private void btnGenerat_click(object sender, RoutedEventArgs e)
        {
            var productId = "AP0001";
            var lotType = "Product";
            var recipeId = "Recipe1";
            switch (cbFuncton.Text)
            {
                case "NeedBeCheckParam":
                    tbEditMessage.Text = Utill.FormatXml(RMSMessageBuilder.BuildNeedBeCheckParameterListRequestXML(Guid.NewGuid().ToString(), eqpId, productId, lotType, recipeId));
                    break;
                case "CheckEqpParam":
                    var ec = new Dictionary<string, string>() { { "1", "1" }, { "2", "2" } };
                    var sv = new Dictionary<string, string>() { { "10", "10" }, { "20", "20" } };
                    tbEditMessage.Text = Utill.FormatXml(RMSMessageBuilder.BuildParamererCheckRequestXML(userId, Guid.NewGuid().ToString(), eqpId, productId, lotType, recipeId, ec, sv));
                    break;
                case "CheckRecipeBody":
                    var window = new CondfigCheckRecipeBodyWindow(eqpId);
                    window.ConfigComplateEvent += ConfigComplateEvent;
                    window.ShowDialog();
                    break;
                case "Download":
                    break;
                default:
                    break;
            }
        }
        //"recipeid", "productid", "lotTyp", "portId", "RecipeBody", "RecipeFormated"

        private void ConfigComplateEvent(string recipeid, string productid, string lotType, string portId, string RecipeFormated, string RecipeBody)
        {
            tbEditMessage.Text = Utill.FormatXml(RMSMessageBuilder.BuildRecipeBodyCheckRequestXML(userId, Guid.NewGuid().ToString(), eqpId, recipeid, productid, lotType, portId, RecipeBody, RecipeFormated));
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

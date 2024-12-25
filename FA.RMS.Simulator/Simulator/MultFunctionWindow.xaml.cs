using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Simitor
{
    /// <summary>
    /// MultFunctionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MultFunctionWindow : Window
    {
        public MultFunctionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 模拟机台
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEqp_click(object sender, RoutedEventArgs e)
        {
            var newWindow = new SimulateEQPWindow();
            newWindow.Show();
        }

        /// <summary>
        /// EAP请求RMS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEapCallRms_click(object sender, RoutedEventArgs e)
        {
            var newWindow = new EAP2RMSFunctionTestWindow();
            newWindow.Show();
        }

        /// <summary>
        /// 性能测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPerformance_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

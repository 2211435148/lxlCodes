using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Windows;
namespace Simitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<(string eqpId, bool isOpen)> EqpStatus = new List<(string eqpId, bool isOpen)>();

        /// <summary>
        /// 修改设备状态
        /// </summary>
        /// <param name="eqpId"></param>
        /// <param name="targetEqpStatus"></param>
        public static void ModiftyEqpStatus(string eqpId, bool targetEqpStatus)
        {
            var queryItem = EqpStatus.FirstOrDefault(t => t.eqpId == eqpId);
            if (queryItem != (null, false))
            {
                //如果关闭 则移出
                if (!targetEqpStatus)
                    EqpStatus.Remove(queryItem);

                if (queryItem.isOpen == targetEqpStatus)
                    throw new Exception("当前已经打开，不能重复打开！");

                queryItem.isOpen = targetEqpStatus;
            }
            else
            {
                EqpStatus.Add((eqpId, targetEqpStatus));
            }
        }

        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 处理由AppDomain引发的异常
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // 记录异常信息
            string errorMsg = $"Unhandled Exception: {e.Exception.Message}";
            errorMsg += Environment.NewLine;
            errorMsg += e.Exception.StackTrace;

            MessageBox.Show(errorMsg);
        }
    }
}

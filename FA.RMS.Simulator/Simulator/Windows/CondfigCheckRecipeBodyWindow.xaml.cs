using FA.Automation.MessageBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static FA.Automation.MessageBus.Constant;

namespace Simulator.Windows
{
    /// <summary>
    /// CondfigCheckRecipeBodyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CondfigCheckRecipeBodyWindow : Window
    {
        //"recipeid", "productid", "lotType", "portId", "RecipeBody", "RecipeFormated"
        public Action<string, string, string, string, string, string> ConfigComplateEvent;
        private string EqpId;
        public CondfigCheckRecipeBodyWindow(string eqpId)
        {
            InitializeComponent();

            EqpId = eqpId;
            cbRecipeId.ItemsSource = GetRecipeList(eqpId);
        }

        public List<string> GetRecipeList(string eqpId)
        {
            var eqpType = RMSMessageHandle.QueryEqpType(eqpId);
            var dir = new DirectoryInfo(Environment.CurrentDirectory + $"/Types/{eqpType}/{eqpId}/");

            var recipeList = new List<string>();
            var configFilePath = dir + @"\recipeList.json";
            if (File.Exists(configFilePath))
            {
                var configContent = File.ReadAllText(configFilePath);
                var configRecipeList = JsonConvert.DeserializeObject<List<RecipeModel>>(configContent);
                var currentRecipeList = configRecipeList.Where(t => t.RecipeCategory == RecipeCategroyEnum.Main).Select(t => t.RecipeId).ToList();
                recipeList = currentRecipeList.Select(t => t).ToList();
            }
            else
            {
                recipeList = dir.GetFiles().Where(t => t.Name.EndsWith(".txt")).Select(t => t.Name.Replace(".txt", "")).ToList();
            }

            return recipeList;
        }

        public string GetRecipeBody(string eqpId, string recipeId)
        {
            try
            {
                var eqpType = RMSMessageHandle.QueryEqpType(eqpId);
                var dir = new DirectoryInfo(Environment.CurrentDirectory + $"/Types/{eqpType}/{eqpId}/");

                string convertRecipeId = recipeId;
                var recipeList = new List<string>();
                var configFilePath = dir + @"\recipeList.json";
                if (File.Exists(configFilePath))
                {
                    var configContent = File.ReadAllText(configFilePath);
                    var configRecipeList = JsonConvert.DeserializeObject<List<RecipeModel>>(configContent);
                    var findItem = configRecipeList.FirstOrDefault(t => t.RecipeId == recipeId);
                    if (findItem != null)
                    {
                        convertRecipeId = findItem.RecipeFileName;
                    }
                }
                else
                {
                    convertRecipeId = recipeId.Replace("\\", "__");
                }

                //递归获取所有文件路径
                List<string> filePaths = Directory.GetFiles($"{Environment.CurrentDirectory}/Types/{eqpType}/{eqpId}/", "", SearchOption.AllDirectories).ToList();


                //找到对应recipeBody文件
                var recipePath = filePaths.FirstOrDefault(a => System.IO.Path.GetFileName(a) == $"{convertRecipeId}.txt");

                string replyMessage = File.ReadAllText(recipePath);

                if (!Utill.IsXml(replyMessage))
                {
                    //新增配置文件：指定设备不做Log->SECS转换(提前用工具转好了)，解决消息内容含有xml，会干掉部分xml内容的问题 -241118
                    var NotConvert_EQPList = File.ReadAllText($"{Environment.CurrentDirectory}/Types/NotConvert_EQPList.txt").Split(";", StringSplitOptions.RemoveEmptyEntries);

                    if (!NotConvert_EQPList.Contains(eqpId))
                    {
                        var recipeBody = File.ReadLines(recipePath).ToList();
                        for (int i = 0; i < recipeBody.Count(); i++)
                        {
                            recipeBody[i] = Regex.Replace(recipeBody[i], @"\[[0-9//]+\]", string.Empty);
                            recipeBody[i] = recipeBody[i].Replace("\"", "'");
                        }

                        replyMessage = RMSMessageHandle.ReplaceSpeciCode(recipeBody);
                    }
                }

                return replyMessage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void cbRecipeId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;
            if (string.IsNullOrEmpty(combox.Text.ToString()))
                return;

            //tbRecipeBody.Text = GetRecipeBody(EqpId, combox.Text.ToString());
        }

        private void btnSure_Click(object sender, RoutedEventArgs e)
        {
            //"recipeid", "productid", "lotType", "portId", "RecipeBody", "RecipeFormated"

            var recipeId = cbRecipeId.Text;
            var productid = tbProductid.Text;
            var lotType = cblotType.Text;
            var portId = tbportId.Text;
            var rRecipeFormated = cbRecipeFormated.Text;
            var rRecipeBody = GetRecipeBody(EqpId, cbRecipeId.Text.ToString());

            if (string.IsNullOrEmpty(recipeId) &&
                string.IsNullOrEmpty(productid) &&
                string.IsNullOrEmpty(lotType) &&
                string.IsNullOrEmpty(portId) &&
                string.IsNullOrEmpty(rRecipeFormated) &&
                string.IsNullOrEmpty(rRecipeBody)
                )
            {
                MessageBox.Show("信息补全！ 重新输入");
                return;
            }

            ConfigComplateEvent?.Invoke(recipeId, productid, lotType, portId, rRecipeFormated, rRecipeBody);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

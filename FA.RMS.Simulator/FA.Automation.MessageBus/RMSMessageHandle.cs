using MySecsDriver.Structure;
using Newtonsoft.Json;
using System;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Transactions;

namespace FA.Automation.MessageBus
{
    public class RMSMessageHandle
    {
        private static List<(string eqpType, List<string> eqpList)> _configEqps;
        public static List<(string eqpType, List<string> eqpList)> ConfigEqps
        {
            get
            {
                _configEqps = GetConfigEqps();
                return _configEqps;
            }
            set { _configEqps = value; }
        }

        public static List<(string eqpType, List<string> eqpList)> GetConfigEqps()
        {
            var resultList = new List<(string eqpType, List<string> eqpList)>();
            var typeDir = new DirectoryInfo(Environment.CurrentDirectory + $"/Types/");

            foreach (var item in typeDir.GetDirectories())
            {
                var eqpDir = new DirectoryInfo(Environment.CurrentDirectory + $"/Types/{item.Name}/");
                var eqpNameList = eqpDir.GetDirectories().Select(x => x.Name).ToList();

                resultList.Add((eqpType: item.Name, eqpList: eqpNameList));
            }

            return resultList;
        }

        public static string QueryEqpType(string eqpId)
        {
            var queryResult = ConfigEqps.FirstOrDefault(t => t.eqpList.Contains(eqpId));

            return queryResult.eqpType;
        }

        public static string HandleRMSMessage(string message)
        {
            try
            {
                var docObj = FA_EAP_RMS_Interface.GetXmlDoc(message);

                var messageName = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "MESSAGENAME");
                var messageTransctionID = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "TRANSACTIONID");
                var eqpId = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "EQPID");
                var userID = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "USERID");

                switch (messageName)
                {
                    //RMS 主动请求EAP 的接口
                    case RMSAction.RECIPELISTREQUEST:
                        return HandleRMSRecipeListRequest(eqpId, userID, messageTransctionID);

                    case RMSAction.PARAMETERVALUEREQUEST:
                        return HandleRMSParameterValueRequest(eqpId, userID, messageTransctionID);

                    case RMSAction.RECIPEBODYREQUEST:
                        var recipeId = FA_EAP_RMS_Interface.GetPropertyInnerTextFromDoc(docObj, "RECIPEID");
                        var recipeFormat = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "RECIPEFORMAT");
                        return HandleRMSRecipeBodyRequest(eqpId, userID, messageTransctionID, recipeId, recipeFormat);
                    case RMSAction.RECIPEBODYDOWNLOADREQUEST:
                        var recipeBody = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "RECIPEBODY");
                        var currentRecipeId = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "RECIPEID");
                        var recipeVer = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "RECIPEVER");
                        var fromEqp = FA_EAP_RMS_Interface.GetPropertyValueFromDoc(docObj, "FROMEQP");
                        return HandleRMSRecipeBodyDownLoadRequest(fromEqp, eqpId, userID, messageTransctionID, recipeBody, currentRecipeId, recipeVer);
                    //EAP主动请求RMS的接口
                    case RMSAction.RECIPEBODYCHECKREPLY:
                        return message;

                    case RMSAction.NEEDBECHECKPARAMETERLISTREPLY:
                        return message;

                    case RMSAction.PARAMETERCHECKREPLY:
                        return message;

                    default:
                        return default;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string HandleRMSRecipeBodyDownLoadRequest(string fromEqp, string currentEqpId, string userID, string messageTransctionID, string recipeBody, string recipeId, string recipeVer)
        {
            try
            {
                var replyMessage = RMSMessageBuilder.BuildRecipeBodyDownLoadReplyXML(userID, messageTransctionID, fromEqp, currentEqpId, recipeId, recipeVer, "OK", "下载成功");

                return replyMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region RMSRecipeListRequest

        /// <summary>
        /// 处理接收到RMS查询RecipeList
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="equipment"></param>
        /// <param name="transactionId"></param>
        protected static string HandleRMSRecipeListRequest(string eqpId, string userId, string transactionId)
        {
            try
            {
                var eqpType = QueryEqpType(eqpId);
                var dir = new DirectoryInfo(Environment.CurrentDirectory + $"/Types/{eqpType}/{eqpId}/");

                var recipeList = new List<string>();
                var configFilePath = dir + @"\recipeList.json";
                if (File.Exists(configFilePath))
                {
                    var configContent = File.ReadAllText(configFilePath);
                    var configRecipeList = JsonConvert.DeserializeObject<List<RecipeModel>>(configContent);
                    var currentRecipeList = configRecipeList.Where(t => t.RecipeCategory == RecipeCategroyEnum.Main).Select(t => t.RecipeId).ToList();
                    recipeList = currentRecipeList.Select(t => "<A>" + t + "</A>").ToList();
                }
                else
                {
                    recipeList = dir.GetFiles().Where(t => t.Name.EndsWith(".txt")).Select(t => "<A>" + t.Name.Replace(".txt", "") + "</A>").ToList();
                }
                var recipeListStr = "<L>" + string.Join(Environment.NewLine, recipeList) + "</L>";
                var replyMessage = RMSMessageBuilder.BuildRecipeListReplyXML(userId, transactionId, eqpId, recipeListStr, "", "");

                return replyMessage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion RMSRecipeListRequest

        #region ParameterCheck

        protected static string HandleRMSParameterValueRequest(string eqpId, string userId, string transactionId)
        {
            try
            {
                var ecDic = new Dictionary<string, string>();
                var svDic = new Dictionary<string, string>();

                for (int i = 0; i < 10; i++)
                {
                    ecDic.Add($"{i}", $"{i}");
                    svDic.Add($"{i * 10}", $"{i + 10}");
                }
                var replyMessage = RMSMessageBuilder.BuildParameterValueReplyXML(userId, transactionId, eqpId, ecDic, svDic);

                return replyMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected static string HandleRMSNeedeCheckParameterListReply(string eqpId)
        {
            try
            {

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected static string HandleRMSParameterCheckReply(string eqpId)
        {
            try
            {
                return default;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ParameterCheck

        #region RecipeBodyCheck

        protected static string HandleRMSRecipeBodyRequest(string eqpId, string userId, string transactionId, string recipeId, string recipeType)
        {
            try
            {
                var eqpType = QueryEqpType(eqpId);
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

                string replyMessage = string.Empty;

                //找到对应recipeBody文件
                var recipePath = filePaths.FirstOrDefault(a => Path.GetFileName(a) == $"{convertRecipeId}.txt");

                if (string.IsNullOrEmpty(recipePath))
                {
                    replyMessage = RMSMessageBuilder.BuildRecipeBodyReplyXML(userId, transactionId, eqpId, recipeId, recipeType, string.Empty, "NG", "Recipe Body File Not Exists");
                }
                else
                {
                    var recipeBodyStr = File.ReadAllText(recipePath);

                    if (Utill.IsXml(recipeBodyStr))
                    {
                        replyMessage = RMSMessageBuilder.BuildRecipeBodyReplyXML(userId, transactionId, eqpId, recipeId, recipeType, recipeBodyStr, "OK", "OK");

                        return replyMessage;
                    }

                    try
                    {
                    //新增配置文件：指定设备不做Log->SECS转换(提前用工具转好了)，解决消息内容含有xml，会干掉部分xml内容的问题 -241118
                    var NotConvert_EQPList = File.ReadAllText($"{Environment.CurrentDirectory}/Types/NotConvert_EQPList.txt").Split(";", StringSplitOptions.RemoveEmptyEntries);

                    var convertSecsStr = recipeBodyStr;
                    if (!NotConvert_EQPList.Contains(eqpId))
                    {
                        var recipeBody = File.ReadLines(recipePath).ToList();
                        for (int i = 0; i < recipeBody.Count(); i++)
                        {
                            recipeBody[i] = Regex.Replace(recipeBody[i], @"\[[0-9//]+\]", string.Empty);
                            recipeBody[i] = recipeBody[i].Replace("\"", "'");
                        }

                        convertSecsStr = ReplaceSpeciCode(recipeBody);
                    }

                        var secsMessage = SecsService.SecsMessageFromText(recipeBodyStr);
                        recipeBodyStr = SecsService.SecsMessageToXml(secsMessage.Children);
                    }
                    catch { }
                    finally
                    {
                        //如转换过程中出现问题，起码会把原来的数据送出去 -241118
                        replyMessage = RMSMessageBuilder.BuildRecipeBodyReplyXML(userId, transactionId, eqpId, recipeId, recipeType, recipeBodyStr, "OK", "OK");
                    }
                }

                return replyMessage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string ReplaceSpeciCode(List<string> originalStrList)
        {
            if (originalStrList.Count == 0)
                return default;

            try
            {
                for (int i = 0; i < originalStrList.Count; i++)
                {
                    var originalStr = originalStrList[i];
                    if (string.IsNullOrEmpty(originalStr))
                        continue;

                    var currentMatchItems = new Regex(@"<(.*?)>").Matches(originalStr);
                    ///解决 value值中有 , 被去除掉的问题 <A,161516 "safafasf"/A>
                    foreach (Match item in currentMatchItems)
                    {
                        var findValue = item.Value;

                        var index = findValue.IndexOf(",");
                        var firstIndex = findValue.IndexOf("\"");
                        var endIndex = findValue.LastIndexOf("\"");

                        if (firstIndex == -1 || endIndex == -1 || index == -1)
                            continue;

                        if (firstIndex == endIndex)
                            continue;

                        var headerStr = findValue.Substring(0, index);
                        var endStr = findValue.Substring(endIndex + 1, findValue.Length - endIndex - 1);
                        var subStr = findValue.Substring(firstIndex + 1, endIndex - firstIndex - 1);

                        var replacestr = headerStr + "'" + subStr + "'" + endStr;
                        originalStr = originalStr.Replace(findValue, replacestr);
                    }

                    var matchItemsFilter = new Regex("<[A-Z1-9]{1,2},\\d+").Matches(originalStr);
                    foreach (Match item in matchItemsFilter)
                    {
                        var findValue = item.Value;
                        var index = findValue.IndexOf(",");
                        if (index == -1)
                            continue;

                        var replacestr = findValue.Substring(0, index);
                        originalStr = originalStr.Replace(findValue, replacestr);
                    }

                    originalStrList[i] = originalStr;
                }

                return string.Join(Environment.NewLine, originalStrList);

            }
            catch (Exception)
            {
                throw;
            }
        }

        protected static string HandleRMSRecipeBodyCheckReply(string eqpId)
        {
            try
            {
                return default;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string FormatRecipeBody(string recipeBody)
        {
            var secsMessage = SecsService.SecsMessageFromText(recipeBody);
            var recipeBodyStr = SecsService.SecsMessageToXml(secsMessage.Children);

            return recipeBodyStr;
        }

        #endregion RecipeBodyCheck
    }
}

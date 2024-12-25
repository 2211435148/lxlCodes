using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using TIBCO.Rendezvous;
using System.Data;
using Fa.Automation.DataModel.Message;
using Newtonsoft.Json;
using System.Threading;
using log4net;
using System.Net;
using SqlSugar;
using FA.Automation.OracleCon.Dao;
using Fa.Automation.DataModel.Model;

namespace Fa.Automation.MessageBus
{
    public class MessageBus_TibcoRv : MessageBus
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(MessageBus));
        private string subject_name = string.Empty;
        private List<NetTransport> TransportList = new List<NetTransport>();
        private NetTransport CreateNetPort(string Service, string Network, string[] DaemonList)
        {
            NetTransport transport = null;
            if (DaemonList.Length == 0)
            {
                try
                {
                    transport = new NetTransport(Service, Network, "");
                    TransportList.Add(transport);
                }
                catch (Exception e)
                {
                    _log.Error("Tibco连接失败, " + e.Message);
                }
            }
            else
            {
                foreach (string daemon in DaemonList)
                {
                    try
                    {
                        transport = new NetTransport(Service, Network, daemon);
                        TransportList.Add(transport);
                    }
                    catch (Exception e)
                    {
                        _log.Error(daemon + "连接失败, " + e.Message);
                    }
                }
            }
            if (TransportList.Count > 0)
            {
                transport = TransportList[0];//默认选择第一个可以连的NetTransport
            }
            else
            {
                throw new Exception("Tibco连接失败");
            }
            return transport;
        }
        private void SendTibcoMessage(Message tibcoMsg)
        {
            Exception sendMessageException = null;
            bool sendResult = false;
            try
            {
                tibcorv_netTtransport.Send(tibcoMsg);
                sendResult = true;
            }
            catch (Exception e)
            {
                sendMessageException = e;
                //从另外一个daemon开始重发，因为系统初始化时采用第一个daemon,第一个daemon已经失败了
                for (int i = 1; i < TransportList.Count; i++)
                {
                    try
                    {
                        //将当前tibcorv_netTtransport切换到另外一个,至到成功为止
                        tibcorv_netTtransport = TransportList[i];
                        tibcorv_netTtransport.Send(tibcoMsg);
                        sendResult = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        sendMessageException = ex;
                        sendResult = false;
                    }
                }
            }
            if (!sendResult)
            {
                throw sendMessageException;
            }
        }
        public MessageBus_TibcoRv()
        {
            initialtimer();
            initialEapTimer();
        }
        public void initialtimer()
        {
            try
            {
                int second = int.Parse(ConfigurationManager.AppSettings["timeout"]);
                timer = new TimerClass();
                timer.initTimer(second * 1000);
            }
            catch (Exception ex)
            {
                _log.Error("Initial timer failed, exception: " + ex.Message);
            }
        }

        public void initialEapTimer()
        {
            try
            {
                int second = int.Parse(ConfigurationManager.AppSettings["timeout"]);
                eapTimer = new EAPTimer();
                eapTimer.initTimer(second * 1000);
            }
            catch (Exception ex)
            {
                _log.Error("Initial eapTimer failed, exception: " + ex.Message);
            }
        }
        public override void InitRmsServerMessageBus(ref string errMessage)
        {
            try
            {
                string strResult = string.Empty;

                tibcorv_Daemon = System.Configuration.ConfigurationManager.AppSettings["Daemon"];
                string[] tibcorv_DaemonList = tibcorv_Daemon.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                _log.Info("tibcorv_Daemon, " + tibcorv_Daemon);
                tibcorv_Service = ConfigurationManager.AppSettings["tibcorv_Service"];
                _log.Info("tibcorv_Service, " + tibcorv_Service);
                tibcorv_Network = ConfigurationManager.AppSettings["tibcorv_Network"];
                _log.Info("tibcorv_Network, " + tibcorv_Network);

                RMSServerSubject = ConfigurationManager.AppSettings["RMSServerSubject"];
                _log.Info("RMSServerSubject, " + RMSServerSubject);
                RMSClientSubject = ConfigurationManager.AppSettings["RMSClientSubject"];
                _log.Info("RMSClientSubject, " + RMSClientSubject);

                RMSServerWatchDogSubject = ConfigurationManager.AppSettings["RMSServerWatchDogSubject"];
                _log.Info("RMSServerWatchDogSubject, " + RMSServerWatchDogSubject);

                EAPSubject = ConfigurationManager.AppSettings["EAPSubject"];
                _log.Info("EAPSubject, " + EAPSubject);
                
                TIBCO.Rendezvous.Environment.Open();
                tibcorv_netTtransport = CreateNetPort(tibcorv_Service, tibcorv_Network, tibcorv_DaemonList);

                tibcorv_queue = new TIBCO.Rendezvous.Queue();
                tibcorv_RMSServerListen = new Listener(tibcorv_queue, tibcorv_netTtransport, RMSServerSubject, null);
                tibcorv_RMSServerListen.MessageReceived += new MessageReceivedEventHandler(tibcorv_RMSServerListen_MessageReceived);
                

                var dispacher = new Dispatcher(tibcorv_queue);
                dispacher.Resume();
            }
            catch (Exception ex)
            {
                errMessage = "Connect Tibco server fail! Reason:" + ex.Message;
                _log.Error(errMessage);
            }
        }
        public void tibcorv_RMSServerListen_MessageReceived(object listener, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            try
            {
                string msg = messageReceivedEventArgs.Message.GetField(ConfigurationManager.AppSettings["tibcorv_messageField"]);
                String sReplySubject = messageReceivedEventArgs.Message.ReplySubject;
                if (sReplySubject != null && sReplySubject.Equals(RMSClientSubject))
                {
                    OnRMSClientMessageReceived(msg);
                }
                else if (sReplySubject != null && sReplySubject.Equals(RMSServerWatchDogSubject))
                {
                    //收到监控工具发送的消息后,自动回复给RMS server监控工具
                    string RMSServerWatchDogTransationId= messageReceivedEventArgs.Message.GetField("TRANSACTIONID");
                    string RMSServerWatchDoReplyMsg = "I am here={TRANSACTIONID="+ RMSServerWatchDogTransationId + ";SERVERIP="+ GetCurrentServerIP() + "}";
                    Message tibcoMsg = new Message() { SendSubject = RMSServerWatchDogSubject, ReplySubject = RMSServerSubject };
                    tibcoMsg.AddField(ConfigurationManager.AppSettings["tibcorv_messageField"], RMSServerWatchDoReplyMsg);
                    tibcoMsg.AddField("TRANSACTIONID", RMSServerWatchDoReplyMsg);
                    SendTibcoMessage(tibcoMsg);
                    //暂时不写Log
                    //_log.Info("subject name, " + RMSServerWatchDogSubject+":"+ RMSServerWatchDoReplyMsg);
                    //
                }
                else if(sReplySubject != null && sReplySubject.Contains(EAPSubject))
                {
                    OnEAPMessageReceived(msg);
                }
                //临时解决EAP没有ReplySubject的问题
                else if (!string.IsNullOrEmpty(AOS_RMS_EAP_Interface.GetPropertyValueFromAOSEAPMessage(msg, "CMD")) && 
                    (ConfigurationManager.AppSettings["EapRuleList"].Split(',').Contains(AOS_RMS_EAP_Interface.GetPropertyValueFromAOSEAPMessage(msg, "CMD"))))
                {
                    OnEAPMessageReceived(msg);
                }
                else
                {
                    OnOtherMessageReceived(msg, sReplySubject);
                }


            }
            catch (Exception ex)
            {
                _log.Error("tibcorv_RMSListenEAP_MessageReceived error:" + ex.Message);
            }
        }

        //public void tibcorv_RMSServerListenEAP_MessageReceived(object listener, MessageReceivedEventArgs messageReceivedEventArgs)
        //{
        //    try
        //    {
        //        string msg = messageReceivedEventArgs.Message.GetField(ConfigurationManager.AppSettings["tibcorv_messageField"]);
        //        OnEAPMessageReceived(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error("tibcorv_RMSListenEAP_MessageReceived error:" + ex.Message);
        //    }
        //}


        public override void CloseSession()
        {
            try
            {
                if (tibcorv_queue != null)
                {
                    tibcorv_queue.Destroy();
                }
                if (tibcorv_netTtransport != null)
                {
                    tibcorv_netTtransport.Destroy();
                }
                foreach (NetTransport transport in TransportList)
                {
                    if (transport != null)
                    {
                        transport.Destroy();
                    }
                }
                TIBCO.Rendezvous.Environment.Close();
            }
            catch
            { }
        }
        public override bool SendMessageFromRmsServerToRmsClient(string message)
        {
            try
            {
                Message tibcoMsg = new Message() { SendSubject = RMSClientSubject, ReplySubject= RMSServerSubject };
                tibcoMsg.AddField(ConfigurationManager.AppSettings["tibcorv_messageField"], message);
                SendTibcoMessage(tibcoMsg);
                _log.Info("subject name, " + RMSClientSubject);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsToEAP Fail!Reason:" + ex.Message);
            }
        }

        public override bool SendMessageFromRmsServerToEAP(string message)
        {
            try
            {
                //发给EAP的消息都会带EQPID这个属性
                string equipmentId = AOS_RMS_EAP_Interface.GetPropertyValueFromAOSEAPMessage(message, "EQPID");
                Message tibcoMsg = new Message() { SendSubject = EAPSubject + "." + equipmentId, ReplySubject = RMSServerSubject };
                string maineqp = GetMainEqp(equipmentId);
                if (!string.IsNullOrEmpty(maineqp))
                {
                    tibcoMsg = new Message() { SendSubject = EAPSubject + "." + maineqp, ReplySubject = RMSServerSubject };
                }
                tibcoMsg.AddField(ConfigurationManager.AppSettings["tibcorv_messageField"], message);
                SendTibcoMessage(tibcoMsg);
                _log.Info("subject name, " + tibcoMsg.SendSubject);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsToEAP Fail!Reason:" + ex.Message);
            }
        }
        public override bool SendMessageFromRmsServerToEAP(string message, string eqpId, string transactionId, string fromRmsmessage)
        {
            try
            {
                Message tibcoMsg = new Message() { SendSubject = EAPSubject + "." + eqpId, ReplySubject = RMSServerSubject };
                string maineqp = GetMainEqp(eqpId);
                if (!string.IsNullOrEmpty(maineqp))
                {
                    tibcoMsg = new Message() { SendSubject = EAPSubject + "." + maineqp, ReplySubject = RMSServerSubject };
                }
                tibcoMsg.AddField(ConfigurationManager.AppSettings["tibcorv_messageField"], message);
                SendTibcoMessage(tibcoMsg);
                _log.Info("subject name, " + tibcoMsg.SendSubject);
                MessageContent content = new MessageContent();
                content.EQPID = eqpId;
                content.Message = message;
                content.TransactionId = transactionId;
                content.FromRmsMessage = fromRmsmessage;
                string strResult = "";
                eapTimer.TransactionInsert(content, ref strResult);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsToEAP Fail!Reason:" + ex.Message);
            }
        }
        public override bool SendMessageFromRmsServerToOtherSystem(string msg, string subject)
        {
            try
            {
                Message tibcoMsg = new Message() { SendSubject = subject, ReplySubject = RMSServerSubject };
                tibcoMsg.AddField(ConfigurationManager.AppSettings["tibcorv_messageField"], msg);
                SendTibcoMessage(tibcoMsg);
                _log.Info("subject name, " + tibcoMsg.SendSubject);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsServerToOtherSystem Fail!Reason:" + ex.Message);
            }
        }
        public string GetCurrentServerIP()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] iplist = Dns.GetHostAddresses(hostName);
            string ip = "";
            foreach (var ipAddress in iplist)
            {
                if (ipAddress.AddressFamily.ToString().Equals("InterNetwork"))
                {
                    ip = ipAddress.ToString();
                }
            }
            return ip;
        }
        public string GetMainEqp(string eqpID)
        {
            List<string> sugar = new List<string>();
            using (SqlSugarClient db = new SugarDao<Rms_Eqp>().Db) //开启数据库连接
            {
                try
                {
                  sugar = db.Queryable<Rms_Eqp>().Where(t => t.Eqp_Id.Equals(eqpID)).Select(t=>t.Main_EQP_ID).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return sugar.FirstOrDefault();
        }
    }
}
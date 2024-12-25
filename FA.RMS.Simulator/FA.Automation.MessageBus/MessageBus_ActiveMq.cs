using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Fa.Automation.DataModel.Message;
using log4net;
using Newtonsoft.Json;

namespace Fa.Automation.MessageBus
{
    public class MessageBus_ActiveMq : MessageBus
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(MessageBus));
        public MessageBus_ActiveMq()
        {
            initialtimer();
            initialEapTimer();
        }
        /// <summary>
        /// 按照配置文件的uri,queue通道来初始化messagebus
        /// </summary>
        /// <param name="equipmentId"></param>
        /// <param name="errMessage"></param>
        public override void InitRmsServerMessageBus(ref string errMessage)
        {
            try
            {
                activemq_connectionFactory = new ConnectionFactory(ConfigurationManager.AppSettings["ACTIVEMQ_URL"]);
                activemq_connection = activemq_connectionFactory.CreateConnection();
                activemq_connection.Start();
                ISession session = activemq_connection.CreateSession();
                string consumerTopicFromRmsClientStr = ConfigurationManager.AppSettings["RMSCLIENTTORMSServerSubject"];
                string producerTopicToRmsClientStr = ConfigurationManager.AppSettings["RMSServerTORMSCLIENTSubject"];

                string consumerTopicFromEAPStr = ConfigurationManager.AppSettings["EAPTORMSServerSubject"];
                string producerTopicToEAPStr = ConfigurationManager.AppSettings["RMSServerTOEAPSubject"];

                rms_Consume_rmsClient_Topic_listener = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(consumerTopicFromRmsClientStr), "name", "filter='demo'", false);
                rms_Consume_rmsClient_Topic_listener.Listener += new MessageListener(rms_Consume_RmsClient_Topic_listener_Listener);
                rms_produce_rmsClient_Topic_sender = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(producerTopicToRmsClientStr));

                rms_Consume_EAP_Topic_listener = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(consumerTopicFromEAPStr), "name", "filter='demo'", false);
                rms_Consume_EAP_Topic_listener.Listener += new MessageListener(rms_Consume_EAP_Topic_listener_Listener);
                rms_produce_EAP_Topic_sender = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(producerTopicToEAPStr));
                initialtimer();
            }
            catch (Exception ex)
            {
                errMessage = "Create Connection Factory Fail!Reason:" + ex.Message;
            }
        }
        public override void InitAlsServerMessageBus(ref string errMessage)
        {
            try
            {
                activemq_connectionFactory = new ConnectionFactory(ConfigurationManager.AppSettings["ACTIVEMQ_URL"]);
                activemq_connection = activemq_connectionFactory.CreateConnection();
                activemq_connection.Start();
                ISession session = activemq_connection.CreateSession();

                string consumerTopicFromEAPStr = ConfigurationManager.AppSettings["EAPTORMSServerSubject"];
                string producerTopicToEAPStr = ConfigurationManager.AppSettings["RMSServerTOEAPSubject"];
                rms_Consume_EAP_Topic_listener = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(consumerTopicFromEAPStr), "name", "filter='demo'", false);
                rms_Consume_EAP_Topic_listener.Listener += new MessageListener(rms_Consume_EAP_Topic_listener_Listener);
                rms_produce_EAP_Topic_sender = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(producerTopicToEAPStr));
                initialtimer();
            }
            catch (Exception ex)
            {
                errMessage = "Create Connection Factory Fail!Reason:" + ex.Message;
            }
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
        public void rms_Consume_RmsClient_Topic_listener_Listener(IMessage message)
        {
            try
            {
                //收到Message后的处理
                ITextMessage txtMessage = (ITextMessage)message;
                txtMessage.Acknowledge();
                string msg = txtMessage.Text;
                OnRMSClientMessageReceived(msg);
                
            }
            catch (Exception ex)
            {
                throw new Exception("rms_Consume_RmsClient_Topic_listener_Listener Fail!Reason:" + ex.Message);
            }
        }

        public void rms_Consume_EAP_Topic_listener_Listener(IMessage message)
        {
            try
            {
                //收到Message后的处理
                ITextMessage txtMessage = (ITextMessage)message;
                txtMessage.Acknowledge();
                string msg = txtMessage.Text;
                OnEAPMessageReceived(msg);
            }
            catch (Exception ex)
            {
                throw new Exception("rms_Consume_EAP_Topic_listener_Listener Fail!Reason:" + ex.Message);
            }
        }


        public override bool SendMessageFromRmsServerToRmsClient(string msg)
        {
            try
            {
                ITextMessage textMessage = rms_produce_rmsClient_Topic_sender.CreateTextMessage();
                //设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性
                textMessage.Properties.SetString("filter", "demo");
                RmsMessage message = JsonConvert.DeserializeObject<RmsMessage>(msg);

                textMessage.NMSCorrelationID = message.Guid;
                textMessage.Text = JsonConvert.SerializeObject(message);
                rms_produce_rmsClient_Topic_sender.Send(textMessage, Apache.NMS.MsgDeliveryMode.NonPersistent, Apache.NMS.MsgPriority.Normal, TimeSpan.MinValue);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsToRmsServer Fail!Reason:" + ex.Message);
            }
            
        }
        public override bool SendMessageFromRmsServerToEAP(string msg)
        {
            try
            {
                ITextMessage textMessage = rms_produce_EAP_Topic_sender.CreateTextMessage();
                //设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性
                textMessage.Properties.SetString("filter", "demo");
                textMessage.Text = msg;
                rms_produce_EAP_Topic_sender.Send(textMessage, Apache.NMS.MsgDeliveryMode.NonPersistent, Apache.NMS.MsgPriority.Normal, TimeSpan.MinValue);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsServerToEAP Fail!Reason:" + ex.Message);
            }

        }
        public override bool SendMessageFromRmsServerToEAP(string message, string eqpId, string transactionId, string fromRmsmessage)
        {
            try
            {
                ITextMessage textMessage = rms_produce_EAP_Topic_sender.CreateTextMessage();
                //设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性
                textMessage.Properties.SetString("filter", "demo");
                textMessage.Text = message;
                MessageContent content = new MessageContent();

                content.EQPID = eqpId;
                content.Message = message;
                content.TransactionId = transactionId;
                content.FromRmsMessage = fromRmsmessage;
                string strResult = "";
                eapTimer.TransactionInsert(content, ref strResult);

                rms_produce_EAP_Topic_sender.Send(textMessage, Apache.NMS.MsgDeliveryMode.NonPersistent, Apache.NMS.MsgPriority.Normal, TimeSpan.MinValue);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsServerToEAP Fail!Reason:" + ex.Message);
            }

        }

        public override bool SendMessageFromAlsServerToEAP(string msg)
        {
            try
            {
                ITextMessage textMessage = rms_produce_EAP_Topic_sender.CreateTextMessage();
                //设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性
                textMessage.Properties.SetString("filter", "demo");
                textMessage.Text = msg;
                rms_produce_EAP_Topic_sender.Send(textMessage, Apache.NMS.MsgDeliveryMode.NonPersistent, Apache.NMS.MsgPriority.Normal, TimeSpan.MinValue);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromAlsServerToEAP Fail!Reason:" + ex.Message);
            }

        }
        public override bool SendMessageFromRmsServerToOtherSystem(string msg, string subject)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("SendMessageFromRmsServerToOtherSystem Fail!Reason:" + ex.Message);
            }
        }
        public override void CloseSession()
        {
            if (activemq_connection != null)
                activemq_connection.Close();
            if (timer != null)
                timer.Destory();
            rms_Consume_rmsClient_Topic_listener = null;
            rms_produce_rmsClient_Topic_sender = null;
            rms_Consume_EAP_Topic_listener = null;
            rms_produce_EAP_Topic_sender = null;
        }
    }
}

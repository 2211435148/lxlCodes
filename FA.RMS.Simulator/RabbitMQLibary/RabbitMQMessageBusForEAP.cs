using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Configuration;
using System.Text;

namespace RabbitMQLibary
{
    public class RabbitMQMessageBusForEAP
    {
        private ConnectionFactory factory = null;
        private IConnection connention;
        private IModel channelEap2Rms;
        private IModel channelRms2Eap;
        private string TimeOutTime = ConfigurationManager.AppSettings["TimeOutTime"]?.ToString();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> callbackMapper = new();
        private string eap2RmsExchange = ConfigurationManager.AppSettings["Eap2RmsExchangeName"]?.ToString();
        private string rms2EapExchange = ConfigurationManager.AppSettings["Rms2EapExchangeName"]?.ToString();
        private string CallBackQueueName;
        private string EqpId;

        public Func<string, string> OnRmsReciveEvent;
        public void InitMqEAP(string eqpId)
        {
            EqpId = eqpId;
            var host = ConfigurationManager.AppSettings["Host"]?.ToString();
            var port = ConfigurationManager.AppSettings["Port"]?.ToString();
            var virtualHost = ConfigurationManager.AppSettings["VirtualHost"]?.ToString();
            var userName = ConfigurationManager.AppSettings["UserName"]?.ToString();
            var password = ConfigurationManager.AppSettings["Password"]?.ToString();

            factory = new ConnectionFactory()
            {
                HostName = host,
                Port = int.Parse(port),
                VirtualHost = virtualHost,
                UserName = userName,
                Password = password
            };
            connention = factory.CreateConnection();

            //TODO:原则:谁接收数据，谁定义路由规则

            // RMS-->EAPServer  EAP作为服务端 接受RMS请求 定义 RMS发送给EAP的exchange
            //定义RMS发给EAP 的model和 交换机---路由规则由接收端 EAP设置 
            channelRms2Eap = connention.CreateModel();
            channelRms2Eap.ExchangeDeclare(rms2EapExchange, ExchangeType.Direct, false, false);
            var eapQueueName = channelRms2Eap.QueueDeclare(eqpId).QueueName;
            channelRms2Eap.QueueBind(eapQueueName, rms2EapExchange, eqpId);
            var rms2EapConsumer = new EventingBasicConsumer(channelRms2Eap);
            rms2EapConsumer.Received += RMSRecive_Received;

            channelRms2Eap.BasicConsume(consumer: rms2EapConsumer,
                                 queue: eapQueueName,
                                 autoAck: true);
            channelEap2Rms = connention.CreateModel();
            channelEap2Rms.ExchangeDeclare(eap2RmsExchange, ExchangeType.Direct, false, false);

            //EAPServer-->RMS EAP作为客户端 请求RMS  定义 eap发送给RMS的exchange 以及返回的queue
            var callBackEapQueueName = channelEap2Rms.QueueDeclare(eqpId + "_EAP_RPC_CallBackQueue" + DateTime.Now.ToString("yyyyMMmddHHmmss")).QueueName;
            channelRms2Eap.QueueBind(callBackEapQueueName, rms2EapExchange, eqpId);

            var rms2EapConsumerCallBack = new EventingBasicConsumer(channelRms2Eap);
            rms2EapConsumerCallBack.Received += RmsRecive_CallbackReceived;

            channelEap2Rms.BasicConsume(consumer: rms2EapConsumerCallBack,
                                 queue: callBackEapQueueName,
                                 autoAck: true);
        }

        /// <summary>
        /// 接收RMS 发送的请求。处理后返回
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ea"></param>
        private void RMSRecive_Received(object model, BasicDeliverEventArgs ea)
        {
            string response = string.Empty;

            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            var replyProps = channelEap2Rms.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;
            try
            {
                var message = Encoding.UTF8.GetString(body);
                response = OnRmsReciveEvent?.Invoke(message);

                if (string.IsNullOrEmpty(response)) return;
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            finally
            {
                if (!string.IsNullOrEmpty(response))
                {
                    var routingKey = string.IsNullOrEmpty(props.ReplyTo) ? "" : props.ReplyTo;
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channelEap2Rms.BasicPublish(exchange: eap2RmsExchange,
                                         routingKey: routingKey,
                                         basicProperties: replyProps,
                                         body: responseBytes
                                         );
                }
          
            }
        }

        /// <summary>
        ///  EAP 发送RMS 。RMS返回的消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ea"></param>
        private void RmsRecive_CallbackReceived(object model, BasicDeliverEventArgs ea)
        {
            if (!callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                return;
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            tcs.TrySetResult(response);
        }

        /// <summary>
        /// EAP 发送给RMS 消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> EapSendRmsAsync(string message)
        {
            try
            {
                var timeOut = int.Parse(TimeOutTime);
                var cancellationTokenSource = new CancellationTokenSource(timeOut * 1000);
                var task = await CallToRms(message, cancellationTokenSource.Token);

                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("task be cancel " + ex.Message);
            }
        }
        public Task<string> CallToRms(string message, CancellationToken cancellationToken = default(CancellationToken))
        {

            IBasicProperties props = channelEap2Rms.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = EqpId + "_callback";
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var tcs = new TaskCompletionSource<string>();
            callbackMapper.TryAdd(correlationId, tcs);

            channelEap2Rms.BasicPublish(exchange: eap2RmsExchange,
                                 routingKey: "",
                                 basicProperties: props,
                                 body: messageBytes);

            cancellationToken.Register(() => callbackMapper.TryRemove(correlationId, out _));

            return tcs.Task;
        }


        public void Dispose()
        {
            // closing a connection will also close all channels on it

            if (channelEap2Rms != null)
            {
                channelEap2Rms.Close();
                channelEap2Rms.Abort();
            }

            if (channelRms2Eap != null)
            {
                channelRms2Eap.Close();
                channelRms2Eap.Abort();
            }

            if (connention != null)
            {
                connention.Close();
            }
        }
    }
}

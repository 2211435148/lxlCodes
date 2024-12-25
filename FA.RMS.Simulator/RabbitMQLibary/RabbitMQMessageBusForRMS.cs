using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Configuration;
using System.Text;

namespace RabbitMQLibary
{
    public class RabbitMQMessageBusForRMS
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
        public Func<string, string> OnEapReciveEvent;
        public void InitMqRMS()
        {
            var host = ConfigurationManager.AppSettings["Host"]?.ToString();
            var port = ConfigurationManager.AppSettings["Port"]?.ToString();
            var virtualHost = ConfigurationManager.AppSettings["VirtualHost"]?.ToString();
            var userName = ConfigurationManager.AppSettings["UserName"]?.ToString();
            var password = ConfigurationManager.AppSettings["Password"]?.ToString();
            var rmsQueueName = ConfigurationManager.AppSettings["RmsQueueName"]?.ToString();

            factory = new ConnectionFactory()
            {
                HostName = host,
                Port = int.Parse(port),
                VirtualHost = virtualHost,
                UserName = userName,
                Password = password
            };
            connention = factory.CreateConnection();

            // RMS-->EAPServer  EAP作为服务端 接受RMS请求 定义 RMS发送给EAP的exchange
            //定义RMS发给EAP 的model和 交换机---路由规则由接收端 EAP设置 
            channelRms2Eap = connention.CreateModel();
            channelRms2Eap.ExchangeDeclare(rms2EapExchange, ExchangeType.Direct, false, false);

            channelEap2Rms = connention.CreateModel();
            channelEap2Rms.ExchangeDeclare(eap2RmsExchange, ExchangeType.Direct, false, false);

            CallBackQueueName = channelEap2Rms.QueueDeclare("RMS_RPC_CallBackQueue" + DateTime.Now.ToString("yyyyMMmddHHmmss")).QueueName;
            channelEap2Rms.QueueBind(CallBackQueueName, eap2RmsExchange, CallBackQueueName);

            var consumer = new EventingBasicConsumer(channelEap2Rms);
            consumer.Received += EapRecive_CallbacReceived;

            channelEap2Rms.BasicConsume(consumer: consumer,
                                 queue: CallBackQueueName,
                                 autoAck: true);


            //EAPServer-->RMS EAP作为客户端 请求RMS  定义 eap发送给RMS的exchange 以及返回的queue
            //定义EAP发给RMS 的model和 交换机---路由规则由接收端 RMS设置 
          
            var eapQueueName = channelEap2Rms.QueueDeclare(rmsQueueName, exclusive: false).QueueName;
            channelEap2Rms.QueueBind(eapQueueName, eap2RmsExchange, "");

            var eap2RmsConsumer = new EventingBasicConsumer(channelEap2Rms);
            eap2RmsConsumer.Received += EAPRecive_Received;

            channelEap2Rms.BasicConsume(consumer: eap2RmsConsumer,
                                 queue: eapQueueName,
                                 autoAck: true);
        }

        /// <summary>
        /// 接收EAP 发送的请求。处理后返回
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ea"></param>
        private void EAPRecive_Received(object model, BasicDeliverEventArgs ea)
        {
            string response = string.Empty;

            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            var replyProps = channelRms2Eap.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;
            try
            {
                var message = Encoding.UTF8.GetString(body);
                response = OnEapReciveEvent?.Invoke(message);
                //response = message + "--Reply";
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                channelRms2Eap.BasicPublish(exchange: rms2EapExchange,
                                     routingKey: props.ReplyTo,
                                     basicProperties: replyProps,
                                     body: responseBytes
                                     );
            }
        }

        /// <summary>
        ///  EAP 发送RMS 。RMS返回的消息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ea"></param>
        private void EapRecive_CallbacReceived(object model, BasicDeliverEventArgs ea)
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
        public async Task<string> RmsSendEapAsync(string message, string eqpId)
        {
            try
            {
                var timeOut = int.Parse(TimeOutTime);
                var cancellationTokenSource = new CancellationTokenSource(timeOut * 1000);
                var task = await CallToEap(message, cancellationTokenSource.Token, eqpId);

                return task;
            }
            catch (Exception ex)
            {
                return "task be cancel " + ex.Message;
            }

        }
        public Task<string> CallToEap(string message, CancellationToken cancellationToken = default(CancellationToken), string eqpId = "")
        {

            IBasicProperties props = channelRms2Eap.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = CallBackQueueName;
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var tcs = new TaskCompletionSource<string>();
            callbackMapper.TryAdd(correlationId, tcs);

            channelRms2Eap.BasicPublish(exchange: rms2EapExchange,
                                 routingKey: eqpId,
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

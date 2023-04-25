using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

using HRP.QMessageLib;


namespace HRP.QServiceLib
{
    public class QReceiver<T> : QWorker, IQReceiver where T : IQMessage
    {
        private CancellationToken _token;
        private Func<T, QRecieverResult> _handler;
        private string _exchangeName;
        private string _exchangePrefix;
        private string _queueName;
        private List<Task> _receivers = new List<Task>();
        private List<EventingBasicConsumer> _consumers = new List<EventingBasicConsumer>();

        private int _retryMessageDelay; //default from factory 1 minutes
        private int _errorMessageDelay; //default from factory 3 minutes
        private long _errorMessageRetries; //default 2;

        private bool _receiveFlag = false;
        private bool _pauseReceiveFlag = false;

        public bool PauseReceive { get { return _pauseReceiveFlag; } set { _pauseReceiveFlag = value; } }

        public QReceiver(string hostUri, string exchangeName, string queueName, CancellationToken token, Func<T, QRecieverResult> handler, int retryMessageDelay, int errorMessageDelay, 
                         long errorMessageRetries, string exchangePrefix, string clientProvidedName = "", bool sendJsonMessage = false)
            : base()
        {
            _hostUri = hostUri;
            _exchangeName = exchangeName;
            _queueName = queueName;
            _token = token;
            _handler = handler;
            _retryMessageDelay = retryMessageDelay;
            _errorMessageDelay = errorMessageDelay;
            _errorMessageRetries = errorMessageRetries;
            _exchangePrefix = exchangePrefix;
            _clientProvidedName = clientProvidedName;
            _sendJsonMessage = sendJsonMessage;

            _token.Register(Dispose);
        }

        public void Receive(int p = 1)
        {
            _receiveFlag = true;
            _pauseReceiveFlag = false;

            if (p < 0) //defence  //|| p > Environment.ProcessorCount * 2
            {
                p = Environment.ProcessorCount;
            }

            for (int i = 0; i < p; i++)
            {
                _receivers.Add(Task.Factory.StartNew(() => InnerReceiver()));
            }
        }

        private void InnerReceiver()
        {
            EventingBasicConsumer consumer = null;

            while (_receiveFlag)
            {
                if (!ConsumerIsLive(consumer))
                {
                    try
                    {
                        consumer = Init();

                        if (!_consumers.Contains(consumer))
                        {
                            _consumers.Add(consumer);
                        }
                    }
                    catch (Exception ex)
                    {
                        _eventLog.WriteEntry(string.Format("Init queue {0}-{1} consumer error: {2}", _exchangeName, _queueName, ex.Message), EventLogEntryType.Error);
                    }
                }

                Thread.Sleep(1000);
            }

            DisposeConsumer(consumer);
        }

        private EventingBasicConsumer Init()
        {
            var channel = CreateQueueSet();
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(queue: _queueName, noAck: false, consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                CheckPause();

                IModel ch = ((EventingBasicConsumer)model).Model;

                if (ch == null || ch.IsClosed) return; 

                QRecieverResult res = new QRecieverResult();
                Exception ex = null;
                T obj = Activator.CreateInstance<T>();
                bool objDeserialized = false;

                try
                {
                    try
                    {
                        obj = DeserializeMessage(ea);
                        objDeserialized = true;
                    }
                    catch (Exception ex1)
                    {
                        _eventLog.WriteEntry(string.Format("Receive consumer {0}-{1} can't deserialize message. Exception: {2}", _exchangeName, _queueName, ex1.ToString()), EventLogEntryType.Information);
                        ex = ex1;
                        res.IsOk = false;
                        res.ErrorMessage = "Can't deserialize message";
                        res.SendToError = true;
                    }

                    if(objDeserialized)
                    {
                        try
                        {
                            res = _handler(obj);
                        }
                        catch (Exception ex1)
                        {
                            _eventLog.WriteEntry(string.Format("Receive consumer {0}-{1} handler function caused an exception. Will retry. Exception: {2}", _exchangeName, _queueName, ex1.ToString()), EventLogEntryType.Information);
                            ex = ex1;
                            res.IsOk = false;
                        }
                    }

                    if (!res.IsOk)
                    {
                        DateTime dt = ea.BasicProperties.Timestamp.ToDateTime();

                        // First attempt is not retry
                        long retryAttemptsDone = GetXDeathCount(ea);

                        bool retryTimeExpired = (DateTime.Now - dt).TotalMinutes > _errorMessageDelay;
                        bool retryLimitExceeded = retryAttemptsDone >= _errorMessageRetries;
                        bool retriesExceeded = retryTimeExpired && retryLimitExceeded;

                        _eventLog.WriteEntry(string.Format("Receive consumer {0}-{1} retry status: Duration: {2}, Attempt {3}.", _exchangeName, _queueName, (DateTime.Now - dt).Minutes, retryAttemptsDone), EventLogEntryType.Information);

                        if (typeof(T) != typeof(QMessageError) && (res.SendToError || retriesExceeded))
                        {
                            res.IsOk = SendErrorMessage(obj, ea.Exchange, res.ErrorMessage, ex, dt);
                        }
                    }

                    if(ch != null && ch.IsOpen)
                    {
                        if (res.IsOk)
                        {
                            ch.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                        else
                        {
                            ch.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        }
                    }
                    else //for debug
                    { 

                    }
                }
                catch (Exception ex2)
                {
                    try //need for dispose receiver
                    {
                        _eventLog.WriteEntry(string.Format("Receive consumer {0}-{1} error: {2}", _exchangeName, _queueName, ex2.Message), EventLogEntryType.Error);
                    }
                    catch (Exception ex3)
                    {
                    }
                }
            };

            return consumer;
        }

        private void CheckPause()
        {
            var maxPauseMinutes = 15;

            var t = Task.Run(() => {

                var dt = DateTime.Now;

                while (_pauseReceiveFlag)
                {
                    Thread.Sleep(1000);

                    if ((DateTime.Now - dt).TotalMinutes > maxPauseMinutes) //pause can't be longer
                    {
                        _pauseReceiveFlag = false;
                        throw new Exception(string.Format("Receiver {0} paused more then {1} minutes", _queueName, maxPauseMinutes));
                    }
                }
            });

            t.Wait();
        }

        private static long GetXDeathCount(BasicDeliverEventArgs ea)
        {
            long count = 0;
            object header;
            if (ea.BasicProperties.Headers.TryGetValue("x-death", out header))
            {
                var headerList = header as IList<object>;
                if (headerList != null && headerList.Count > 0)
                {
                    var deadLetterHeaders = headerList.First() as IDictionary<string, object>;

                    object deadLetterCount;
                    if (deadLetterHeaders != null && deadLetterHeaders.TryGetValue("count", out deadLetterCount))
                    {
                        count = (long) deadLetterCount;
                    }   
                }
            }
            return count;
        }

        private IModel CreateQueueSet()
        {
            IModel channel = CreateChannel(_exchangeName);

            //--------------------------------------- Retry Exchanges ---------------------------------------

            var retryDelayExchangeName = _queueName + "-RetryDelay";
            var retryDelayChannel = CreateChannel(retryDelayExchangeName);

            var retryBackExchangeName = _queueName + "-RetryBack";
            var retryBackChannel = CreateChannel(retryBackExchangeName);  

            //--------------------------------------- Queues ---------------------------------------

            var queueArgsDelay = new Dictionary<string, object> { { "x-dead-letter-exchange", retryDelayExchangeName } }; 
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: queueArgsDelay);
            channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: "");
            retryBackChannel.QueueBind(queue: _queueName, exchange: retryBackExchangeName, routingKey: "");

            var retryQueueName = _queueName + "-Delay";
            var queueArgsBack = new Dictionary<string, object> { { "x-dead-letter-exchange", retryBackExchangeName }, { "x-message-ttl", _retryMessageDelay * 60000 } };
            retryDelayChannel.QueueDeclare(queue: retryQueueName, durable: true, exclusive: false, autoDelete: false, arguments: queueArgsBack);
            retryDelayChannel.QueueBind(queue: retryQueueName, exchange: retryDelayExchangeName, routingKey: "");

            return channel;
        }

        private IModel CreateChannel(string exchangeName)
        {
            var conn = GetConnection();

            IModel channel = conn.CreateModel();
            channel.BasicQos(prefetchSize: 0, prefetchCount: 20, global: false); //dispatches messages on workers load state 
            channel.ExchangeDeclare(exchange: exchangeName, type: "fanout", durable: true);

            return channel;
        }

        private bool SendErrorMessage(T msg, string backExchange, string errorText, Exception ex, DateTime startedOn)
        {
            bool res = false;
            QMessageError errMsg = CreateErrorMessage(msg, backExchange, errorText, ex, startedOn);

            try
            {
                string routeName = QWorkerFactory.GetExchangeName(typeof(QMessageError), _exchangePrefix);

                using (QPublisher<QMessageError> pb = new QPublisher<QMessageError>(_hostUri, routeName, _token, _clientProvidedName, _sendJsonMessage))
                {
                    pb.Publish(errMsg);
                    res = true;
                }
            }
            catch (Exception ex1)
            {
                _eventLog.WriteEntry(string.Format("Send QError message {0}-{1} error: {2}", _exchangeName, _queueName, ex1.Message), EventLogEntryType.Error);
            }

            return res;
        }

        private QMessageError CreateErrorMessage(T msg, string backExchange, string errorText, Exception ex, DateTime startedOn)
        {
            try
            {
                string errTxt = errorText;

                if (string.IsNullOrWhiteSpace(errTxt))
                {
                    errTxt = (ex != null) ? ex.Message : "Unknown bad result from QService message processing";
                }

                string errorJson = ObjectToJsonString(ex);
                string messageJson = ObjectToJsonString(msg);

                return new QMessageError()
                {
                    MessageJson = messageJson,
                    MessageType = typeof(T),
                    BackExchange = backExchange,
                    ErrorMessage = errTxt,
                    StartedOn = startedOn,
                    LoggedOn = DateTime.Now,
                    ErrorJson = errorJson
                };
            }
            catch (Exception ex1)
            {

                throw;
            }
        }

        private string ObjectToJsonString(object message)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented
            };

            try
            {
                return JsonConvert.SerializeObject(message, settings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex, settings);
            }
        }

        // Check using real serialization because typeof(T).IsSerializable checks only top class
        private bool isExceptionSerializable(Exception ex)
        {
            bool serializable;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, ex);
                    serializable = true;
                }
            }
            catch (Exception)
            {
                serializable = false;
            }
            return serializable;
        }

        private bool IsJsonMessage(string mimeContentType)
        {
            return QMessageSerialization.IsJsonMessage(mimeContentType);
        }

        private T DeserializeMessage(BasicDeliverEventArgs ea)
        {
            if(IsJsonMessage(ea.BasicProperties.ContentType))
            {
                return JsonByteArrayToObject(ea.Body);
            }
            else
            {
                return ByteArrayToObject(ea.Body);
            }
        }

        private T ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (T)obj;
            }
        }

        private T JsonByteArrayToObject(byte[] arrBytes)
        {
            var str = System.Text.Encoding.UTF8.GetString(arrBytes);

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            };

            return JsonConvert.DeserializeObject<T>(str, settings);
        }


        private void DisposeConsumer(EventingBasicConsumer consumer)
        {
            if (consumer != null && consumer.Model != null)
            {
                try //can be second dispose
                {
                    consumer.Model.Close(200, "Goodbye!");
                    consumer.Model.Dispose();
                }
                catch(Exception ex) 
                { }
            }
        }

        private bool ConsumerIsLive(EventingBasicConsumer consumer)
        {
            if(consumer != null && consumer.Model != null && consumer.Model.IsOpen)
            {
                return true;
            }

            DisposeConsumer(consumer);

            return false;
        }

        public void StopReceive()
        {
            _receiveFlag = false;
            _consumers.ForEach(x => DisposeConsumer(x));
            _consumers.Clear();
            Task.WaitAll(_receivers.ToArray(), 2000);
        }


        protected override void Dispose(bool disposing)
        {
            if (_isClosed) return; //prevent repeated dispose

            if (disposing)
            {
                StopReceive();
            }

            base.Dispose(disposing);
        }


    }
}

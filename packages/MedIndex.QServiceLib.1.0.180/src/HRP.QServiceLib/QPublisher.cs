using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mime;

using RabbitMQ.Client;
using Newtonsoft.Json;

using HRP.QMessageLib;


namespace HRP.QServiceLib
{
    public class QPublisher<T> : QWorker, IQPublisher where T : IQMessage
    {
        private CancellationToken _token;
        private string _exchangeName = "";
        private IModel _channel = null;


        public QPublisher(string hostUri, string exchangeName, CancellationToken token, string clientProvidedName = "", bool sendJsonMessage = false) : base()
        {
            _hostUri = hostUri;
            _exchangeName = exchangeName;
            _token = token;
            _clientProvidedName = clientProvidedName;
            _sendJsonMessage = sendJsonMessage;
        }

        public QPublisher(string hostUri, string exchangeName, CancellationToken token) : base()
        {
            _hostUri = hostUri;
            _exchangeName = exchangeName;
            _token = token;
        }

        public IQMessage Publish(IQMessage message)
        {
            _channel = Init(_channel);

            return InnerPublish((T)message, _channel);
        }

        public List<IQMessage> Publish(List<IQMessage> messages, int p = 1)
        {
            if (p < 0 || p > Environment.ProcessorCount * 2) //defence
            {
                p = Environment.ProcessorCount;
            }

            ConcurrentQueue<IQMessage> q = new ConcurrentQueue<IQMessage>(messages);
            ConcurrentBag<IQMessage> bag = new ConcurrentBag<IQMessage>();

            Parallel.For(0, p, (i) => {

                IQMessage msg;
                IModel channel = null;

                while (q.TryDequeue(out msg))
                {
                    channel = Init(channel);

                    try
                    {
                        //throw new Exception("test");
                        msg = InnerPublish((T)msg, channel);                       
                    }
                    catch(Exception ex)
                    {   
                        //Exception already logged by InnerPublish
                        bag.Add(msg);
                    }

                    if (_token.IsCancellationRequested)
                    {
                        while (q.TryDequeue(out msg))
                        {
                            bag.Add(msg);
                        }

                        break;
                    }
                }

                ChannelDispose(channel);
            });

            return bag.ToList();
        }

        private T InnerPublish(T message, IModel channel)
        {
            try
            {
                var basicProp = channel.CreateBasicProperties();
                basicProp.Persistent = true;
                basicProp.Headers = new Dictionary<string, object>();
                basicProp.Headers.Add("MessageId", channel.NextPublishSeqNo.ToString());

                basicProp.Type = typeof(T).AssemblyQualifiedName;
                basicProp.Timestamp = DateTime.Now.ToAmqpTimestamp();

                var contType = _sendJsonMessage ? QMessageSerialization.jsonContentType : QMessageSerialization.binaryContentType;

                basicProp.ContentType = contType.MediaType;
                basicProp.ContentEncoding = contType.CharSet;


                var body = _sendJsonMessage ? MessageToJsonBytes(message) : MessageToBytes(message); 


                channel.BasicPublish(exchange: _exchangeName, routingKey: "", basicProperties: basicProp, body: body);
                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 0, 10)); //wait for 10 seconds
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry(string.Format("Send message to exchange {0} error: {1}", _exchangeName, ex.Message), EventLogEntryType.Error);
                throw;
            }

            return message;
        }

        private IModel Init(IModel channel = null)
        {
            if (channel == null || channel.IsClosed)
            {
                ChannelDispose(channel);

                var conn = GetConnection();

                channel = conn.CreateModel();

                channel.ConfirmSelect();
                channel.ExchangeDeclare(exchange: _exchangeName, type: "fanout", durable: true);
            }

            return channel;
        }

        private byte[] MessageToBytes(object message)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, message);
                return ms.ToArray();
            }
        }

        private byte[] MessageToJsonBytes(object message)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                                                                             TypeNameHandling = TypeNameHandling.Objects,
                                                                             Formatting = Formatting.Indented };

            string str = JsonConvert.SerializeObject(message, settings);

            return Encoding.UTF8.GetBytes(str);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void ChannelDispose(IModel channel)
        {
            if (channel != null && channel.IsOpen)
            {
                channel.Close();
                channel.Dispose();
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (_isClosed) return; //prevent repeated dispose

            if (disposing)
            {
                ChannelDispose(_channel);
            }

            base.Dispose(disposing);
        }
    }
}

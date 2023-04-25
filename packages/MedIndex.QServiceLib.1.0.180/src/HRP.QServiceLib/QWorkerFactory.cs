using System;
using System.Threading;

using HRP.QMessageLib;

namespace HRP.QServiceLib
{
    public class QWorkerFactory
    {
        private string _hostUri = "";
        private string _serviceName = "";
        private string _clientProvidedName = "";
        private bool _sendJsonMessage = false;

        private string _exchangePrefix;
        private CancellationToken _token;

        private int _retryMessageDelay; //minutes
        private int _errorMessageDelay; //minutes
        private int _errorMessageRetries;


        public QWorkerFactory(string hostUri, string serviceName, CancellationToken token, int retryMessageDelay = 1, int errorMessageDelay = 3, int errorMessageRetries = 2, string exchangePrefix = "", 
                              string clientProvidedName = "", bool sendJsonMessage = false)
        {
            _hostUri = hostUri;
            _serviceName = serviceName;
            _token = token;
            _retryMessageDelay = retryMessageDelay;
            _errorMessageDelay = errorMessageDelay;
            _errorMessageRetries = errorMessageRetries;
            _exchangePrefix = exchangePrefix != null ? exchangePrefix.Trim() : String.Empty;
            _clientProvidedName = string.IsNullOrWhiteSpace(clientProvidedName) ? serviceName : clientProvidedName;
            _sendJsonMessage = sendJsonMessage;
        }

        public QReceiver<T> CreateQReciever<T>(Func<T, QRecieverResult> handler) where T : IQMessage
        {
            string routeName = GetExchangeName(typeof(T), _exchangePrefix);
            string queueName = GetQueueName(typeof(T), _serviceName);

            QReceiver<T> rc = new QReceiver<T>(_hostUri, routeName, queueName, _token, handler, _retryMessageDelay, _errorMessageDelay, _errorMessageRetries,
                                               _exchangePrefix, _clientProvidedName, _sendJsonMessage);
            return rc;
        }

        public QPublisher<T> CreateQPublisher<T>() where T : IQMessage
        {
            string routeName = GetExchangeName(typeof(T), _exchangePrefix);

            QPublisher<T> pb = new QPublisher<T>(_hostUri, routeName, _token, _clientProvidedName + "-Publisher", _sendJsonMessage);

            return pb;
        }

        public string GetQueueName(Type t, string serviceName)
        {
            string str = string.Format("{0}-{1}", serviceName, t.Name);
            return NormalizeName(str, _exchangePrefix);
        }

        public static string GetExchangeName(Type t, string exchangePrefix)
        {
            return NormalizeName(t.Name, exchangePrefix);
        }

        public static string NormalizeName(string name, string exchangePrefix)
        {
            string prefixed_name = String.Empty;
            if (!String.IsNullOrWhiteSpace(exchangePrefix))
            {
                prefixed_name = exchangePrefix + "-";
            }
            prefixed_name += name;
            return CleanName(prefixed_name);
        }

        public static string CleanName(string name)
        {
            return name.Replace("<", "").Replace(">", "").Replace(".", "");
        }
         
    }
}

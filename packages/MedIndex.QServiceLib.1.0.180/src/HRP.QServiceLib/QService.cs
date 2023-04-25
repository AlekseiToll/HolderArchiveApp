using System;
using System.Timers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


using HRP.QMessageLib;

namespace HRP.QServiceLib
{
    public class QService : QLog, IShelfService, IDisposable
    {
        private int _parallelIndex = 1;

        protected string _hostUri = "amqp://localhost";  //amqp://user:pass@host:10000/vhost
        protected string _clientProvidedName = "";
        protected bool _sendJsonMessage = false;
        protected string _serviceName = "";
        protected string _exchangePrefix = "";
        protected int _retryMessageDelay = 1; //minutes
        protected int _errorMessageDelay = 3; //minutes
        protected int _errorMessageRetries = 2;
        protected bool _isRunning = false;
        protected QWorkerFactory _qFactory;

        private CancellationTokenSource _tokenSource;

        protected TimeSpan _reInitPeriod;
        protected TimeSpan _schedPeriod;

        private System.Timers.Timer _reInitTimer;
        private System.Timers.Timer _schedTimer;

        public string HostUri { set { _hostUri = value; } }
        public int ParallelIndex { set { _parallelIndex = value; } }
        public TimeSpan ReInitPeriod { set { _reInitPeriod = value; } }
        public TimeSpan SchedPeriod { set { _schedPeriod = value; } }
        public string ExchangePrefix { set { _exchangePrefix = value; } }

        public int retryMessageDelay { get { return _retryMessageDelay; } set { _retryMessageDelay = value; } }
        public int errorMessageDelay { get { return _errorMessageDelay; } set { _errorMessageDelay = value; } }
        public int errorMessageRetries { get { return _errorMessageRetries; } set { _errorMessageRetries = value; } }
        public string clientProvidedName { get { return _clientProvidedName; } set { _clientProvidedName = value; } }
        public bool SendJsonMessage { get { return _sendJsonMessage; } set { _sendJsonMessage = value; } }


        public bool IsRunning { get { return _isRunning; } }

        private ConcurrentDictionary<Type, IQPublisher> _publishers = new ConcurrentDictionary<Type, IQPublisher>();
        private ConcurrentDictionary<Type, Object> _publishersLocks = new ConcurrentDictionary<Type, Object>();

        private List<IQReceiver> _recievers = new List<IQReceiver>();

        public QService() : base()
        {
            _tokenSource = new CancellationTokenSource();
            _serviceName = QWorkerFactory.CleanName(this.GetType().Name);
        }

        public virtual void Initialize()
        {
            //RecieveMessage<T>(Func<T, bool> handler) where T : IQMessage
        }

        public virtual void Start()
        {
            _qFactory = new QWorkerFactory(_hostUri, _serviceName, _tokenSource.Token, retryMessageDelay, errorMessageDelay, errorMessageRetries, _exchangePrefix, clientProvidedName, _sendJsonMessage);

            Initialize();
            StartListenMessages();

            _reInitTimer = SetTimer(_reInitPeriod, ReInitTimer_Elapsed);
            _schedTimer = SetTimer(_schedPeriod, Sched_Elapsed);

            _eventLog.WriteEntry(string.Format("{0} OnStart", _serviceName), EventLogEntryType.SuccessAudit);
        }

        public virtual void Stop()
        {
            _eventLog.WriteEntry(string.Format("{0} OnStop", _serviceName), EventLogEntryType.SuccessAudit);
            _tokenSource.Cancel();

            Dispose();
        }


        protected void StartListenMessages()
        {
            _recievers.ForEach(x => x.Receive(_parallelIndex));
            _isRunning = true;
        }

        protected void StopListenMessages()
        {
            _isRunning = false;
            _recievers.ForEach(x => x.StopReceive());
        }

        public void RecieveMessage<T>(Func<T, QRecieverResult> handler) where T : IQMessage
        {
            IQReceiver rc = _qFactory.CreateQReciever<T>(handler) as IQReceiver;

            _recievers.Add(rc);
        }

        public T SendMessage<T>(T message) where T : IQMessage
        {
            IQPublisher pb = GetPublisher<T>() as IQPublisher;
            Type t = typeof(T);
            Object publishLock = _publishersLocks.GetOrAdd(t, type => new Object());
            lock (publishLock)
            {
                pb.Publish(message);
            }

            return message;
        }

        public List<IQMessage> SendMessageList<T>(List<T> messages) where T : IQMessage
        {
            List<IQMessage> ret = new List<IQMessage>();
            IQPublisher pb = GetPublisher<T>() as IQPublisher;
            Type t = typeof(T);
            Object publishLock = _publishersLocks.GetOrAdd(t, type => new Object());
            lock (publishLock)
            {
                ret = pb.Publish(messages.Select(x => x as IQMessage).ToList(), _parallelIndex);
            }

            return ret;
        }

        private IQPublisher GetPublisher<T>() where T : IQMessage
        {
            Type t = typeof(T);

            IQPublisher pb = _publishers.GetOrAdd(t, type => _qFactory.CreateQPublisher<T>() as IQPublisher);

            return pb;
        }


        private System.Timers.Timer SetTimer(TimeSpan period, ElapsedEventHandler handler)
        {
            if (period.TotalMilliseconds == 0) return null;

            var timer = new System.Timers.Timer(period.TotalMilliseconds);
            timer.Elapsed += handler;
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Start();

            return timer;
        }

        private void DisposeTimer(System.Timers.Timer timer)
        {
            if (timer != null)
            {
                try
                {
                    timer.Stop();
                    timer.Close();
                    timer.Dispose();
                    timer = null;
                }
                catch (Exception ex) // for debug
                { }
            }
        }

        private void ReInitTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
           try
            {
                _eventLog.WriteEntry(string.Format("Start ReInit {0}", _serviceName), EventLogEntryType.SuccessAudit);

                _recievers.ForEach(x => { if (x != null) x.PauseReceive = true; });

                Thread.Sleep(10000);

                ReInit();

                _recievers.ForEach(x => { if (x != null) x.PauseReceive = false; });

                DisposeTimer(_reInitTimer);
                this._reInitTimer = SetTimer(this._reInitPeriod, ReInitTimer_Elapsed);

                GC.Collect();

                _eventLog.WriteEntry(string.Format("Finish ReInit {0}", _serviceName), EventLogEntryType.SuccessAudit);
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry(string.Format("Error ReInit {0}, will retry in 5 minutes. Error: {1}", _serviceName, ex.Message), EventLogEntryType.Error);

                DisposeTimer(_reInitTimer);
                this._reInitTimer = SetTimer(new TimeSpan(0, 5, 0), ReInitTimer_Elapsed);
            }
        }

        private void Sched_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _eventLog.WriteEntry(string.Format("Start Sched {0}", _serviceName), EventLogEntryType.SuccessAudit);

                Sched();

                DisposeTimer(_schedTimer);
                this._schedTimer = SetTimer(this._schedPeriod, Sched_Elapsed);

                _eventLog.WriteEntry(string.Format("Finish Sched {0}", _serviceName), EventLogEntryType.SuccessAudit);
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry(string.Format("Error Sched {0}, will retry in 5 minutes. Error: {1}", _serviceName, ex.Message), EventLogEntryType.Error);

                DisposeTimer(_schedTimer);
                this._schedTimer = SetTimer(new TimeSpan(0, 5, 0), Sched_Elapsed);
            }
        }

        protected virtual void ReInit()
        {
        }

        protected virtual void Sched()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (_isClosed) return;    

            if (disposing)
            {
                if (_reInitTimer != null && _reInitTimer.Enabled)
                {
                    _reInitTimer.Close();
                    _reInitTimer.Dispose();
                }

                if(_schedTimer !=null && _schedTimer.Enabled)
                {
                    _schedTimer.Close();
                    _schedTimer.Dispose();
                }

                _recievers.ForEach(x => { if (x != null) x.Dispose(); });
                _recievers.Clear();

                _publishers.Values.ToList().ForEach(x => { if (x != null) x.Dispose(); });
                _publishers.Clear();

                _tokenSource.Dispose();
                _isRunning = false;
            }

            base.Dispose(disposing);
        }
    }
}

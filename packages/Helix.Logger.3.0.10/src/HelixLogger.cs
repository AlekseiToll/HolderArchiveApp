using System;
using System.Text;
using NLog;
using Newtonsoft.Json;


namespace Helix.Logger
{
    

    public class HelixLogger : IHelixLogger
    {
        private readonly string loggerName;
        private readonly NLog.Logger logger;
        private LogEventInfo logEvent;

        public HelixLogger(string loggerName)
        {
            this.loggerName = loggerName;
            //if (String.IsNullOrWhiteSpace(this.loggerName))
            //    this.loggerName = "HelixLogger";


            try
            {
                this.logger = LogManager.GetLogger(this.loggerName);
            }
            catch
            {
                this.logger = NLog.LogManager.CreateNullLogger();
                if (NLog.LogManager.ThrowExceptions)
                    throw;
            }
        }

        public void Log(LogLevel logLevel, string message, string entityId = null, Guid? actionGuid = null, double? executeTime = null, string serializedData = "")
        {
            try
            {
                logEvent = new LogEventInfo(logLevel, this.loggerName, message);
                logEvent.Properties["actionGuid"] = actionGuid.HasValue ? actionGuid.ToString() : Guid.NewGuid().ToString();
                logEvent.Properties["entityId"] = entityId;
                logEvent.Properties["executeTime"] = executeTime;
                logEvent.Properties["serializedData"] = serializedData;
                //logger.Log(typeof(HelixLogger), logEvent);
                logger.Log(logEvent);
            }
            catch (Exception ex)
            {
                WriteToConsole(ex);
                if (NLog.LogManager.ThrowExceptions)
                    throw;
            }
        }
        public void Log(LogLevel logLevel, Exception exception, string entityId = null, Guid? actionGuid = null, double? executeTime = null)
        {
            if (exception == null)
                return;
            try
            {
                logEvent = new LogEventInfo(logLevel, this.loggerName, exception.Message);
                logEvent.Exception = exception;
                logEvent.Properties["actionGuid"] = actionGuid.HasValue ? actionGuid.ToString() : Guid.NewGuid().ToString();
                logEvent.Properties["entityId"] = entityId;
                logEvent.Properties["executeTime"] = executeTime;
                logEvent.Properties["exStackTrace"] = ValidateString8000(exception.StackTrace);
                logEvent.Properties["fullExeptionMessage"] = GetFullMessage(exception);
                //logger.Log(typeof(HelixLogger), logEvent);
                logger.Log(logEvent);
            }
            catch(Exception ex)
            {
                WriteToConsole(ex);
                if (NLog.LogManager.ThrowExceptions)
                    throw;
            }
        }


        #region Trace
        #region message
        public void Trace(string message, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, message, null, Guid.NewGuid(), executeTime);
        }
        public void Trace(string message, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, message, null, actionGuid, executeTime);
        }
        public void Trace(string message, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, message, entityId, Guid.NewGuid(), executeTime);
        }
        public void Trace(string message, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, message, entityId, actionGuid, executeTime);
        }
        public void Trace(string message, string entityId, Guid actionGuid, string serializedData, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, message, entityId, actionGuid, executeTime, serializedData);
        }
        public void Trace(string message, string entityId, Guid actionGuid, object objectToSerialize, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, message, entityId, actionGuid, executeTime, Serialize(objectToSerialize));
        }
        #endregion


        #region exception
        public void Trace(Exception exception, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, exception, null, Guid.NewGuid(), executeTime);
        }
        public void Trace(Exception exception, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, exception, null, actionGuid, executeTime);
        }
        public void Trace(Exception exception, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, exception, entityId, Guid.NewGuid(), executeTime);
        }
        public void Trace(Exception exception, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Trace, exception, entityId, actionGuid, executeTime);
        }
        #endregion
        #endregion


        #region Debug
        #region message
        public void Debug(string message, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, message, null, Guid.NewGuid(), executeTime);
        }
        public void Debug(string message, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, message, null, actionGuid, executeTime);
        }
        public void Debug(string message, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, message, entityId, Guid.NewGuid(), executeTime);
        }
        public void Debug(string message, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, message, entityId, actionGuid, executeTime);
        }
        public void Debug(string message, string entityId, Guid actionGuid, string serializedData, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, message, entityId, actionGuid, executeTime, serializedData);
        }
        public void Debug(string message, string entityId, Guid actionGuid, object objectToSerialize, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, message, entityId, actionGuid, executeTime, Serialize(objectToSerialize));
        }
        #endregion


        #region exception
        public void Debug(Exception exception, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, exception, null, Guid.NewGuid(), executeTime);
        }
        public void Debug(Exception exception, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, exception, null, actionGuid, executeTime);
        }
        public void Debug(Exception exception, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, exception, entityId, Guid.NewGuid(), executeTime);
        }
        public void Debug(Exception exception, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Debug, exception, entityId, actionGuid, executeTime);
        }
        #endregion
        #endregion


        #region Info
        #region message
        public void Info(string message, double? executeTime = null)
        {
            this.Log(LogLevel.Info, message, null, Guid.NewGuid(), executeTime);
        }
        public void Info(string message, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Info, message, null, actionGuid, executeTime);
        }
        public void Info(string message, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Info, message, entityId, Guid.NewGuid(), executeTime);
        }
        public void Info(string message, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Info, message, entityId, actionGuid, executeTime);
        }
        public void Info(string message, string entityId, Guid actionGuid, string serializedData, double? executeTime = null)
        {
            this.Log(LogLevel.Info, message, entityId, actionGuid, executeTime, serializedData);
        }
        public void Info(string message, string entityId, Guid actionGuid, object objectToSerialize, double? executeTime = null)
        {
            this.Log(LogLevel.Info, message, entityId, actionGuid, executeTime, Serialize(objectToSerialize));
        }
        #endregion


        #region exception
        public void Info(Exception exception, double? executeTime = null)
        {
            this.Log(LogLevel.Info, exception, null, Guid.NewGuid(), executeTime);
        }
        public void Info(Exception exception, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Info, exception, null, actionGuid, executeTime);
        }
        public void Info(Exception exception, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Info, exception, entityId, Guid.NewGuid(), executeTime);
        }
        public void Info(Exception exception, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Info, exception, entityId, actionGuid, executeTime);
        }
        #endregion
        #endregion


        #region Warn
        #region message
        public void Warn(string message, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, message, null, Guid.NewGuid(), executeTime);
        }
        public void Warn(string message, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, message, null, actionGuid, executeTime);
        }
        public void Warn(string message, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, message, entityId, Guid.NewGuid(), executeTime);
        }
        public void Warn(string message, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, message, entityId, actionGuid, executeTime);
        }
        public void Warn(string message, string entityId, Guid actionGuid, string serializedData, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, message, entityId, actionGuid, executeTime, serializedData);
        }
        public void Warn(string message, string entityId, Guid actionGuid, object objectToSerialize, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, message, entityId, actionGuid, executeTime, Serialize(objectToSerialize));
        }
        #endregion


        #region exception
        public void Warn(Exception exception, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, exception, null, Guid.NewGuid(), executeTime);
        }
        public void Warn(Exception exception, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, exception, null, actionGuid, executeTime);
        }
        public void Warn(Exception exception, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, exception, entityId, Guid.NewGuid(), executeTime);
        }
        public void Warn(Exception exception, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Warn, exception, entityId, actionGuid, executeTime);
        }
        #endregion
        #endregion


        #region Error
        #region message
        public void Error(string message, double? executeTime = null)
        {
            this.Log(LogLevel.Error, message, null, Guid.NewGuid(), executeTime);
        }
        public void Error(string message, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Error, message, null, actionGuid, executeTime);
        }
        public void Error(string message, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Error, message, entityId, Guid.NewGuid(), executeTime);
        }
        public void Error(string message, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Error, message, entityId, actionGuid, executeTime);
        }
        public void Error(string message, string entityId, Guid actionGuid, string serializedData, double? executeTime = null)
        {
            this.Log(LogLevel.Error, message, entityId, actionGuid, executeTime, serializedData);
        }
        public void Error(string message, string entityId, Guid actionGuid, object objectToSerialize, double? executeTime = null)
        {
            this.Log(LogLevel.Error, message, entityId, actionGuid, executeTime, Serialize(objectToSerialize));
        }
        #endregion


        #region exception
        public void Error(Exception exception, double? executeTime = null)
        {
            this.Log(LogLevel.Error, exception, null, Guid.NewGuid(), executeTime);
        }
        public void Error(Exception exception, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Error, exception, null, actionGuid, executeTime);
        }
        public void Error(Exception exception, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Error, exception, entityId, Guid.NewGuid(), executeTime);
        }
        public void Error(Exception exception, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Error, exception, entityId, actionGuid, executeTime);
        }
        #endregion
        #endregion


        #region Fatal
        #region message
        public void Fatal(string message, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, message, null, Guid.NewGuid(), executeTime);
        }
        public void Fatal(string message, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, message, null, actionGuid, executeTime);
        }
        public void Fatal(string message, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, message, entityId, Guid.NewGuid(), executeTime);
        }
        public void Fatal(string message, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, message, entityId, actionGuid, executeTime);
        }
        public void Fatal(string message, string entityId, Guid actionGuid, string serializedData, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, message, entityId, actionGuid, executeTime, serializedData);
        }
        public void Fatal(string message, string entityId, Guid actionGuid, object objectToSerialize, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, message, entityId, actionGuid, executeTime, Serialize(objectToSerialize));
        }
        #endregion


        #region exception
        public void Fatal(Exception exception, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, exception, null, Guid.NewGuid(), executeTime);
        }
        public void Fatal(Exception exception, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, exception, null, actionGuid, executeTime);
        }
        public void Fatal(Exception exception, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, exception, entityId, Guid.NewGuid(), executeTime);
        }
        public void Fatal(Exception exception, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Fatal, exception, entityId, actionGuid, executeTime);
        }
        #endregion
        #endregion


        #region Off
        #region message
        public void Off(string message, double? executeTime = null)
        {
            this.Log(LogLevel.Off, message, null, Guid.NewGuid(), executeTime);
        }
        public void Off(string message, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Off, message, null, actionGuid, executeTime);
        }
        public void Off(string message, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Off, message, entityId, Guid.NewGuid(), executeTime);
        }
        public void Off(string message, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Off, message, entityId, actionGuid, executeTime);
        }
        public void Off(string message, string entityId, Guid actionGuid, string serializedData, double? executeTime = null)
        {
            this.Log(LogLevel.Off, message, entityId, actionGuid, executeTime, serializedData);
        }
        public void Off(string message, string entityId, Guid actionGuid, object objectToSerialize, double? executeTime = null)
        {
            this.Log(LogLevel.Off, message, entityId, actionGuid, executeTime, Serialize(objectToSerialize));
        }
        #endregion


        #region exception
        public void Off(Exception exception, double? executeTime = null)
        {
            this.Log(LogLevel.Off, exception, null, Guid.NewGuid(), executeTime);
        }
        public void Off(Exception exception, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Off, exception, null, actionGuid, executeTime);
        }
        public void Off(Exception exception, string entityId, double? executeTime = null)
        {
            this.Log(LogLevel.Off, exception, entityId, Guid.NewGuid(), executeTime);
        }
        public void Off(Exception exception, string entityId, Guid actionGuid, double? executeTime = null)
        {
            this.Log(LogLevel.Off, exception, entityId, actionGuid, executeTime);
        }
        #endregion
        #endregion

        private string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            catch
            {
                return "Ошибка сериализации";
            }
        }

        private string GetFullMessage(Exception ex)
        {
            StringBuilder rez = new StringBuilder();
            //var validationEx = ex as DbEntityValidationException;
            //if (validationEx != null)
            //{
            //    rez.Append("Ошибки валидации:\r\n");
            //    foreach (var validationErrors in validationEx.EntityValidationErrors)
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //            rez.Append("Property: " + validationError.PropertyName + "\rError: " + validationError.ErrorMessage + "\r\n");
            //    return rez.ToString();
            //}
            var currEx = ex;
            while (currEx != null)
            {
                rez.Append("\r\n").Append(currEx.Message);
                currEx = currEx.InnerException;
            }
            if (rez.Length < 2)
                return "";

            if (rez.Length > 7999)
                return rez.ToString(0, 7999);

            return rez.ToString(2, rez.Length - 2);
        }
        private string ValidateString8000(string input)
        {
            if (String.IsNullOrEmpty(input))
                return string.Empty;
            if (input.Length > 7999)
                return input.Substring(0, 7999);
            else
                return input;
        }

        private void WriteToConsole(Exception ex)
        {
            try
            {
                if (ex == null)
                    return;
                Console.WriteLine(ex.Message);
                Console.WriteLine("");
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine("");
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine(ex.InnerException.InnerException.Message);
                        Console.WriteLine("");
                    }
                }
            }
            catch { }
        }
    }
}

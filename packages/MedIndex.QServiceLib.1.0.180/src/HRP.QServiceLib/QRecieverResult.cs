namespace HRP.QServiceLib
{
    public class QRecieverResult
    {
        public bool IsOk { get; set; }
        public bool SendToError { get; set; }
        public string ErrorMessage { get; set; }

        public QRecieverResult() { }

        public QRecieverResult(bool isOk, string errorMessage = null, bool sendToError = false)
        {
            IsOk = isOk;
            SendToError = sendToError;
            ErrorMessage = errorMessage;
        }
    }
}

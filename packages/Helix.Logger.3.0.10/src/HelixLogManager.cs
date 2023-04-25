using System.Diagnostics;

namespace Helix.Logger
{
    public static class HelixLogManager
    {
        public static HelixLogger GetCurrentClassLogger([CallerMemberName]string memberName = "")
        {
            var loggerName = "";

            try
            {
                loggerName = new StackTrace().GetFrame(1).GetMethod().ReflectedType.FullName;
            }
            catch
            {
                if (NLog.LogManager.ThrowExceptions)
                    throw;
            }

            if (string.IsNullOrEmpty(loggerName))
                loggerName = "HelixLogger";

            return new HelixLogger(loggerName);
        }
    }
}

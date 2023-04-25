using System;
using System.Web.Script.Serialization;
using Helix.Logger;

namespace HolderArchiveApp.Service
{
    public class ServiceMethods
    {
	    public static string ObjectToJson(object obj)
	    {
		    try
		    {
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				return serializer.Serialize(obj);
			}
		    catch (Exception)
		    {
			    return "Ошибка при сериализации: " + obj.GetType();
		    }
	    }

	    public static void LogException(Exception ex, string msg, HelixLogger logger)
	    {
		    var newEx = new ApplicationException(msg, ex);
			logger.Error(newEx);
	    }
    }

	public class TrackingException : ApplicationException
	{
		public TrackingException(string message) : base(message)
		{
		}
	}
}

using System;
using Newtonsoft.Json;

namespace HolderArchiveApp.QueueProcessing.Service
{
	public static class JsonExtension
	{
		public static string ToJson(this object obj)
		{
			return ToJson(obj, Formatting.Indented, new JsonSerializerSettings());
		}

		public static string ToJson(this object obj, Formatting formatting)
		{
			return ToJson(obj, formatting, new JsonSerializerSettings());
		}

		public static string ToJson(this object obj, JsonSerializerSettings settings)
		{
			return ToJson(obj, Formatting.Indented, settings);
		}

		public static string ToJson(this object obj, Formatting formatting, JsonSerializerSettings settings)
		{
			try
			{
				return JsonConvert.SerializeObject(obj, formatting, settings);
			}
			catch (Exception ex)
			{
				return $"{obj} (не удалось сериализовать объект {ex})";
			}
		}
	}
}

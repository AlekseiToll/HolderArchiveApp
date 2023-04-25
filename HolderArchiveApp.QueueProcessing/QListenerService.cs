using HRP.QServiceLib;
using HRP.QMessageLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HolderArchiveApp.Domain.Entities;

namespace HolderArchiveApp.QueueProcessing.Service
{
    internal class QListenerService : QService
    {
	    readonly QSettings _settings;

		public QListenerService(QSettings settings)
		{
			_settings = settings;

			_hostUri = settings.HostUri;								//"amqp://guest:guest@localhost:5672/";
			ParallelIndex = settings.ParallelIndex;						//1
			ExchangePrefix = settings.ExPrefix;							//"DevTest"
			errorMessageDelay = settings.RetiresTimeout;				//100
			errorMessageRetries = settings.RetriesCount;				//3
			clientProvidedName = settings.ClientProvidedName;			//"QListenerService"
			SendJsonMessage = settings.SendJsonMessage;					//true
		}

		public override void Initialize()
		{
			base.Initialize();
			RecieveMessage<QSampleInfoMessage>(SampleChangedHandler);
		}

		private QRecieverResult SampleChangedHandler(QSampleInfoMessage msg)
		{
			try
			{
				var sample = ConvertToSample(msg);
				bool res = SendDataToWebApi(sample);
				LoggerHost.LoggerInstance.Trace("Обработано сообщение об изменении статуса пробы " + msg.ToJson());
			}
			catch (Exception ex)
			{
				var exception = new ApplicationException("Не удалось обработать сообщение об изменении статуса пробы " + msg.ToJson(), ex);
				LoggerHost.LoggerInstance.Error(exception);
				throw exception;
			}
			return new QRecieverResult() { IsOk = true };
		}

		Sample ConvertToSample(QSampleInfoMessage msg)
		{
			return new Sample()
			{
				SampleNumber = msg.SampleNumber,
				LabelId = msg.SampleLabelId,
				ContainerType = msg.ContainerType,
				Workflow = msg.Workflow,
				Status = msg.SampleStatus,
				SampleTemplate = msg.SampleTemplate,
				ChangedOn = msg.ChangedOn,
				LoginOn = msg.LoginOn,
			};
		}

		private bool SendDataToWebApi(Sample sample)
		{
			try
			{
				//var uri = @"http://" + _settings.WebServerIp + ":" + _settings.WebServerPort + @"/QService/AddSampleData";
				var uri = @"http://" + _settings.WebServerUrl + "/api/QServiceArea/QService/AddSampleData";
				HttpClient client = new HttpClient();
				HttpResponseMessage response = client.PostAsJsonAsync<Sample>(uri, sample).Result;

				// Check that response was successful or throw exception
				response.EnsureSuccessStatusCode();
				response.Content.ReadAsAsync<HttpResponseMessage>().Wait();

				return true;
			}
			catch (Exception ex)
			{
				var exception = new ApplicationException("Не удалось передать сообщение WebAPI " + sample.ToJson(), ex);
				LoggerHost.LoggerInstance.Error(exception);
				throw exception;
			}
		}
	}
}

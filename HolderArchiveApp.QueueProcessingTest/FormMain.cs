using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.QueueProcessing.Service;
using HRP.QMessageLib;
using HRP.QServiceLib;

namespace HolderArchiveApp.QueueProcessingTest
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();
		}

		private void btnSend_Click(object sender, EventArgs e)
		{
			QSampleInfoMessage msg = new QSampleInfoMessage();
			msg.Workflow = "Test Workflow";
			msg.ChangedOn = DateTime.Now;
			msg.ContainerType = "ContainerType";
			msg.LoginOn = DateTime.Now;
			msg.SampleLabelId = "SampleLabelId";
			msg.SampleNumber = 123;
			msg.SampleStatus = "A";
			msg.SampleTemplate = "tmp_in";

			SendMessage<QSampleInfoMessage>(msg);
		}

		private static void SendMessage<T>(T message) where T : class, IQMessage
		{
			//string uri = "amqp://dev:dev@dev-spb-024.spb.helix.ru";
			string uri = "amqp://guest:guest@localhost:5672/";
			using (CancellationTokenSource tokenSource = new CancellationTokenSource())
			{
				var exchangeName = QWorkerFactory.GetExchangeName(message.GetType(), "DevTest");
				using (QPublisher<T> publisher = new QPublisher<T>(uri, exchangeName, tokenSource.Token))
				{
					var res = publisher.Publish(message);
				}
			}
		}
	}
}

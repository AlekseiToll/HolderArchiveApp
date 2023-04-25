using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogicLevel;
using HolderArchiveApp.Domain.Entities;

namespace HolderArchiveApp.Areas.QService.Controllers
{
	//[ClaimsRoleAuthorize]
	[RoutePrefix("api/QServiceArea/QService")]
	public class QServiceController : ApiController
    {
		private readonly DataManagerQService _dataManager;

		public QServiceController(DataManagerQService dataManager)
		{
			_dataManager = dataManager;
		}

		[HttpPost]
		//[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		[Route(nameof(AddSampleData))]
		public HttpResponseMessage AddSampleData([FromBody] Sample sample)
		{
			DataOperationResult res = _dataManager.AddOrUpdateSample(sample);
			if (res.ResultCode != DataOperationResult.ResultCodes.Ok)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, res.MessageForUI);
			}
			return Request.CreateResponse(HttpStatusCode.OK);
		}
	}
}

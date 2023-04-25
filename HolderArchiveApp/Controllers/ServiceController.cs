using System.Linq;
using System.Web.Mvc;
using BusinessLogicLevel;
using HolderArchiveApp.Domain.Entities;
using InnerPortal.Shared.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace HolderArchiveApp.Controllers
{
	[ClaimsRoleAuthorize]
	public class ServiceController : Controller
    {
		private readonly DataManagerService _dataManager;

		public ServiceController(DataManagerService dataManager)
		{
			_dataManager = dataManager;
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult GetLabs(DataSourceRequest request)
		{
			IQueryable<Laboratory> listLabs;
			DataOperationResult res = _dataManager.GetLabs(out listLabs);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				SelectList labs = new SelectList(listLabs, "Code", "Code");
				DataSourceResult result = labs.ToDataSourceResult(request);
				return Json(result);
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult GetContainerTypes(DataSourceRequest request)
		{
			IQueryable<ContainerTypeACGD> listContainerTypes;
			DataOperationResult res = _dataManager.GetContainerTypes(out listContainerTypes);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				//SelectList containerTypes = new SelectList(listContainerTypes, "Name", "Name");
				SelectList containerTypes = new SelectList(listContainerTypes, "Name", "NameAndDescription");
				DataSourceResult result = containerTypes.ToDataSourceResult(request);
				return Json(result);
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_dataManager.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
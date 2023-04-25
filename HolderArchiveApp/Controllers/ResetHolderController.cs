using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using BusinessLogicLevel;
using HolderArchiveApp.Domain.Entities;
using InnerPortal.Shared.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace HolderArchiveApp.Controllers
{
	[ClaimsRoleAuthorize]
	public class ResetHolderController : Controller
	{
		private readonly DataManagerResetHolder _dataManager;

		public ResetHolderController(DataManagerResetHolder dataManager)
		{
			_dataManager = dataManager;
		}

		// GET: HolderTypes
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin, ArchiveDropper")]
		public ActionResult Get(DataSourceRequest request)
		{
			List<HolderToReset> listHolders;
			var user = System.Web.HttpContext.Current.User;
			DataOperationResult res = _dataManager.GetHoldersToReset(user.Identity.Name, out listHolders);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				DataSourceResult result = listHolders.ToDataSourceResult(request);
				return Json(result);
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin, ArchiveDropper")]
		public ActionResult Reset([DataSourceRequest] DataSourceRequest request, int holderId)
		{
			DataOperationResult res = _dataManager.ResetHolder(holderId);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin, ArchiveDropper")]
		public ActionResult GetColorForHolder([DataSourceRequest] DataSourceRequest request, int holderId)
		{
			string color;
			DataOperationResult res = _dataManager.GetColorForHolder(holderId, out color);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				return new JsonResult { Data = new { Color = color }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}
	}
}
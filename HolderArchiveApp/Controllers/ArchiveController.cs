using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Web.Mvc;
using BusinessLogicLevel;
using HolderArchiveApp.Domain.Entities;
using InnerPortal.Shared.Attributes;

namespace HolderArchiveApp.Controllers
{
	[ClaimsRoleAuthorize]
	public class ArchiveController : Controller
	{
		private readonly DataManagerArchive _dataManager;

		public ArchiveController(DataManagerArchive dataManager)
		{
			_dataManager = dataManager;
		}

		// GET: HolderTypes
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin, Archive")]
		public ActionResult GetHoldersForArchive()
		{
			var user = System.Web.HttpContext.Current.User;
			List<HolderTypeToArchive> listHolderTypesToArchive;
			DataOperationResult res = _dataManager.GetHoldersForArchive(user.Identity.Name, out listHolderTypesToArchive);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				return Json(listHolderTypesToArchive);
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin, Archive")]
		public ActionResult GetHolderTypeInfo(int holderTypeId)
		{
			HolderType holderType;
			DataOperationResult res = _dataManager.GetHolderTypeById(holderTypeId, out holderType);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				return new JsonResult
				{
					Data = new
					{
						Name = holderType.Name,
						Color = ColorTranslator.ToHtml(holderType.HolderColor),
						CountCellsHorizontal = holderType.CountCellsHorizontal,
						CountCellsVertical = holderType.CountCellsVertical
					},
					JsonRequestBehavior = JsonRequestBehavior.AllowGet
				};
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult GetValidityOfSample(string barCode, int holderTypeId)
		{
			bool resValidity;
			string info;
			DataOperationResult res = _dataManager.GetValidityOfSample(barCode, holderTypeId, out resValidity, out info);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				return new JsonResult { Data = new { Valid = resValidity, Info = info }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin, Archive")]
		public ActionResult ArchiveHolder(int holderId)
		{
			DataOperationResult res = _dataManager.ArchiveHolder(holderId);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin, Archive")]
		public ActionResult SaveSampleCoordinates(int holderId, string row, string column, string barCode)
		{
			DataOperationResult res = _dataManager.SaveSampleCoordinates(holderId, row, column, barCode);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

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
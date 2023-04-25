using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BusinessLogicLevel;
using HolderArchiveApp.Domain.Entities;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using HolderArchiveApp.Models;
using InnerPortal.Shared.Attributes;

namespace HolderArchiveApp.Controllers
{
	[ClaimsRoleAuthorize]
	public class HolderTypesController : Controller
	{ 
	    private readonly DataManagerHolderTypes _dataManager;

		public HolderTypesController(DataManagerHolderTypes dataManager)
		{
			_dataManager = dataManager;
		}

		// GET: HolderTypes
		public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult Get([DataSourceRequest] DataSourceRequest request)
		{
			List<HolderType> listHolderTypes;
			DataOperationResult res = _dataManager.GetHolderTypesList(out listHolderTypes);
			if (res.ResultCode != DataOperationResult.ResultCodes.Ok)
			{
				Response.StatusCode = 500;
				return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			}

			List<HolderTypeViewModel> listDataViewModel = new List<HolderTypeViewModel>();
			foreach (var item in listHolderTypes)
			{
				listDataViewModel.Add(new HolderTypeViewModel(item));
			}
			
			IQueryable<HolderTypeViewModel> qlistDataViewModel = listDataViewModel.AsQueryable();

			DataSourceResult result = qlistDataViewModel.ToDataSourceResult(request);
			return Json(result);
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult GetHoldersByHolderType(DataSourceRequest request, int? id)
		{
			if (id == null)
			{
				Response.StatusCode = 500;
				return new JsonResult { Data = new { ErrorMessage = "Не указан тип штатива" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			}

			List<Holder> listHolders;
			DataOperationResult res = _dataManager.GetHoldersByType((int)id, out listHolders);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				DataSourceResult result = listHolders.ToDataSourceResult(request);
				return Json(result);
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult GetCountNewHoldersForHolderType([DataSourceRequest] DataSourceRequest request, int holderTypeId)
		{
			int count;
			DataOperationResult res = _dataManager.GetCountNewHoldersForHolderType(holderTypeId, out count);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				return new JsonResult {Data = new {Count = count}, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult GetNewHoldersForHolderType([DataSourceRequest] DataSourceRequest request, int holderTypeId)
		{
			int[] holders;
			DataOperationResult res = _dataManager.GetNewHoldersForHolderType(holderTypeId, out holders);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				return new JsonResult {Data = new {Array = holders}, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult Update([DataSourceRequest]DataSourceRequest request, HolderTypeViewModel holderTypeViewModel)
		{
			//if (ModelState.IsValid)
			//{
			holderTypeViewModel.ChangedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
			HolderType holderType = holderTypeViewModel.GetHolderType();
			DataOperationResult res = _dataManager.UpdateHolderType(holderType);

			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				foreach (var modelValue in ModelState.Values)
					modelValue.Errors.Clear();
				return Json(new[] {holderTypeViewModel}.ToDataSourceResult(request, ModelState));
			}

			ModelState.AddModelError("ErrorMessage", res.MessageForUI);
			return Json(new[] { holderTypeViewModel }.ToDataSourceResult(request, ModelState));
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		//[ValidateAntiForgeryToken]
		public ActionResult Create([DataSourceRequest] DataSourceRequest request, HolderTypeViewModel holderTypeViewModel)
	    {
		    holderTypeViewModel.CreatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
		    holderTypeViewModel.ChangedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
		    HolderType holderType = holderTypeViewModel.GetHolderType();
		    DataOperationResult res = _dataManager.AddHolderType(holderType);
		    if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
		    {
			    foreach (var modelValue in ModelState.Values)
				    modelValue.Errors.Clear();
			    return Json(new[] {holderTypeViewModel}.ToDataSourceResult(request, ModelState));
		    }

		    ModelState.AddModelError("ErrorMessage", res.MessageForUI);
		    return Json(new[] {holderTypeViewModel}.ToDataSourceResult(request, ModelState));
	    }

		// POST: HolderTypes/Delete/5
		//[HttpPost, ActionName("Delete")]
		//[ValidateAntiForgeryToken]
		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult DeleteHolderConfirmed(int holderId)
		{
			DataOperationResult res = _dataManager.DeleteHolderConfirmed(holderId);

			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		/// <summary>All virtual holders to work</summary>
		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult AllToWork([DataSourceRequest] DataSourceRequest request, int holderTypeId)
		{
			DataOperationResult res = _dataManager.AllVirtualHoldersToWork(holderTypeId);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult AddHoldersToType([DataSourceRequest] DataSourceRequest request, int holderTypeId, int count)
		{
			DataOperationResult res = _dataManager.AddHoldersToType(holderTypeId, count);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
				return new HttpStatusCodeResult(HttpStatusCode.OK);

			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult OneHolderToWork([DataSourceRequest] DataSourceRequest request, int holderId)
		{
			DataOperationResult res = _dataManager.OneVirtualHolderToWork(holderId);
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

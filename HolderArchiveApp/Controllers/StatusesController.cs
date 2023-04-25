using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLogicLevel;
using HolderArchiveApp.Domain.Entities;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using HolderArchiveApp.Models;
using InnerPortal.Shared.Attributes;
using Kendo.Mvc;

namespace HolderArchiveApp.Controllers
{
	[ClaimsRoleAuthorize]
	public class StatusesController : Controller
	{
		private readonly DataManagerStatuses _dataManager;

		public StatusesController(DataManagerStatuses dataManager)
		{
			_dataManager = dataManager;
		}

		// GET: Statuses
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult Get([DataSourceRequest] DataSourceRequest request)
		{
			//if (request.PageSize == 0) {request.PageSize = 10;}
			List<WorkflowRecord> listData;
			DataOperationResult res = _dataManager.GetWorkflowRecordsList(out listData);
			if (res.ResultCode != DataOperationResult.ResultCodes.Ok)
			{
				Response.StatusCode = 500;
				return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			}

			List<WorkflowRecordViewModel> listDataViewModel = new List<WorkflowRecordViewModel>();
			foreach (var item in listData)
			{
				listDataViewModel.Add(new WorkflowRecordViewModel(item));
			}

			IQueryable<WorkflowRecordViewModel> qlistDataViewModel = listDataViewModel.AsQueryable();

			ModifyFilters(request.Filters);

			DataSourceResult result = qlistDataViewModel.ToDataSourceResult(request);
			return Json(result);
		}

		private void ModifyFilters(IList<IFilterDescriptor> filters)
		{
			if (filters != null && filters.Any())
			{
				foreach (var filter in filters)
				{
					var descriptor = filter as FilterDescriptor;
					if (descriptor != null && descriptor.Member == "Statuses")
					{
						descriptor.Member = "StatusesAsString";
					}
					else if (filter is CompositeFilterDescriptor)
					{
						ModifyFilters(((CompositeFilterDescriptor)filter).FilterDescriptors);
					}
				}
			}
		}


		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult Update([DataSourceRequest]DataSourceRequest request, WorkflowRecordViewModel recordViewModel)
		{
			WorkflowRecord workflowRecord = recordViewModel.GetWorkflowRecord();
			DataOperationResult res = _dataManager.UpdateWorkflowRecord(workflowRecord);

			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				foreach (var modelValue in ModelState.Values)
					modelValue.Errors.Clear();

				return Json(new[] {recordViewModel}.ToDataSourceResult(request, ModelState));
			}

			ModelState.AddModelError("ErrorMessage", res.MessageForUI);
			return Json(new[] { recordViewModel }.ToDataSourceResult(request, ModelState));
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult GetWorkflows(DataSourceRequest request)
		{
			IQueryable<Workflow> workflows;
			DataOperationResult res = _dataManager.GetWorkflows(out workflows);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				SelectList listWorkflows = new SelectList(workflows, "Description", "Description");
				DataSourceResult result = listWorkflows.ToDataSourceResult(request);
				return Json(result);
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		// POST: Statuses/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		//[ValidateAntiForgeryToken]
		public ActionResult Create([DataSourceRequest] DataSourceRequest request, WorkflowRecordViewModel recordViewModel)
		{
			WorkflowRecord workflowRecord = recordViewModel.GetWorkflowRecord();
			DataOperationResult res = _dataManager.AddWorkflowRecord(workflowRecord);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				foreach (var modelValue in ModelState.Values)
					modelValue.Errors.Clear();
			
				return Json(new[] {recordViewModel}.ToDataSourceResult(request, ModelState));
			}

			ModelState.AddModelError("ErrorMessage", res.MessageForUI);
			return Json(new[] { recordViewModel }.ToDataSourceResult(request, ModelState));
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

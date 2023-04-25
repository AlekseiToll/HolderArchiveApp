using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogicLevel;
using HolderArchiveApp.Domain.Entities;
using HolderArchiveApp.Models;
using InnerPortal.Shared.Attributes;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace HolderArchiveApp.Controllers
{
	[ClaimsRoleAuthorize]
	public class UsersController : Controller
    {
		private readonly DataManagerUsers _dataManager;

		public UsersController(DataManagerUsers dataManager)
		{
			_dataManager = dataManager;
		}

		// GET: Users
		public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult Get(DataSourceRequest request)
		{
			List<User> listUsers;
			DataOperationResult res = _dataManager.GetUsersList(out listUsers);
			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				List<UserViewModel> listDataViewModel = new List<UserViewModel>();
				foreach (var item in listUsers)
				{
					string lab;
					DataOperationResult resLab = _dataManager.GetLaboratoryForUser(item.Id, out lab);
					if (resLab.ResultCode == DataOperationResult.ResultCodes.Ok)
					{
						listDataViewModel.Add(new UserViewModel(item, lab));
					}
					else
					{
						Response.StatusCode = 500;
						return new JsonResult { Data = new { ErrorMessage = resLab.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
					}
				}

				DataSourceResult result = listDataViewModel.ToDataSourceResult(request);
				return Json(result);
			}
			Response.StatusCode = 500;
			return new JsonResult { Data = new { ErrorMessage = res.MessageForUI }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
		}

		[HttpPost]
		[ClaimsRoleAuthorize(Roles = "Developer, ArchiveAdmin")]
		public ActionResult Update([DataSourceRequest]DataSourceRequest request, UserViewModel userViewModel)
		{
			User user = userViewModel.GetUser();
			DataOperationResult res = _dataManager.UpdateUserLaboratory(user.Id, userViewModel.LaboratoryName);

			if (res.ResultCode == DataOperationResult.ResultCodes.Ok)
			{
				foreach (var modelValue in ModelState.Values)
					modelValue.Errors.Clear();

				return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
			}

			ModelState.AddModelError("ErrorMessage", res.MessageForUI);
			return Json(new[] { userViewModel }.ToDataSourceResult(request, ModelState));
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

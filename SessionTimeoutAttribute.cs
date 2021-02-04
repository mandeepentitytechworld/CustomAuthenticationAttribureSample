using AdminApp.Business.Interfaces;
using AdminApp.Model;
using AdminApp.Model.Constant;
using AdminApp.Model.DataModels;
using AdminApp.WebAdmin.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdminApp.Model.Common;

namespace AdminApp.WebAdmin.Filters.SessionTimeoutAttribute
{
    public class CheckSessionTimeoutOnActionExcutingAttribute : ActionFilterAttribute
    {
        #region GLOBAL DECLARATION 
        private readonly IAccountService _accountService;
        #endregion

        #region CONSTRUCTOR
        public CheckSessionTimeoutOnActionExcutingAttribute(IAccountService accountService)
        {
            _accountService = accountService;
        }
        #endregion

        #region ON ACTION EXECUTING
        public override  void OnActionExecuting(ActionExecutingContext context)
        {
            
            if (SessionHelper.GetObjectFromJson<Account>(context.HttpContext.Session, SessionHelper.ConstCurrentAccount) == null)
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    ClaimsIdentity identity = context.HttpContext.User.Identity as ClaimsIdentity;
                    string objectID = identity.Claims.FirstOrDefault(x => x.Type == Constant.ConstClaimObjectIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(objectID))
                    {
                        Account resultAccount =  _accountService.GetByAzureObjectId(objectID).Result;
                        if (resultAccount != null)
                        {
                            bool isStaff = false;
                            if (resultAccount?.AccountRole?.Any() == true)
                            {
                                isStaff = resultAccount.AccountRole.Where(r => r.RoleId != Enums.Role.Client.GetHashCode()).Any();
                                if(isStaff)
                                {
                                    if (resultAccount?.LocationAccount?.Any() == true)
                                    {
                                        if (resultAccount.LocationAccount.Count == 1)
                                        {
                                            context.HttpContext.Session.SetSession(Constant.LocationId, resultAccount.LocationAccount[0].LocationId);
                                        }
                                    }
                                    SessionHelper.SetObjectAsJson(context.HttpContext.Session, SessionHelper.ConstCurrentAccount, resultAccount);
                                }
                            }
                            if(isStaff == false)
                            {
                                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                {
                                    controller = "Session",
                                    action = "SignOutRenderpage",
                                    from = SignOutTypes.Unauthorized
                                }));
                            }
                        }
                    }
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Session",
                        action = "SignOutRenderpage",
                        from = SignOutTypes.SessionTimeout
                    }));
                }
            }
            else
            {
                bool anyRoleExist = false;
                Account resultAccount = SessionHelper.GetObjectFromJson<Account>(context.HttpContext.Session, SessionHelper.ConstCurrentAccount);
                if ((resultAccount != null) && (resultAccount.AccountRole.Count > 0))
                {
                    anyRoleExist = true;
                }
                if (!anyRoleExist)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Session",
                        action = "SignOutRenderpage",
                        from = SignOutTypes.Unauthorized
                    }));
                }
            }
            base.OnActionExecuting(context);
        }
        #endregion
    }

    public class CheckSessionTimeoutOnActionExcutedAttribute : ActionFilterAttribute
    {

        #region Global Declaration 
        private readonly IAccountService _accountService;
        #endregion

        #region Constructor
        public CheckSessionTimeoutOnActionExcutedAttribute(IAccountService accountService)
        {
            _accountService = accountService;
        }
        #endregion

        #region ON ACTION EXECUTED
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (SessionHelper.GetObjectFromJson<Account>(context.HttpContext.Session, SessionHelper.ConstCurrentAccount) == null)
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    ClaimsIdentity identity = context.HttpContext.User.Identity as ClaimsIdentity;
                    string objectID = identity.Claims.FirstOrDefault(x => x.Type == Constant.ConstClaimObjectIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(objectID))
                    {
                        Account resultAccount = _accountService.GetByAzureObjectId(objectID).Result;
                        if (resultAccount != null)
                        {
                            bool isStaff = false;
                            if (resultAccount?.AccountRole?.Any() == true)
                            {
                                isStaff = resultAccount.AccountRole.Where(r => r.RoleId != Enums.Role.Client.GetHashCode()).Any();
                                if(isStaff)
                                {
                                    SessionHelper.SetObjectAsJson(context.HttpContext.Session, SessionHelper.ConstCurrentAccount, resultAccount);
                                }
                            }
                            if (isStaff == false)
                            {
                                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                                {
                                    controller = "Session",
                                    action = "SignOutRenderpage",
                                    from = SignOutTypes.Unauthorized
                                }));
                            }
                        }
                    }
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Session",
                        action = "SignOutRenderpage",
                        from = SignOutTypes.SessionTimeout
                    }));
                }
            }
            else
            {
                bool anyRoleExist = false;
                Account resultAccount = SessionHelper.GetObjectFromJson<Account>(context.HttpContext.Session, SessionHelper.ConstCurrentAccount);
                if ((resultAccount != null) && (resultAccount.AccountRole.Count > 0))
                {
                    anyRoleExist = true;
                }
                if (!anyRoleExist)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Session",
                        action = "SignOutRenderpage",
                        from = SignOutTypes.Unauthorized
                    }));
                }
            }
            base.OnActionExecuted(context);
        }
        #endregion
    }

}

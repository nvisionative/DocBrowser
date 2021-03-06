using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Api;

namespace Connect.DNN.Modules.DocBrowser.Common
{
    public enum SecurityAccessLevel
    {
        Anonymous = 0,
        Authenticated = 1,
        View = 2,
        Edit = 3,
        Admin = 4,
        Host = 5
    }

    public class DocBrowserAuthorizeAttribute : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
    {
        public SecurityAccessLevel SecurityLevel { get; set; }
        public UserInfo User { get; set; }

        public DocBrowserAuthorizeAttribute()
        {
            SecurityLevel = SecurityAccessLevel.Admin;
        }

        public DocBrowserAuthorizeAttribute(SecurityAccessLevel accessLevel)
        {
            SecurityLevel = accessLevel;
        }

        public override bool IsAuthorized(AuthFilterContext context)
        {
            if (SecurityLevel == SecurityAccessLevel.Anonymous)
            {
                return true;
            }
            User = HttpContextSource.Current.Request.IsAuthenticated ? UserController.Instance.GetCurrentUserInfo() : new UserInfo();
            ContextSecurity security = new ContextSecurity(context.ActionContext.Request.FindModuleInfo());
            switch (SecurityLevel)
            {
                case SecurityAccessLevel.Authenticated:
                    return User.UserID != -1;
                case SecurityAccessLevel.Host:
                    return User.IsSuperUser;
                case SecurityAccessLevel.Admin:
                    return security.IsAdmin;
                case SecurityAccessLevel.Edit:
                    return security.CanEdit;
                case SecurityAccessLevel.View:
                    return security.CanView;
            }
            return false;
        }
    }
}
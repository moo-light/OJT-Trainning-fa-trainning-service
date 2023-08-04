using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Utils
{
    public enum PermissionItem
    {
        SyllabusPermission,
        TrainingProgramPermission,
        ClassPermission,
        AttendancePermission,
        LearningMaterial,
        UserPermission,
        TrainingMaterialPermission,
        ApplicationPermission,
        TrainingClassPermission,
    }

    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var getClaimValue = context.HttpContext.User.FindFirst(c => c.Type == _claim.Type).Value;
            if (getClaimValue != "FullAccess")
            {
                var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
                if (!hasClaim)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }

}

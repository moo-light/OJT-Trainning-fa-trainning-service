using Application.ViewModels.UserViewModels;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Utils
{
    public static class GenerateJsonWebTokenString
    {
        public static string GenerateJsonWebToken(this User user, string secretKey, DateTime now)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("SyllabusPermission",user.Role.SyllabusPermission),
                new Claim("TrainingProgramPermission",user.Role.TrainingProgramPermission),
                new Claim("TrainingMaterialPermission",user.Role.TrainingMaterialPermission),
                new Claim("ClassPermission",user.Role.ClassPermission),
                new Claim("LearningMaterial",user.Role.LearningMaterial),
                new Claim("AttendancePermission",user.Role.AttendancePermission),
                new Claim("UserPermission",user.Role.UserPermission),
                new Claim("ApplicationPermission",user.Role.ApplicationPermission),
                new Claim(ClaimTypes.NameIdentifier ,user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
            };
            var token = new JwtSecurityToken(
               issuer: secretKey,
               audience: secretKey,
               claims,
               expires: now.AddMinutes(120),
               signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal? GetPrincipalFromExpiredToken(this string? token, string secretKey)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {
                
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            else return null;
            

        }

        public static JwtDTO VerifyToken(this string? token, string secretKey)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {

                var principal = tokenHandler.ReadJwtToken(token).Claims;

                var result = new JwtDTO
                {
                    UserId = principal.First(x => x.Type == "UserId").Value,
                    ApplicationPermission = principal.First(x => x.Type == "ApplicationPermission").Value,
                    AttendancePermission = principal.First(x => x.Type == "AttendancePermission").Value,
                    ClassPermission = principal.First(x => x.Type == "ClassPermission").Value,
                    LearningMaterial = principal.First(x => x.Type == "LearningMaterial").Value,
                    SyllabusPermission = principal.First(x => x.Type == "SyllabusPermission").Value,
                    TrainingMaterialPermission = principal.First(x => x.Type == "TrainingMaterialPermission").Value,
                    TrainingProgramPermission = principal.First(x => x.Type == "TrainingProgramPermission").Value,
                    UserPermission = principal.First(x => x.Type == "UserPermission").Value
                };
                return result;
            }
            else return null;
        }

    }
}

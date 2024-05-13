using Tel.Api.Models;
using Tel.Core.Config;
using TelServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Tel.Api.Controllers
{
    public class AccountController : BaseController
    {
        readonly IOptionsMonitor<DefaultServerConfig> serverOptionsMonitor;

        public AccountController(IOptionsMonitor<DefaultServerConfig> optionsMonitor)
        {
            serverOptionsMonitor = optionsMonitor;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ApiResponse GetToken(GetTokenRequest request)
        {
            if ((serverOptionsMonitor.CurrentValue?.Api?.Accounts?.Length ?? 0) == 0)
            {
                ApiResponse.errorCode = ErrorCodeEnum.NoAccount;
                ApiResponse.errorMessage = "账号或密码错误";
                return ApiResponse;
            }

            var account = serverOptionsMonitor.CurrentValue.Api.Accounts.FirstOrDefault((x) =>
            {
                return x.Name.Equals(request.name) && x.Password.Equals(request.password);
            });

            if (account == null)
            {
                ApiResponse.errorCode = ErrorCodeEnum.NoAccount;
                ApiResponse.errorMessage = "账号或密码错误";
                return ApiResponse;
            }

            // 生成Token
            var claims = new[] {
                    new Claim("Name", account.Name)
                };

            ApiResponse.data = GenerateToken(
                claims,
                serverOptionsMonitor.CurrentValue.Api.JWT.IssuerSigningKey,
                serverOptionsMonitor.CurrentValue.Api.JWT.Expires,
                serverOptionsMonitor.CurrentValue.Api.JWT.ValidIssuer,
                serverOptionsMonitor.CurrentValue.Api.JWT.ValidAudience);

            return ApiResponse;
        }

        public static string GenerateToken(
            IEnumerable<Claim> claims, string Secret, int expiresMinutes = 60, string issuer = null, string audience = null)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiresMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}

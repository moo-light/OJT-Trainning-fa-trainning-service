using Application.ViewModels.UserViewModels;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils;

public class ExternalAuthUtils : IExternalAuthUtils
{
    private readonly IConfiguration _configuration;
    private readonly string _googleClientId;

    public ExternalAuthUtils(IConfiguration configuration)
    {
        _configuration = configuration;
        _googleClientId = _configuration["Authentication:Google:ClientId"]!;
    }

    public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
    {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _googleClientId }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
            return payload;
    }
}

public interface IExternalAuthUtils
{
    public Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth);
}
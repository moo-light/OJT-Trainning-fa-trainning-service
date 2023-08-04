using Application.Utils;
using Application.ViewModels.UserViewModels;
using AutoFixture;
using Castle.Core.Configuration;
using Domains.Test;
using FluentAssertions;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.Utils
{
    public class ExternalAuthUtilsTests : SetupTest
    {
        private readonly IExternalAuthUtils _externalAuthUtils;

        public ExternalAuthUtilsTests()
        {
            _externalAuthUtils = new ExternalAuthUtils(_config.Object);
        }
        //help me input this         
        public async Task VerifyGoogleToken_ValidToken_ReturnsPayload()
        {
            // Arrange
            var externalAuth = new ExternalAuthDto { IdToken = "valid-token" }; // can not get this without front-end service

            // Act
            var payload = await _externalAuthUtils.VerifyGoogleToken(externalAuth);

            payload.Should().NotBeNull();
            //payload.Audience.Should().Be("your-google-client-id");
            payload.Audience.Should().Be("501463064471-v1sn8e75fqk8bjg975dbn6fb29459dsq.apps.googleusercontent.com");
        }
        [Fact]
        public async Task VerifyGoogleToken_Should()
        {

            var result = async () => await _externalAuthUtils.VerifyGoogleToken(_fixture.Create<ExternalAuthDto>());
            await result.Should().ThrowAsync<InvalidJwtException>();
        }
    }

}


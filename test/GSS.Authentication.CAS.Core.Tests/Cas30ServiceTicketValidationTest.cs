﻿using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using GSS.Authentication.CAS.Validation;
using Microsoft.Extensions.FileProviders;
using RichardSzalay.MockHttp;
using Xunit;

namespace GSS.Authentication.CAS.Tests.Validation
{
    public class Cas30ServiceTicketValidationTest : IClassFixture<CasFixture>
    {
        private CasFixture fixture;

        public Cas30ServiceTicketValidationTest(CasFixture fixture)
        {
            this.fixture = fixture;
        }
        
        [Fact]
        public async Task ValidateServiceTicket_SuccessAsync()
        {
            // Arrange
            var ticket = Guid.NewGuid().ToString();
            var requestUrl = $"{fixture.Options.CasServerUrlBase}/p3/serviceValidate?ticket={ticket}&service={Uri.EscapeDataString(fixture.Service)}";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, requestUrl)
              .Respond(fixture.FileProvider.ReadAsHttpContent("Cas30ValidationSuccess.xml", mediaType: "application/xml"));
            var validator = new Cas30ServiceTicketValidator(fixture.Options, new HttpClient(mockHttp));

            // Act
            var principal = await validator.ValidateAsync(ticket, fixture.Service, CancellationToken.None);

            //Assert
            Assert.NotNull(principal);
            Assert.NotNull(principal.Assertion);
            Assert.Equal(principal.GetPrincipalName(), principal.Assertion.PrincipalName);
            Assert.NotEmpty(principal.Assertion.Attributes);
            mockHttp.VerifyNoOutstandingRequest();
            mockHttp.VerifyNoOutstandingExpectation();
        }
        
        [Fact]
        public async Task ValidateServiceTicket_FailAsync()
        {
            // Arrange
            var ticket = Guid.NewGuid().ToString();
            var requestUrl = $"{fixture.Options.CasServerUrlBase}/p3/serviceValidate?ticket={ticket}&service={Uri.EscapeDataString(fixture.Service)}";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, requestUrl)
              .Respond(fixture.FileProvider.ReadAsHttpContent("Cas20ValidationFail.xml", mediaType: "application/xml"));
            var validator = new Cas30ServiceTicketValidator(fixture.Options, new HttpClient(mockHttp));

            // Act & Assert
            await Assert.ThrowsAsync<AuthenticationException>(() => validator.ValidateAsync(ticket, fixture.Service, CancellationToken.None));
            mockHttp.VerifyNoOutstandingRequest();
            mockHttp.VerifyNoOutstandingExpectation();
        }
        
        [Fact]
        public async Task ValidateServiceTicket_ErrorStatusCodeAsync()
        {
            // Arrange
            var ticket = Guid.NewGuid().ToString();
            var requestUrl = $"{fixture.Options.CasServerUrlBase}/p3/serviceValidate?ticket={ticket}&service={Uri.EscapeDataString(fixture.Service)}";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, requestUrl)
              .Respond(HttpStatusCode.BadRequest);
            var validator = new Cas30ServiceTicketValidator(fixture.Options, new HttpClient(mockHttp));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => validator.ValidateAsync(ticket, fixture.Service, CancellationToken.None));
            mockHttp.VerifyNoOutstandingRequest();
            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}

// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Toralux.Open.IdentityServer.STS.Identity.IntegrationTests.Common;
using Toralux.Open.IdentityServer.STS.Identity.IntegrationTests.Mocks;
using Toralux.Open.IdentityServer.STS.Identity.IntegrationTests.Tests.Base;
using Xunit;

namespace Toralux.Open.IdentityServer.STS.Identity.IntegrationTests.Tests
{
    public class ManageControllerTests : BaseClassFixture
    {
        public ManageControllerTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task AuthorizeUserCanAccessManageViews()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client,registerFormData);

            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(registerResponse);
            
            foreach (var route in RoutesConstants.GetManageRoutes())
            {
                // Act
                var response = await Client.GetAsync($"/Manage/{route}");

                // Assert
                response.EnsureSuccessStatusCode();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task UnAuthorizeUserCannotAccessManageViews()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            foreach (var route in RoutesConstants.GetManageRoutes())
            {
                // Act
                var response = await Client.GetAsync($"/Manage/{route}");

                // Assert      
                response.StatusCode.Should().Be(HttpStatusCode.Redirect);

                //The redirect to login
                response.Headers.Location.ToString().Should().Contain("Account/Login");
            }
        }
        
        [Fact]
        public async Task UserIsAbleToUpdateProfile()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(registerResponse);

            // Prepare request to update profile
            const string manageAction = "/Manage/Index";
            var manageResponse = await Client.GetAsync(manageAction);
            var antiForgeryToken = await manageResponse.ExtractAntiForgeryToken();

            var manageProfileData = UserMocks.GenerateManageProfileData(registerFormData["Email"], antiForgeryToken);

            // Update profile
            var requestWithAntiForgeryCookie = RequestHelper.CreatePostRequestWithCookies(manageAction, manageProfileData, manageResponse);
            var requestWithIdentityCookie = CookiesHelper.CopyCookiesFromResponse(requestWithAntiForgeryCookie, registerResponse);
            var responseMessage = await Client.SendAsync(requestWithIdentityCookie);

            // Assert      
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Redirect);

            //The redirect to login
            responseMessage.Headers.Location.ToString().Should().Be("/Manage");
        }

        [Fact]
        public async Task UserIsRedirectedBackToPasskeysWhenNoPasskeyCredentialIsPosted()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(registerResponse);

            // Prepare request to add passkey
            const string passkeysAction = "/Manage/Passkeys";
            const string addPasskeyAction = "/Manage/AddPasskey";
            var passkeysResponse = await Client.GetAsync(passkeysAction);
            var antiForgeryToken = await passkeysResponse.ExtractAntiForgeryToken();

            var addPasskeyData = new Dictionary<string, string>
            {
                { UserMocks.AntiForgeryTokenKey, antiForgeryToken }
            };

            // Add passkey without credential payload
            var requestWithAntiForgeryCookie = RequestHelper.CreatePostRequestWithCookies(addPasskeyAction, addPasskeyData, passkeysResponse);
            var requestWithIdentityCookie = CookiesHelper.CopyCookiesFromResponse(requestWithAntiForgeryCookie, registerResponse);
            var responseMessage = await Client.SendAsync(requestWithIdentityCookie);

            // Assert
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Redirect);
            responseMessage.Headers.Location.ToString().Should().Be(passkeysAction);
        }

        [Fact]
        public async Task UserIsRedirectedBackToPasskeysWhenPasskeyClientErrorIsPosted()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(registerResponse);

            // Prepare request to add passkey with simulated client-side error
            const string passkeysAction = "/Manage/Passkeys";
            const string addPasskeyAction = "/Manage/AddPasskey";
            var passkeysResponse = await Client.GetAsync(passkeysAction);
            var antiForgeryToken = await passkeysResponse.ExtractAntiForgeryToken();

            var addPasskeyData = new Dictionary<string, string>
            {
                { "Passkey.Error", "No passkey was provided by the authenticator." },
                { UserMocks.AntiForgeryTokenKey, antiForgeryToken }
            };

            // Add passkey with simulated passkey error
            var requestWithAntiForgeryCookie = RequestHelper.CreatePostRequestWithCookies(addPasskeyAction, addPasskeyData, passkeysResponse);
            var requestWithIdentityCookie = CookiesHelper.CopyCookiesFromResponse(requestWithAntiForgeryCookie, registerResponse);
            var responseMessage = await Client.SendAsync(requestWithIdentityCookie);

            // Assert
            responseMessage.StatusCode.Should().Be(HttpStatusCode.Redirect);
            responseMessage.Headers.Location.ToString().Should().Be(passkeysAction);
        }

        [Fact]
        public async Task UserIsRedirectedToPasskeysWhenRenamePasskeyIdIsInvalid()
        {
            // Clear headers
            Client.DefaultRequestHeaders.Clear();

            // Register new user
            var registerFormData = UserMocks.GenerateRegisterData();
            var registerResponse = await UserMocks.RegisterNewUserAsync(Client, registerFormData);

            // Get cookie with user identity for next request
            Client.PutCookiesOnRequest(registerResponse);

            // Invalid base64url id should redirect back to passkeys
            var response = await Client.GetAsync("/Manage/RenamePasskey?id=not-a-valid-id");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location.ToString().Should().Be("/Manage/Passkeys");
        }
    }
}

// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Toralux.Open.IdentityServer.STS.Identity.Configuration.Test;

namespace Toralux.Open.IdentityServer.STS.Identity.IntegrationTests.Common
{
    public static class WebApplicationFactoryExtensions 
    {
        public static HttpClient SetupClient(this WebApplicationFactory<StartupTest> fixture)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            return fixture.WithWebHostBuilder(
                builder => builder
                    .UseStartup<StartupTest>()
                    .ConfigureTestServices(services => { })
            ).CreateClient(options);
        }
    }
}
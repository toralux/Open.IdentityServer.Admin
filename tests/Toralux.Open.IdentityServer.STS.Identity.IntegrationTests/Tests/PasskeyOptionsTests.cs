// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Toralux.Open.IdentityServer.STS.Identity.Helpers;
using Xunit;

namespace Toralux.Open.IdentityServer.STS.Identity.IntegrationTests.Tests
{
    public class PasskeyOptionsTests
    {
        [Theory]
        [InlineData("https://localhost", false, true)]
        [InlineData("http://127.0.0.1:44310", false, true)]
        [InlineData("https://localhost", true, false)]
        [InlineData("ftp://localhost", false, false)]
        [InlineData("https://example.com", false, false)]
        public async Task ConfigurePasskeyOptionsDevelopmentFallbackValidatesOrigin(string origin, bool crossOrigin, bool expected)
        {
            var options = BuildPasskeyOptions(isDevelopment: true);

            options.ValidateOrigin.Should().NotBeNull();

            var result = await options.ValidateOrigin!(new PasskeyOriginValidationContext
            {
                HttpContext = new DefaultHttpContext(),
                Origin = origin,
                CrossOrigin = crossOrigin
            });

            result.Should().Be(expected);
        }

        private static IdentityPasskeyOptions BuildPasskeyOptions(bool isDevelopment)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .Build();
            var services = new ServiceCollection();

            services.ConfigurePasskeyOptions(configuration, isDevelopment);

            using var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IOptions<IdentityPasskeyOptions>>().Value;
        }
    }
}

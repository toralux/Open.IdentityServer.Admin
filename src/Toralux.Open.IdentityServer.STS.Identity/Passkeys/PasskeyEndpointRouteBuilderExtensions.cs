// Copyright (c) Duende Software. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// Modified by Jan Skoruba

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Toralux.Open.IdentityServer.STS.Identity.Helpers;

namespace Toralux.Open.IdentityServer.STS.Identity.Passkeys
{
    public static class PasskeyEndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapPasskeyEndpoints<TUser>(this IEndpointRouteBuilder endpoints)
            where TUser : class
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            var accountGroup = endpoints.MapGroup("/Identity/Account").ExcludeFromDescription();

            accountGroup.MapPost("/PasskeyCreationOptions", async (
                HttpContext context,
                [FromServices] UserManager<TUser> userManager,
                [FromServices] SignInManager<TUser> signInManager,
                [FromServices] IAntiforgery antiforgery) =>
            {
                await antiforgery.ValidateRequestAsync(context);

                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                {
                    return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                }

                var userId = await userManager.GetUserIdAsync(user);
                var userName = await userManager.GetUserNameAsync(user) ?? "User";

                var optionsJson = await signInManager.MakePasskeyCreationOptionsAsync(new()
                {
                    Id = userId,
                    Name = userName,
                    DisplayName = userName
                });
                return TypedResults.Content(optionsJson, contentType: "application/json");
            });

            accountGroup.MapPost("/PasskeyRequestOptions", async (
                HttpContext context,
                [FromServices] UserResolver<TUser> userResolver,
                [FromServices] SignInManager<TUser> signInManager,
                [FromServices] IAntiforgery antiforgery,
                [FromQuery] string username) =>
            {
                await antiforgery.ValidateRequestAsync(context);

                var user = string.IsNullOrWhiteSpace(username) ? null : await userResolver.GetUserAsync(username);
                var optionsJson = await signInManager.MakePasskeyRequestOptionsAsync(user);
                return TypedResults.Content(optionsJson, contentType: "application/json");
            });

            return accountGroup;
        }
    }
}

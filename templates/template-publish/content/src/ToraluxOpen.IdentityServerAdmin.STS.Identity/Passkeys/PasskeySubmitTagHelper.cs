// Copyright (c) Duende Software. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// Modified by Jan Skoruba

#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace ToraluxOpen.IdentityServerAdmin.STS.Identity.Passkeys
{
    [HtmlTargetElement("passkey-submit")]
public class PasskeySubmitTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        [HtmlAttributeName("operation")]
        public PasskeyOperation Operation { get; set; }

        [HtmlAttributeName("name")]
        public string Name { get; set; } = null!;
        
        [HtmlAttributeName("email-name")]
        public string? EmailName { get; set; }

        public PasskeySubmitTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Get tokens and store the antiforgery cookie so that subsequent passkey fetch requests can be validated
            var tokens = _httpContextAccessor.HttpContext?.RequestServices
                .GetService<IAntiforgery>()?.GetAndStoreTokens(_httpContextAccessor.HttpContext);
            
            // Button is the main element we want to create, capture all attributes etc.
            var buttonAttributes = output.Attributes.Where(it => it.Name != "operation" && it.Name != "name" && it.Name != "email-name").ToList();
            var buttonContent = (await output.GetChildContentAsync(NullHtmlEncoder.Default))
                .GetContent(NullHtmlEncoder.Default);
            
            // Create the button
            using var htmlWriter = new StringWriter();
            htmlWriter.Write("<button type=\"submit\" name=\"__passkeySubmit\" ");
            foreach (var buttonAttribute in buttonAttributes)
            {
                buttonAttribute.WriteTo(htmlWriter, NullHtmlEncoder.Default);
                htmlWriter.Write(" ");
            }
            htmlWriter.Write(">");
            if (!string.IsNullOrEmpty(buttonContent))
            {
                htmlWriter.Write(buttonContent);
            }
            htmlWriter.Write("</button>");
            htmlWriter.WriteLine();
            
            // Create the passkey-submit web component element
            htmlWriter.Write("<passkey-submit ");
            htmlWriter.Write($"operation=\"{Operation}\" ");
            htmlWriter.Write($"name=\"{Name}\" ");
            htmlWriter.Write($"email-name=\"{EmailName ?? ""}\" ");
            htmlWriter.Write($"request-token-name=\"{tokens?.HeaderName ?? ""}\" ");
            htmlWriter.Write($"request-token-value=\"{tokens?.RequestToken ?? ""}\" ");
            var pathBase = _httpContextAccessor.HttpContext?.Request.PathBase.Value ?? string.Empty;
            htmlWriter.Write($"creation-options-url=\"{pathBase}/Identity/Account/PasskeyCreationOptions\" ");
            htmlWriter.Write($"request-options-url=\"{pathBase}/Identity/Account/PasskeyRequestOptions\" ");
            htmlWriter.Write(">");
            htmlWriter.Write("</passkey-submit>");
            
            // Emit the element
            output.TagName = null;
            output.Attributes.Clear();
            output.Content.Clear();
            output.Content.SetHtmlContent(htmlWriter.ToString());

            await base.ProcessAsync(context, output);
        }
    }
}

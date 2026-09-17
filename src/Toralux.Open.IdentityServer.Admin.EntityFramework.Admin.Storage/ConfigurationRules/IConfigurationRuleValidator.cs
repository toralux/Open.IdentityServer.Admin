// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Entities;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.ConfigurationRules;

public interface IConfigurationRuleValidator
{
    /// <summary>
    /// Validates using pre-loaded data to avoid N+1 queries
    /// </summary>
    List<ConfigurationIssueView> ValidateWithContext(ValidationContext context, string configuration, string messageTemplate, string fixDescriptionTemplate, ConfigurationIssueTypeView issueType);
}

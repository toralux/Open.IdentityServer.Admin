// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Skoruba.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Entities;

namespace Skoruba.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.ConfigurationRules;

public interface IConfigurationRuleValidatorFactory
{
    IConfigurationRuleValidator Create(ConfigurationRuleType ruleType);
}

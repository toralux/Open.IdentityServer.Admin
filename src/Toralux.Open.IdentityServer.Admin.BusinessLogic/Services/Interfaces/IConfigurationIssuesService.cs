// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Threading.Tasks;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces;

public interface IConfigurationIssuesService
{
    Task<List<ConfigurationIssueDto>> GetAllIssuesAsync();
    Task<ConfigurationIssuesPagedDto> GetIssuesAsync(ConfigurationIssuesFilterDto filter);
}
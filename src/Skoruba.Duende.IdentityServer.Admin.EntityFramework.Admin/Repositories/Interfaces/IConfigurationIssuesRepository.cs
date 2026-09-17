// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Threading.Tasks;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Dtos;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Entities;

namespace Skoruba.Open.IdentityServer.Admin.EntityFramework.Admin.Repositories.Interfaces;

public interface IConfigurationIssuesRepository
{
    Task<List<ConfigurationIssueView>> GetAllIssuesAsync();
    Task<ConfigurationIssuesPagedDto> GetIssuesAsync(ConfigurationIssuesFilterDto filter);
}
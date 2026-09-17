// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Dtos;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Entities;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Mappers;

public static class ConfigurationIssueMappers
{
    public static ConfigurationIssueDto ToDto(this ConfigurationIssueView issue)
    {
        return new ConfigurationIssueDto
        {
            ResourceType = issue.ResourceType,
            ResourceName = issue.ResourceName,
            ResourceId = issue.ResourceId,
            Message = issue.Message,
            FixDescription = issue.FixDescription,
            IssueType = issue.IssueType
        };
    }
}
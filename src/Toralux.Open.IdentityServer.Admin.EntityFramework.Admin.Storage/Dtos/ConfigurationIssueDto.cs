// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

#nullable enable

using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Entities;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Dtos;

public class ConfigurationIssueDto
{
    public ConfigurationResourceType ResourceType { get; set; }
    public string? ResourceName { get; set; }
    public int ResourceId { get; set; }
    public string? Message { get; set; }
    public string? FixDescription { get; set; }
    public ConfigurationIssueTypeView IssueType { get; set; }
}
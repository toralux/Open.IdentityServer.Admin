// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.ComponentModel.DataAnnotations;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Entities;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;

public class ConfigurationRuleDto
{
    public int Id { get; set; }
    [Required]
    public ConfigurationRuleType RuleType { get; set; }
    [Required]
    public ConfigurationResourceType ResourceType { get; set; }
    [Required]
    public ConfigurationIssueType IssueType { get; set; }
    public bool IsEnabled { get; set; }
    public string Configuration { get; set; }
    [Required]
    [MaxLength(500)]
    public string MessageTemplate { get; set; }
    [MaxLength(1000)]
    public string FixDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

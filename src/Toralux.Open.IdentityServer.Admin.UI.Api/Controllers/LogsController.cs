// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Log;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Toralux.Open.IdentityServer.Admin.UI.Api.Configuration.Constants;
using Toralux.Open.IdentityServer.Admin.UI.Api.ExceptionHandling;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class LogsController(IAuditLogService auditLogService) : ControllerBase
    {
        [HttpGet(nameof(AuditLog))]
        public async Task<ActionResult<AuditLogsDto>> AuditLog([FromQuery]AuditLogFilterDto filters)
        {
            var logs = await auditLogService.GetAsync(filters);

            return Ok(logs);
        }
    }
}
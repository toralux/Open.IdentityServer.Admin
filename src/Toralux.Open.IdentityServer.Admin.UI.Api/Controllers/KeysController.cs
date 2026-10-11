// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Toralux.Open.IdentityServer.Admin.UI.Api.Configuration.Constants;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Key;
using Toralux.Open.IdentityServer.Admin.UI.Api.ExceptionHandling;
using Toralux.Open.IdentityServer.Admin.UI.Api.Mappers;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class KeysController : ControllerBase
    {
        private readonly IKeyService _keyService;

        public KeysController(IKeyService keyService)
        {
            _keyService = keyService;
        }

        [HttpGet]
        public async Task<ActionResult<KeysApiDto>> Get(int page = 1, int pageSize = 10)
        {
            var keys = await _keyService.GetKeysAsync(page, pageSize);
            var keysApi = keys.ToKeysApiDto();

            return Ok(keysApi);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KeyApiDto>> Get(string id)
        {
            var key = await _keyService.GetKeyAsync(id);

            var keyApi = key.ToKeyApiDto();

            return Ok(keyApi);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            await _keyService.DeleteKeyAsync(id);

            return NoContent();
        }
    }
}
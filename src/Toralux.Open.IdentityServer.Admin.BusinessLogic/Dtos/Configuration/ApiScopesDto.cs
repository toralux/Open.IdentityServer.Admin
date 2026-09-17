// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration
{
	public class ApiScopesDto
	{
		public ApiScopesDto()
		{
			Scopes = new List<ApiScopeDto>();
		}
		
		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public List<ApiScopeDto> Scopes { get; set; }
	}
}
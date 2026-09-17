// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Constants
{
    /// <summary>
    /// Compatibility shim for Duende's DPoPTokenExpirationValidationMode
    /// (originally Duende.IdentityServer.Models.DPoPTokenExpirationValidationMode).
    /// Open.IdentityServer keeps Client.DPoPValidationMode as an unused int column for
    /// schema compatibility; this enum mirrors the original Duende member values so the
    /// admin UI dropdown and the int mappings keep writing identical column values.
    /// </summary>
    public enum DPoPTokenExpirationValidationMode
    {
        Custom = 0,
        Iat = 1,
        Nonce = 2,
        IatAndNonce = 3
    }
}

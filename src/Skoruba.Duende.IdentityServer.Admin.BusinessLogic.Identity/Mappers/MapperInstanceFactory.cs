// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers
{
    public static class MapperInstanceFactory
    {
        public static T CreateInstance<T>()
        {
            try
            {
                var instance = Activator.CreateInstance<T>();
                if (instance == null)
                {
                    throw new InvalidOperationException($"Cannot create an instance of {typeof(T).FullName}.");
                }

                return instance;
            }
            catch (Exception ex) when (ex is MissingMethodException or MemberAccessException)
            {
                throw new InvalidOperationException(
                    $"Cannot create an instance of {typeof(T).FullName}. " +
                    "Ensure the type has a public parameterless constructor.",
                    ex);
            }
        }
    }
}

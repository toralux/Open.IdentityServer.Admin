// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Riok.Mapperly.Abstractions;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Log;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Entities;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    internal static partial class LogMapper
    {
        [MapperIgnoreSource(nameof(Log.PropertiesXml))]
        [MapperIgnoreTarget(nameof(LogDto.PropertiesXml))]
        public static partial LogDto ToLogDto(Log source);

        [MapperIgnoreSource(nameof(LogDto.PropertiesXml))]
        [MapperIgnoreTarget(nameof(Log.PropertiesXml))]
        public static partial Log ToLog(LogDto source);

        public static partial AuditLogDto ToAuditLogDto(AuditLog source);
    }

    public static class LogMappers
    {
        public static LogDto ToModel(this Log log)
        {
            return log == null ? null : LogMapper.ToLogDto(log);
        }

        public static LogsDto ToModel(this PagedList<Log> logs)
        {
            if (logs == null) return null;

            return new LogsDto
            {
                TotalCount = logs.TotalCount,
                PageSize = logs.PageSize,
                Logs = logs.Data.Select(LogMapper.ToLogDto).ToList()
            };
        }

        public static AuditLogsDto ToModel<TAuditLog>(this PagedList<TAuditLog> auditLogs)
            where TAuditLog : AuditLog
        {
            if (auditLogs == null) return null;

            return new AuditLogsDto
            {
                TotalCount = auditLogs.TotalCount,
                PageSize = auditLogs.PageSize,
                Logs = auditLogs.Data.Select(x => LogMapper.ToAuditLogDto(x)).ToList()
            };
        }

        public static AuditLogDto ToModel(this AuditLog auditLog)
        {
            return auditLog == null ? null : LogMapper.ToAuditLogDto(auditLog);
        }

        public static Log ToEntity(this LogDto log)
        {
            return log == null ? null : LogMapper.ToLog(log);
        }
    }
}

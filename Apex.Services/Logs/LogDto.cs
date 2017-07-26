using System;
using Apex.Services.Extensions;

namespace Apex.Services.Logs
{
    public sealed class LogDto
    {
        public LogDto(
            int id,
            string application,
            DateTime logged,
            string level,
            string message,
            string logger,
            string callsite,
            string exception)
        {
            Id = id;
            Application = application;
            Logged = $"{logged.ToPrettyDate()}<div>{logged.ToDateTimeString()}</div>";
            Level = level;
            Message = message;
            Logger = logger;
            Callsite = callsite;
            Exception = exception;
        }

        public int Id { get; }

        public string Application { get; }

        public string Logged { get; }

        public string Level { get; }

        public string Message { get; }

        public string Logger { get; }

        public string Callsite { get; }

        public string Exception { get; }
    }
}

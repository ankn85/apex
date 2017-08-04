using Apex.Data.Entities.Logs;
using Apex.Services.Extensions;

namespace Apex.Services.Models.Logs
{
    public sealed class LogDto
    {
        public LogDto(Log entity)
        {
            Id = entity.Id;
            Application = entity.Application;
            Logged = $"{entity.Logged.ToPrettyDate()}<div>{entity.Logged.ToDateTimeString()}</div>";
            Level = entity.Level;
            Message = entity.Message;
            Logger = entity.Logger;
            Callsite = entity.Callsite;
            Exception = entity.Exception;
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

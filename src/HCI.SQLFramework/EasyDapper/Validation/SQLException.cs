using System;

namespace EasyDapper.Validation
{
    public class SQLException : Exception
    {
        public SQLException()
        {
        }

        public SQLException(string message) : base(message)
        {
        }

        public SQLException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SQLException(string message, string sql, object entity) : base(message)
        {
            SQL = sql;
            Entity = entity;
        }

        public string SQL { get; private set; }
        public object Entity { get; private set; }
        public override string Message => $"{base.Message ?? string.Empty} {Environment.NewLine} {SQL}";
    }
}

using System;

namespace HCI.SQLFramework.Validation
{
    public class SQLException : Exception
    {
        private readonly string _sql;
        private readonly object _entity;

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
            _sql = sql;
            _entity = entity;
        }

        public string SQL => _sql;
        public object Entity => _entity;
        public override string Message => $"{base.Message ?? string.Empty} {Environment.NewLine} {SQL}";
    }
}

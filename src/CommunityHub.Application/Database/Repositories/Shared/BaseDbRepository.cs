using System.Data;

namespace CommunityHub.Application.Database.Repositories.Shared;

public abstract class BaseDbRepository
{
    protected void AddParameter(IDbCommand command, string name, string? value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = (object?)value ?? DBNull.Value;
        dbParam.DbType = DbType.String;
        command.Parameters.Add(dbParam);
    }

    protected void AddParameter(IDbCommand command, string name, long value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = value;
        command.Parameters.Add(dbParam);
    }

    protected void AddParameter(IDbCommand command, string name, int value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = value;
        command.Parameters.Add(dbParam);
    }

    protected void AddParameter(IDbCommand command, string name, DateTime value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = DateOnly.FromDateTime(value);
        command.Parameters.Add(dbParam);
    }

    protected void AddParameter(IDbCommand command, string name, TimeSpan value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = value;
        dbParam.DbType = DbType.Time;
        command.Parameters.Add(dbParam);
    }

    protected void AddParameter(IDbCommand command, string name, DateTime? value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = value.HasValue ? (object)DateOnly.FromDateTime(value.Value) : DBNull.Value;
        command.Parameters.Add(dbParam);
    }
    protected void AddParameter(IDbCommand command, string name, bool value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = value;
        dbParam.DbType = DbType.Boolean;
        command.Parameters.Add(dbParam);
    }
    protected void AddParameter(IDbCommand command, string name, int? value)
    {
        IDbDataParameter dbParam = command.CreateParameter();
        dbParam.ParameterName = name;
        dbParam.Value = value.HasValue ? (object)value.Value : DBNull.Value;
        dbParam.DbType = DbType.Int32;
        command.Parameters.Add(dbParam);
    }
}
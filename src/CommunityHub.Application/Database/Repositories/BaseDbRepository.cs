using System.Data;

namespace CommunityHub.Application.Database.Repositories;

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
        dbParam.Value = value;
        dbParam.DbType = DbType.DateTime;
        command.Parameters.Add(dbParam);
    }
}
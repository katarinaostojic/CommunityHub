using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public abstract class BaseDbRepository
{
    protected void AddParameter(IDbCommand command, string name, string? value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = (object?)value ?? DBNull.Value;
        param.DbType = DbType.String;
        command.Parameters.Add(param);
    }

    protected void AddParameter(IDbCommand command, string name, long value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        command.Parameters.Add(param);
    }

    protected void AddParameter(IDbCommand command, string name, int value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        command.Parameters.Add(param);
    }

    protected void AddParameter(IDbCommand command, string name, DateTime value)
    {
        IDbDataParameter param = command.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        param.DbType = DbType.DateTime;
        command.Parameters.Add(param);
    }
}
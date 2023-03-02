using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace NetworkDriveMapperLibrary.Internal.DataAccess;

public class SqliteDataAccess : ISqliteDataAccess
{
    public string GetConnectionString(string name = "Default")
    {
        return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }

    public List<T> LoadData<T>(string connectionStringName, string storedProcedure)
    {
        string connectionString = GetConnectionString(connectionStringName);

        using (IDbConnection connection = new SQLiteConnection(connectionString))
        {
            var output = connection.Query<T>(storedProcedure, new DynamicParameters());

            return output.ToList();
        }
    }

    public void SaveData<T>(T model, string connectionStringName, string storedProcedure)
    {
        string connectionString = GetConnectionString(connectionStringName);

        using IDbConnection connection = new SQLiteConnection(connectionString);
        connection.Execute(storedProcedure, model);
    }
}
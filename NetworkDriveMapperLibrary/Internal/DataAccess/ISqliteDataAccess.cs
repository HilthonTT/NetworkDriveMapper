namespace NetworkDriveMapperLibrary.Internal.DataAccess
{
    public interface ISqliteDataAccess
    {
        string GetConnectionString(string name = "Default");
        List<T> LoadData<T>(string connectionStringName, string storedProcedure);
        void SaveData<T>(T model, string connectionStringName, string storedProcedure);
    }
}
namespace Queued.Adapter.Storage.Context
{
    internal static class DbSchema
    {
        public static void Setup(DbContext context)
        {
            CreateWorkTable(context);
        }

        private static void CreateWorkTable(DbContext context)
        {
            var conn = context.Connection;
            using var command = conn.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Work (
                    Id TEXT PRIMARY KEY,
                    Data TEXT,
                    RequestedAt TEXT,
                    CompletedAt TEXT,
                    Attempts INT
                )";
            command.ExecuteNonQuery();
        }
    }
}

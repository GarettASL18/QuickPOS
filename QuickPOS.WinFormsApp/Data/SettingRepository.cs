using System;
using Microsoft.Data.SqlClient;

namespace QuickPOS.Data
{
    public class SettingRepository : ISettingRepository
    {
        private readonly SqlConnectionFactory _factory;
        public SettingRepository(SqlConnectionFactory factory) => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        public string? Get(string key)
        {
            const string sql = "SELECT Value FROM dbo.Setting WHERE KeyName=@k;";
            using var cn = _factory.Create();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@k", key);
            cn.Open();
            var obj = cmd.ExecuteScalar();
            return obj == null || obj == DBNull.Value ? null : obj.ToString();
        }

        public void Set(string key, string value)
        {
            using var cn = _factory.Create();
            cn.Open();
            // Upsert simple
            using var tx = cn.BeginTransaction();
            try
            {
                using var cmd = new SqlCommand("IF EXISTS(SELECT 1 FROM dbo.Setting WHERE KeyName=@k) UPDATE dbo.Setting SET Value=@v WHERE KeyName=@k ELSE INSERT dbo.Setting(KeyName,Value) VALUES(@k,@v);", cn, tx);
                cmd.Parameters.AddWithValue("@k", key);
                cmd.Parameters.AddWithValue("@v", value);
                cmd.ExecuteNonQuery();
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}

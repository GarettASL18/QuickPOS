using Microsoft.Extensions.Configuration;
namespace QuickPOS
{
    public static class Config
    {
        private static readonly IConfigurationRoot configuration;
        private static decimal? _overrideImpuesto;

        static Config()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string ConnectionString => configuration.GetConnectionString("QuickPOS");

        public static decimal Impuesto
        {
            get
            {
                if (_overrideImpuesto.HasValue) return _overrideImpuesto.Value;
                // fallback to appsettings value
                var v = configuration.GetValue<decimal?>("Impuesto");
                return v ?? 0.15m;
            }
        }

        public static void SetImpuesto(decimal value) => _overrideImpuesto = value;

        // Optional: a method to clear override and reload appsettings (if you want)
        public static void ClearOverride() => _overrideImpuesto = null;
    }
}

using Microsoft.Extensions.Configuration;

namespace QuickPOS;

public static class Config
{
    private static readonly IConfigurationRoot configuration;

    static Config()
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static string ConnectionString =>
        configuration.GetConnectionString("QuickPOS");

    public static decimal Impuesto =>
        configuration.GetValue<decimal>("Impuesto");
}

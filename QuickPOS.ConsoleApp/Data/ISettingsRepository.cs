namespace QuickPOS.Data
{
    public interface ISettingRepository
    {
        string? Get(string key);
        void Set(string key, string value);
    }
}

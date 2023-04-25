namespace HRP.QServiceLib
{
    public interface ICheckSettings
    {
        string CheckConnStr(string title, string name);
        string CheckAppSetting(string title, string name);
    }
}

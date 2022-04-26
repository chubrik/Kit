namespace Chubrik.Kit
{
    public interface IDataClient
    {
        void PushToWrite(string path, string text);
    }
}

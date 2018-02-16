namespace Kit {
    public interface IDataClient {
        
        void PushToWrite(string path, string text, string targetDirectory = null);
    }
}

public interface IUniqueSaveDataProvider
{
    byte[] SaveState();
    void LoadState(byte[] data);
    string UniqueId { get; }
}
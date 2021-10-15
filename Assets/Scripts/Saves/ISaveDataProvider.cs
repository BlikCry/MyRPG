public interface ISaveDataProvider
{
    byte[] SaveState();
    void LoadState(byte[] data);
}
using Unity.Netcode;

struct PlayerData : INetworkSerializable
{
    public ulong Id;
    public ushort Length;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Length);
    }
}
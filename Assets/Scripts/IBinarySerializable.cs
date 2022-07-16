using System.IO;

public interface IBinarySerializable
{
    void DeSerialize(BinaryReader reader);
    void Serialize(BinaryWriter writer);
}

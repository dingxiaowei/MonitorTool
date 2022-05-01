using System.IO;

namespace MonitorLib.GOT
{
    public interface IBinarySerializable
    {
        void DeSerialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    };
}

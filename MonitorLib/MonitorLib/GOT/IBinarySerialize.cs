using System.IO;

namespace MonitorLib.GOT
{
    public interface IBinarySerialize
    {
        void DeSerialize(BinaryReader reader);
        void Serialize(BinaryWriter writer);
    };
}

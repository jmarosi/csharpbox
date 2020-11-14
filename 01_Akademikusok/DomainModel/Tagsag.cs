using Utilities;

namespace Akademikusok
{
    class Tagsag : DeserializerBase
    {
        public uint Id { get; set; }
        public uint TagId { get; set; }
        public string Forma { get; set; }
        public uint Ev { get; set; }
    }
}
using Utilities;

namespace Forint.DataModel
{
    class Erme : DeserializerBase
    {
        public uint ErmeId { get; set; }
        public uint Cimlet { get; set; }
        public double Tomeg { get; set; }
        public uint Darab { get; set; }
        public string Kiadas { get; set; }
        public string Bevonas { get; set; }
    }
}
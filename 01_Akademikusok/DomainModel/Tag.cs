using Utilities;

namespace Akademikusok
{
    public class Tag : DeserializerBase
    {
        public uint Id { get; set; }
        public string Nev { get; set; }
        public string Nem { get; set; }
        public string Szuletett { get; set; }
        public string Elhunyt { get; set; }
        public string Identitas { get; set; }

    }
}
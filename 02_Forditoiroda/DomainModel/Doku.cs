using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Forditoiroda.DomainModel
{
    class Doku : DeserializerBase
    {
        public uint Id { get; set; }
        public uint Terjedelem { get; set; }
        public string Szakterulet { get; set; }
        public uint NyelvId { get; set; }
        public ushort Munkido { get; set; }
    }
}

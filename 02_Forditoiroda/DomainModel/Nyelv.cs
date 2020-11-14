using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Forditoiroda.DomainModel
{
    class Nyelv : DeserializerBase
    {
        public uint Id { get; set; }
        public string FNyelv { get; set; }
        public string CNyelv { get; set; }
        public uint Egysegar { get; set; }
    }
}
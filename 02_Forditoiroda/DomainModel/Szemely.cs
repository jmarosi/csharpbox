using System;
using Utilities;

namespace Forditoiroda.DomainModel
{
    class Szemely : DeserializerBase
    {
        public uint Id { get; set; }
        public string Nev { get; set; }
        public int ElerhetoInt { get; set; }
        public bool Elerheto {
            get
            {
                switch (ElerhetoInt)
                {
                    case 0:
                        return true;
                    case -1:
                        return false;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
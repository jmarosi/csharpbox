using Forint.DataModel;
using System;
using System.Linq;
using Utilities;

namespace Forint
{
    class Program
    {
        static void Main(string[] args)
        {
            var AkodList = new Deserializer<Akod>().Deserialize(@".\Database\Akod.txt");
            var AnyagList = new Deserializer<Anyag>().Deserialize(@".\Database\Anyag.txt");
            var ErmeList = new Deserializer<Erme>().Deserialize(@".\Database\Erme.txt");
            var TervezoList = new Deserializer<Tervezo>().Deserialize(@".\Database\Tervezo.txt");
            var TkodList = new Deserializer<Tkod>().Deserialize(@".\Database\Tkod.txt");

            // Ezüst tartalmú érmék címlete és tömege
            var EzustId = AnyagList.Single(x => x.FemNev == "Ezüst").AnyagId; // REV: Ez mehetett volna a lenti Where-be is, ha már ott van (ld. utolsó feladatod megoldása)
            var x1 = ErmeList
                .Join(AkodList, e => e.ErmeId, a => a.ErmeId, (e, a) => new { e.Cimlet, e.Tomeg, a.FemId })
                .Where(x => x.FemId == EzustId)
                .Select(x => new { x.Cimlet, x.Tomeg });

            Console.WriteLine("\n---[ Ezüst tartalmú érmék címlete és tömege ]---");
            foreach (var x in x1)
                Console.WriteLine($"Címlet: {x.Cimlet,-3} Tömeg: {x.Tomeg}");


            // Ma forgalomba lévő érmék tervezői
            var x2 = ErmeList
                .Where(e => String.IsNullOrEmpty(e.Bevonas)) // még forgalomban lévő
                .Join(TkodList, e => e.ErmeId, tk => tk.ErmeId, (e, tk) => new { tk.TervezoId }) // tervező id kinyerése
                .Distinct() // ismétlődések kiszűrése
                .Join(TervezoList, tk => tk.TervezoId, t => t.TId, (tk, t) => new { t.Nev }); // név kinyerése

            Console.WriteLine("\n---[ Ma forgalomba lévő érmék tervezői ]---");
            foreach (var x in x2)
                Console.WriteLine($"- {x.Nev}");


            // bevontak közül a leghosszabb ideig forgalomban
            var x3 = ErmeList
                .Where(e => String.IsNullOrEmpty(e.Bevonas) == false) // bevont
                .GroupBy(e => DateTime.Parse(e.Bevonas).Year - DateTime.Parse(e.Kiadas).Year, (key, values) => new
                {
                    ActiveYears = key,
                    Erme = values.First() // ha több azonos lenne, akkro az elsőt vesszük
                }) // forgalomban levő évek szerint csoportosítva
                .OrderByDescending(x => x.ActiveYears) // leghosszabb elöl
                .First().Erme;

            Console.WriteLine("\n---[ Leghosszabb ideig forgalomban ]---");
            Console.WriteLine($"Címlet: {x3.Cimlet} Kiadás éve: {DateTime.Parse(x3.Kiadas).Year}");


            // A legnehezebb érméhez összesen mennyi fémet használtak
            // REV: Én itt valószínáleg rendeztem volna és vettem volna az elsőt a külön keresés helyett
            var maxWeight = ErmeList.Max(e => e.Tomeg);
            var x4 = ErmeList
                .Where(e => e.Tomeg == maxWeight)
                .Select(e => new { e.Cimlet, TotaWeight = e.Tomeg * e.Darab })
                .First(); // ha több azonos tömegű érme lenne, akkor ez elsőt vesszük
            Console.WriteLine("\n---[ A legnehezebb érméhez összesen mennyi fémet használtak ]--");
            Console.WriteLine($"Címlet: {x4.Cimlet} Teljes tömeg {x4.TotaWeight / 1000.0} [Kg]");


            // A művészek egyenként hány érme tervezésében vettek részt
            var x5 = ErmeList
                .Join(TkodList, e => e.ErmeId, tk => tk.ErmeId, (e, tk) => new { tk.TervezoId, e.ErmeId }) // TervezoId kinyerése
                .Join(TervezoList, x => x.TervezoId, t => t.TId, (x, t) => new { x.ErmeId, t.Nev }) // Név kinyerése
                .GroupBy(x => x.Nev, (key, values) => new { Designer = key, NbrOfCoins = values.Count() })
                .OrderByDescending(x => x.NbrOfCoins);

            Console.WriteLine("\n---[ A művészek egyenként hány érme tervezésében vettek részt ]---");
            foreach (var x in x5)
                Console.WriteLine($"{x.Designer,-25} - {x.NbrOfCoins} db");


            // Érmék 1990 és 1999 között
            var x6 = ErmeList
                .Where(e => DateTime.Parse(e.Kiadas) > DateTime.Parse("1990.01.01") && DateTime.Parse(e.Kiadas) < DateTime.Parse("1999.12.31"))
                .Join(TkodList, e => e.ErmeId, tk => tk.ErmeId, (e, tk) => new { e.Kiadas, e.Cimlet, tk.TervezoId }) // TervezoId kinyerése
                .Join(TervezoList, x => x.TervezoId, t => t.TId, (x, t) => new { x.Kiadas, x.Cimlet, t.Nev }) // Nev kinyerése
                .OrderBy(x => x.Nev)
                .ThenBy(x => x.Kiadas);

            Console.WriteLine("\n---[ Érmék 1990 és 1999 között ]---");
            foreach (var x in x6)
                Console.WriteLine($"Tervező: {x.Nev,-13} - Kiadás:{x.Kiadas} - Címlet: {x.Cimlet,-4}");


            // A 200Ft-os tervezője által kiadott egyéb érmék
            // REV: Valószínűleg itt ezek a szűrések is mehettek volna a Where-be
            var ErmeId = ErmeList.Single(e => e.Cimlet == 200).ErmeId;
            var TervezoId = TkodList.Single(tk => tk.ErmeId == ErmeId).TervezoId;
            var x7 = ErmeList
                .Join(TkodList, e => e.ErmeId, tk => tk.ErmeId, (e, tk) => new { e.Cimlet, e.Kiadas, tk.TervezoId })
                .Where(x => x.TervezoId == TervezoId && x.Cimlet != 200); // a 200-as már ne legyen benne

            Console.WriteLine("\n---[ A 200Ft-os tervezője által kiadott egyéb érmék ]---");
            foreach (var x in x7)
                Console.WriteLine($"Címlet: {x.Cimlet} Kiadás: {x.Kiadas}");


            // Nikkelt nem tartalmazó érmék
            var x8 = ErmeList
                .Join(AkodList, e => e.ErmeId, ak => ak.ErmeId, (e, ak) => new { e.Kiadas, e.Cimlet, ak.FemId })
                .Join(AnyagList, x => x.FemId, a => a.AnyagId, (x, a) => new { x.Kiadas, x.Cimlet, a.FemNev })
                .Where(x => x.FemNev.ToLower() != "nikkel");
            Console.WriteLine("\n--- [ Nikkelt nem tartalmazó érmék ]---");
            foreach (var x in x8)
                Console.WriteLine($"Címlet: {x.Cimlet,-3} Kiadás: {x.Kiadas}");


            Console.ReadLine();
        }
    }
}
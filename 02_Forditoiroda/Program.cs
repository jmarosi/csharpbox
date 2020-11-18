using Forditoiroda.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace Forditoiroda
{
    class Program
    {
        static void Main(string[] args)
        {
            var DokuList = new Deserializer<Doku>().Deserialize(@"Database\doku.txt");
            var ForditoList = new Deserializer<Fordito>().Deserialize(@"Database\fordito.txt");
            var NyelvList = new Deserializer<Nyelv>().Deserialize(@"Database\nyelv.txt");
            var SzemelyList = new Deserializer<Szemely>().Deserialize(@"Database\szemely.txt", new List<string> { "Elerheto" });


            // 1., <5000 karakter és összbevétel
            var smallDocs = DokuList.Where(d => d.Terjedelem <= 5000);
            var x1 = smallDocs
                .Join(NyelvList, 
                      doku => doku.NyelvId, 
                      nyelv => nyelv.Id, 
                      (doku, nyelv) => new { Price = nyelv.Egysegar })
                .Sum(d => d.Price);

            Console.WriteLine("\n---[ 1. feladat: <5000 és összbevétel ]---");
            Console.WriteLine($"Number of small docs: {smallDocs.Count()}, total price = {x1}");


            // 2., angolról magyarra terjedelem és szakterület
            var en2huId = NyelvList.Single(ny => ny.FNyelv == "angol" && ny.CNyelv == "magyar").Id;
            var x2 = DokuList
                .Where(d => d.NyelvId == en2huId)
                .Select(d => new { d.Terjedelem, d.Szakterulet })
                .OrderByDescending(d => d.Terjedelem);
            Console.WriteLine("\n---[ 2. feladat: Angolról magyarra, terjedelem és szakterület. Csökkenőben. ]---");
            foreach (var x in x2)
                Console.WriteLine(x);


            // 3., ~egy munkanapnyi feladatok, szakterület és forrásnyelv, abc sorrendben
            var x3 = DokuList
                .Where(d => d.Munkido >= 7 && d.Munkido <= 9)
                .Join(NyelvList, doku => doku.NyelvId, nyelv => nyelv.Id, (doku, nyelv) => new
                {
                    doku.Szakterulet,
                    nyelv.FNyelv,
                    nyelv.CNyelv
                })
                .OrderBy(d => d.FNyelv);

            Console.WriteLine("\n---[ 3. feladat: ~egy munkanapnyi feladatok, szakterület és forrásnyelv, abc sorrendben ]---");
            foreach (var x in x3)
            {
                var fc = $"{x.FNyelv}-{x.CNyelv}".PadRight(20);
                Console.WriteLine($"{fc} {x.Szakterulet,-20}");
            }


            // 4., magyarrol a legtöbb célnyelvre fordító
            // REV: Ez talán kicsit bonyolult lett :)
            // Step1: Nyelv Join Fordito Join Szemely
            // Step2: Ami marad, arra egy GroupBy() SzemelyId alapján, belsejébe transzformáció: Count(forrásnyelv==magyar); 
            // Step3: rendezés+first
            var x4 = ForditoList
                .GroupBy(fordito => fordito.SzemelyId)
                .SelectMany(g => g, (outer, inner) => new { TagId = outer.Key, inner.NyelvId })
                .Join(NyelvList, x => x.NyelvId, ny => ny.Id, (x, ny) => new { x.TagId, ny.FNyelv })
                .Where(x => x.FNyelv == "magyar")
                .GroupBy(x => x.TagId)
                .Select(g => new { TagId = g.Key, NumberOfPairs = g.Count() })
                .OrderByDescending(x => x.NumberOfPairs)
                .FirstOrDefault()?.TagId;

            Console.WriteLine("\n---[ 4. feladat: magyarrol a legtöbb célnyelvre fordító ]---");
            if (x4 != null)
                Console.Write($"{ SzemelyList.Single(x => x.Id == x4).Nev}");
            else
                Console.Write("Nem volt magyar nyelvről fordító ember.");


            Console.ReadLine();
        }
    }
}
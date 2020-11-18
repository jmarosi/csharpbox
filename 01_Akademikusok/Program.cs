using System;
using System.Linq;
using Utilities;

namespace Akademikusok
{
    class Program
    {
        static void Main(string[] args)
        {
            var tagList = new Deserializer<Tag>().Deserialize(@"Database\tag.txt");
            var tagsagList = new Deserializer<Tagsag>().Deserialize(@"Database\tagsag.txt");

            // 1., élő akadémikusok ABC sorrendben
            var x1 = tagList
                .Where(t => String.IsNullOrEmpty(t.Elhunyt))
                .OrderBy(t => t.Nev);

            Console.WriteLine("\n--- [1. élő akadémikusok ABC sorrendben ]---");
            foreach (var x in x1.Take(50))
                Console.WriteLine($"{x.Nev,-30} {x.Szuletett}-{x.Elhunyt}");


            // 2., élő rendes akadémikusok
            var x2 = tagList
                .Join(tagsagList, 
                       tag => tag.Id, 
                       tagsag => tagsag.TagId, 
                       (tag, tagsag) => new { tag.Nev, tag.Szuletett, tag.Elhunyt, tagsag.Forma }
                 )
                .Where(t => String.IsNullOrEmpty(t.Elhunyt) && t.Forma == "r") // élő és rendes
                .OrderBy(t => t.Nev);

            Console.WriteLine("\n--- [2. élő rendes akademikusok ]---");
            foreach (var x in x2.Take(50))
                Console.WriteLine($"{x.Nev,-30} '{x.Forma}' {x.Szuletett}-{x.Elhunyt}");


            // 3., kit mikor választottak meg először
            // REV: A GroupBy és a Join együtt árulkodó; ezekből általában lehet GroupJoin; azzal talán kicsit egyszerűbb lenne
            // Take(1) helyett inkább SingleOrDefault(); kifejezőbb és hatékonyabb is sokszor kicsit
            var x3 = tagsagList
                .OrderBy(t => t.Ev) // év alapján sorba
                .GroupBy(t => t.TagId) // csoportok id alapján (csoporton belül már év alapján rendezve)
                .SelectMany(g => g.Take(1), (g, v) => new { g.Key, v.Ev }) // a csoportból az első tagsag-ot kivesszük és annak az évét lekérjük
                .Join(tagList, e => e.Key, t => t.Id, (e, t) => new { Id = e.Key, t.Nev, ElsoMegvalsztasEve = e.Ev }) // id-ból nevet
                .OrderBy(x => x.Nev);

            Console.WriteLine("\n--- [3. kit mikor választottak meg először ]---");
            foreach (var x in x3.Take(50))
                Console.WriteLine($"{x.Nev,-30} első megválasztás: {x.ElsoMegvalsztasEve}");



            // 4., férfi/nő arány
            Console.WriteLine("\n--- [4. férfi/nő arány ]---");
            string percentage = String.Empty;
            var totalFemale = tagList.Count(t => t.Nem.ToLower() == "n");
            var totalMember = tagList.Count();
            percentage = totalMember > 0 ? $"{(double)totalFemale / totalMember:p3}" : "nem értelmezhető";
            Console.WriteLine($"Nők aránya: {percentage}");

            // 4., férfi/nő arány alternatív megoldás
            percentage = totalMember > 0 ? $"{tagList.Select(t => { return t.Nem == "n" ? 1 : 0; }).Average():p3}" : "nem értelmezhető";
            Console.WriteLine($"Nők aránya: {percentage}");


            // 5,. átlagos idő amíg levelzők voltak mielőtt rendes tagok lettek
            //-tagId alapján csoportosítani
            //  -szűrés azokra akik voltak levelezők és rendesek is
            //    -átlag(rendes.év - levelező.év)
            // 9.61839530332681
            // REV: A GroupBy és a Select mindig komibnálható a GroupBy overloadjában
            // REV: A where-t meg kellene próbálni előrébb tenni; hatékonyabb lenne
            // REV: Ha a GroupBy() és a Select() egybe lenne gyúrva a végén, akkor az Average()-t is bele lehetne gyúrni egy plusz paraméterrel
            var avgTime = tagsagList
                .GroupBy(x => x.TagId)
                .Select(g => new { TagId = g.Key, Tagsag = g })
                .Where(g => g.Tagsag.Any(x => x.Forma == "l") && g.Tagsag.Any(x => x.Forma == "r"))
                .Average(g => g.Tagsag.Single(x => x.Forma == "r").Ev - g.Tagsag.Single(x => x.Forma == "l").Ev);

            Console.WriteLine("\n--- [5. átlagos idő amíg levelzők voltak mielőtt rendes tagok lettek ]---");
            Console.WriteLine($"{avgTime:N3} év");


            // 6., XX. században megválasztott rendes tagok
            var x6 = tagsagList
                .Where(x => x.Forma == "r" && x.Ev >= 1900 && x.Ev <= 2000)
                .Join(tagList, tagsag => tagsag.Id, tag => tag.Id, (tagsag, tag) => new
                {
                    Name = tag.Nev,
                    YearOfBirth = tag.Szuletett,
                    YearOfDeath = tag.Elhunyt,
                    YearOfElection = tagsag.Ev
                });

            int longestName = x6.Max(x => x.Name.Length); // for the padding

            var x7 = x6
                .OrderBy(x => x.YearOfElection)
                .ThenBy(x => x.Name)
                .GroupBy(x => x.YearOfElection);

            Console.WriteLine("\n--- [6. XX. században megválasztott rendes tagok ]---");
            foreach (var yeargroup in x7.Take(10))
            {
                Console.WriteLine(yeargroup.Key);
                foreach (var member in yeargroup)
                {
                    Console.WriteLine(
                        $"      " +
                        $"{member.Name.PadRight(longestName)} " +
                        $"{member.YearOfBirth} - " +
                        $"{member.YearOfDeath}");
                }
                Console.WriteLine("\n");
            }


            Console.ReadLine();
        }
    }
}
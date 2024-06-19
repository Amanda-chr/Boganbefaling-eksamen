using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boganbefaling_eksamen;
using Newtonsoft.Json;

namespace Boganbefaling_eksamen
{
    public class ImporterJSON
    {
        private readonly string _sampleJsonFilePath;

        public ImporterJSON(string sampleJsonFIlePath)
        {
            _sampleJsonFilePath = sampleJsonFIlePath;
        }

        public List<Publikation> ImporterPublikationer()
        {
            StreamReader reader = new StreamReader(_sampleJsonFilePath);
            var json = reader.ReadToEnd();

            var jsonList = JsonConvert.DeserializeObject<List<dynamic>>(json);

            List<Publikation> publikationerFraJson = new List<Publikation>(); //importere json elementer, så de kan bruges som objekter
            foreach (var jsonPub in jsonList)
            {
                if (jsonPub.type == "Bog")
                {
                    publikationerFraJson.Add(new Bog
                    {
                        Type = jsonPub.type,
                        Titel = jsonPub.Titel,
                        Forfatter = jsonPub.Forfatter,
                        Genrer = jsonPub.Genrer.ToObject<List<string>>(),
                        PublikationsAar = jsonPub.PublikationsAar,
                        Udgiver = jsonPub.Udgiver,
                        AntalSider = jsonPub.AntalSider,
                        Kapitler = jsonPub.Kapitler
                    });
                }
                else if (jsonPub.type == "Lydbog")
                {
                    publikationerFraJson.Add(new Lydbog
                    {
                        Type = jsonPub.type,
                        Titel = jsonPub.Titel,
                        Forfatter = jsonPub.Forfatter,
                        Genrer = jsonPub.Genrer.ToObject<List<string>>(),
                        PublikationsAar = jsonPub.PublikationsAar,
                        Udgiver = jsonPub.Udgiver,
                        IndtaltAf = jsonPub.IndtaltAf,
                        LaengdeIMinutter = jsonPub.LaengdeIMinutter
                    });
                }
                else if (jsonPub.type == "Tegneserie")
                {
                    publikationerFraJson.Add(new Tegneserie
                    {
                        Type = jsonPub.type,
                        Titel = jsonPub.Titel,
                        Forfatter = jsonPub.Forfatter,
                        Genrer = jsonPub.Genrer.ToObject<List<string>>(),
                        PublikationsAar = jsonPub.PublikationsAar,
                        Udgiver = jsonPub.Udgiver,
                        AntalSider = jsonPub.AntalSider,
                        Illustrator = jsonPub.Illustrator,
                        Farve = jsonPub.Farve,
                        Sort_Hvid = jsonPub.Sort_Hvid
                    });
                }
            }

            return publikationerFraJson;
        }

        public List<string> HentGenrertilWPF()
        {
            List<Publikation> publikationer = ImporterPublikationer();
            return publikationer.SelectMany(p => p.Genrer).Distinct().ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    public class BogAnbefaling
    {
        private List<Publikation> allePublikationer;

        public BogAnbefaling(string jsonFilePath)
        {
            InitializeBooks(jsonFilePath);
        }

        private void InitializeBooks(string jsonFilePath)
        {
            ImporterJSON importer = new ImporterJSON(jsonFilePath);
            allePublikationer = importer.ImporterPublikationer();
        }

        public List<Publikation> AnbefalBoeger(List<string> brugerGenrer)
        {
            List<Publikation> anbefalede = new List<Publikation>();

            if (brugerGenrer.Count > 0)
            {
                foreach (Publikation pub in allePublikationer)
                {
                    double matchCount = 0;
                    foreach (var genre in brugerGenrer)
                    {
                        if (pub.Genrer.Contains(genre, StringComparer.OrdinalIgnoreCase))
                        {
                            matchCount++;
                        }
                    }

                    if (matchCount > 0) //viser bøgerne efter de titler hvor fleste genrer matcher
                    {
                        pub.MatchPercentage = matchCount / brugerGenrer.Count;
                        anbefalede.Add(pub);
                    }
                }

                anbefalede = anbefalede.OrderByDescending(p => p.MatchPercentage).ToList();
            }

            return anbefalede;
        }
    }

}

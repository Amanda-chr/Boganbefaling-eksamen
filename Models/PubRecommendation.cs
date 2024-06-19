using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    public class PubRecommendation
    {
        //Class needed for actually recommending books to the user
        //Publications from Json is initialized
        private List<Publication> allPublications;

        public PubRecommendation(string jsonFilePath)
        {
            InitializeBooks(jsonFilePath);
        }

        //Method that is loading the publications
        private void InitializeBooks(string jsonFilePath)
        {
            ImportJson import = new ImportJson(jsonFilePath);
            allPublications = import.LoadPublicationsFromJson();
        }

        //Method that checks the matches of the user input and the publications' genres 
        public List<Publication> RecommendPublication(List<string> userGenres)
        {
            List<Publication> recommended = new List<Publication>();

            if (userGenres.Count > 0)
            {
                foreach (Publication pub in allPublications)
                {
                    double matchCount = 0;
                    foreach (var genre in userGenres)
                    {
                        if (pub.Genres.Contains(genre, StringComparer.OrdinalIgnoreCase))
                        {
                            matchCount++;
                        }
                    }

                    if (matchCount > 0) 
                    {
                        pub.MatchPercentage = matchCount / userGenres.Count;
                        recommended.Add(pub);
                    }
                }

                recommended = recommended.OrderByDescending(p => p.MatchPercentage).ToList();
            }

            return recommended;
        }
    }

}

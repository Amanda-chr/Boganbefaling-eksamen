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
    public class ImportJson
    {
        private readonly string _sampleJsonFilePath;

        public ImportJson(string sampleJsonFilePath)
        {
            _sampleJsonFilePath = sampleJsonFilePath;
        }

        public List<Publication> LoadPublicationsFromJson()
        {
            StreamReader reader = new StreamReader(_sampleJsonFilePath);
            var json = reader.ReadToEnd();

            var jsonList = JsonConvert.DeserializeObject<List<dynamic>>(json);

            List<Publication> publicationsFromJson = new List<Publication>(); //List to store the objects from the Json file
            foreach (var jsonPub in jsonList)
            {
                if (jsonPub.type == "Book")
                {
                    publicationsFromJson.Add(new Book
                    {
                        Type = jsonPub.type,
                        Title = jsonPub.Title,
                        Author = jsonPub.Author,
                        Genres = jsonPub.Genre.ToObject<List<string>>(),
                        PublicationYear = jsonPub.PublicationYear,
                        Publisher = jsonPub.Publisher,
                        NumOfPages = jsonPub.NumOfPages,
                        Chapters = jsonPub.Chapters
                    });
                }
                else if (jsonPub.type == "Audiobook")
                {
                    publicationsFromJson.Add(new Audiobook
                    {
                        Type = jsonPub.type,
                        Title = jsonPub.Title,
                        Author = jsonPub.Author,
                        Genres = jsonPub.Genre.ToObject<List<string>>(),
                        PublicationYear = jsonPub.PublicationYear,
                        Publisher = jsonPub.Publisher,
                        Narrator = jsonPub.Narrator,
                        LengthInMinutes = jsonPub.LengthInMinutes
                    });
                }
                else if (jsonPub.type == "Comic")
                {
                    publicationsFromJson.Add(new Comic
                    {
                        Type = jsonPub.type,
                        Title = jsonPub.Title,
                        Author = jsonPub.Author,
                        Genres = jsonPub.Genre.ToObject<List<string>>(),
                        PublicationYear = jsonPub.PublicationYear,
                        Publisher = jsonPub.Publisher,
                        NumOfPages = jsonPub.NumOfPages,
                        Illustrator = jsonPub.Illustrator,
                        Color = jsonPub.Color,
                        Black_White = jsonPub.Black_White
                    });
                }
            }

            return publicationsFromJson;
        }

        public List<string> GetGenresToWPF()
        {
            List<Publication> publications = LoadPublicationsFromJson();
            return publications.SelectMany(p => p.Genres).Distinct().ToList();
        }
    }
}
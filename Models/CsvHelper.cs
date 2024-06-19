using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    public static class CsvHelper
    {
        public static void WriteSearchHistoryToCsv(string filePath, List<SearchStatistics.SearchHistoryEntry> searchHistoryData)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Date,TotalSearches,GenreCounts");

                foreach (var entry in searchHistoryData)
                {
                    string date = entry.Date.ToString("yyyy-MM-dd");
                    string totalSearches = entry.TotalSearches.ToString();
                    string genreCounts = string.Join(",", entry.GenreCounts.Select(kv => $"{kv.Key}:{kv.Value}"));

                    writer.WriteLine($"{date},{totalSearches},{genreCounts}");
                }
            }
        }
    }
}

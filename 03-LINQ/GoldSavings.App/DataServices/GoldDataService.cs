using GoldSavings.App.Client;
using GoldSavings.App.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GoldSavings.App.Services
{
    public class GoldDataService
    {
        private readonly GoldClient _goldClient;

        public GoldDataService()
        {
            _goldClient = new GoldClient();
        }

        public async Task<List<GoldPrice>> GetGoldPrices(DateTime startDate, DateTime endDate)
        {
            List<GoldPrice> allPrices = new List<GoldPrice>();
            DateTime currentStart = startDate;

            while (currentStart < endDate)
            {
                // Ensure the chunk does not exceed 93 days or the overall endDate
                DateTime currentEnd = currentStart.AddDays(93);
                if (currentEnd > endDate) currentEnd = endDate;

                // Call the client for this specific chunk
                var chunk = await _goldClient.GetGoldPrices(currentStart, currentEnd);

                if (chunk != null)
                {
                    allPrices.AddRange(chunk);
                }

                // Move to the next day after the current chunk end to avoid overlaps
                currentStart = currentEnd.AddDays(1);
            }

            return allPrices.OrderBy(p => p.Date).ToList();
        }
   
       
        public void SavePricesToXml(List<GoldPrice> prices, string filePath)
            {
                if (prices == null || prices.Count == 0)
                {
                    throw new ArgumentException("The price list is empty and cannot be saved.");
                }

                try
                {
                    string directory = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    XmlSerializer serializer = new XmlSerializer(typeof(List<GoldPrice>));

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        serializer.Serialize(writer, prices);
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException($"An error occurred while saving XML: {ex.Message}", ex);
                }
            }

        public List<GoldPrice> LoadPricesFromXml(string filePath) => (List<GoldPrice>)new XmlSerializer(typeof(List<GoldPrice>)).Deserialize(File.OpenRead(filePath));
    }
}


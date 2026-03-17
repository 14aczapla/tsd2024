using GoldSavings.App.Model;
using GoldSavings.App.Services;

using System;
using System.IO;
using GoldSavings.App.Model;
using GoldSavings.App.Client;
using GoldSavings.App.Services;
namespace GoldSavings.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Gold Investor!");

        // Step 1: Get gold prices
        GoldDataService dataService = new GoldDataService();
        DateTime startDate = new DateTime(2020, 01, 01);
        DateTime endDate = DateTime.Now;
        List<GoldPrice> goldPrices = dataService.GetGoldPrices(startDate, endDate).GetAwaiter().GetResult();

        if (goldPrices.Count == 0)
        {
            Console.WriteLine("No data found. Exiting.");
            return;
        }

        Console.WriteLine($"Retrieved {goldPrices.Count} records. Ready for analysis.");

        // Step 2: Perform analysis
        GoldAnalysisService analysisService = new GoldAnalysisService(goldPrices);
        var avgPrice = analysisService.GetAveragePrice();

        // Step 3: Print results
        GoldResultPrinter.PrintSingleValue(Math.Round(avgPrice, 2), "Average Gold Price Last Half Year");

        //2.a
        //query
        var top3HighestQuery = (from p in goldPrices
                                orderby p.Price descending
                                select p).Take(3).ToList();

        GoldResultPrinter.PrintPrices(top3HighestQuery, "Top 3 Highest Gold Prices");

        var top3LowestQuery = (from p in goldPrices
                               orderby p.Price ascending
                               select p).Take(3).ToList();

        GoldResultPrinter.PrintPrices(top3LowestQuery, "Top 3 Lowest Gold Prices");


        //method
        var top3HighestMethod = goldPrices
            .OrderByDescending(p => p.Price)
            .Take(3)
            .ToList();

        GoldResultPrinter.PrintPrices(top3HighestMethod, "Top 3 Highest Gold Prices");

        var top3LowestMethod = goldPrices
            .OrderBy(p => p.Price)
            .Take(3)
            .ToList();

        GoldResultPrinter.PrintPrices(top3LowestMethod, "Top 3 Lowest Gold Prices");

        //2.b
        var buyPrice = goldPrices
            .Where(p => p.Date.Year == 2020 && p.Date.Month == 1)
            .OrderBy(p => p.Date)
            .FirstOrDefault();

        if (buyPrice != null)
        {
            var profitableDays = goldPrices
                .Where(p => p.Date > buyPrice.Date && (p.Price - buyPrice.Price) / buyPrice.Price > 0.05)
                .ToList();

            Console.WriteLine($"Bought on {buyPrice.Date:yyyy-MM-dd} at {buyPrice.Price} PLN");
            GoldResultPrinter.PrintPrices(profitableDays, "Days with > 5% Gain");
        }

        //2.c
        var secondTenStart = goldPrices
            .OrderByDescending(p => p.Price)
            .Skip(10)                        
            .Take(3)                         
            .ToList();

        GoldResultPrinter.PrintPrices(secondTenStart, "Dates Opening the Second Ten (Positions 11-13)");

        //2.d
        var yearlyAverages = from p in goldPrices
                             where p.Date.Year == 2020 || p.Date.Year == 2023 || p.Date.Year == 2024
                             group p by p.Date.Year into yearGroup
                             select new
                             {
                                 Year = yearGroup.Key,
                                 AveragePrice = yearGroup.Average(p => p.Price)
                             };

        foreach (var item in yearlyAverages)
        {
            Console.WriteLine($"Year: {item.Year} | Average Price: {item.AveragePrice:F2} PLN");
        }

        //2.e
        var bestBuy = goldPrices
            .OrderBy(p => p.Price)
            .First();

        var bestSell = goldPrices
            .Where(p => p.Date > bestBuy.Date)
            .OrderByDescending(p => p.Price)
            .First();

        double profit = bestSell.Price - bestBuy.Price;
        double roi = (profit / bestBuy.Price) * 100;

        Console.WriteLine($"Buy Date:  {bestBuy.Date:yyyy-MM-dd} at {bestBuy.Price} PLN");
        Console.WriteLine($"Sell Date: {bestSell.Date:yyyy-MM-dd} at {bestSell.Price} PLN");
        Console.WriteLine($"Total ROI: {roi:F2}%");

        //3
        string outputDir = Path.Combine(Environment.CurrentDirectory, "data");
        string outputFile = Path.Combine(outputDir, "goldPrices.xml");
        dataService.SavePricesToXml(goldPrices, outputFile);


        //satisfactory
        //1
        Func<int, bool> isLeapYear = year => (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);

        int yearToCheck = DateTime.Now.Year;
        Console.WriteLine($"Is {yearToCheck} a leap year? {isLeapYear(yearToCheck)}");

        //2
        var randomGoldList = new RandomizedList<GoldPrice>();

        foreach (var price in top3HighestMethod)
        {
            randomGoldList.Add(price);
        }

        if (!randomGoldList.IsEmpty)
        {
            var randomPrice = randomGoldList.Get(2);
            Console.WriteLine($"Randomly retrieved price: {randomPrice.Price} PLN from date {randomPrice.Date:yyyy-MM-dd}");
        }

    }
}
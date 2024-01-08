using HtmlAgilityPack;
using ScrapingOpenFoodFacts;
using System;

namespace ScapingOpenFoodFacts
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new ScrappingHtml("https://world.openfoodfacts.org").RunJobAsync();
        }    
    }
}
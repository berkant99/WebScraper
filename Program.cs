using HtmlAgilityPack;
using Newtonsoft.Json;

namespace FlatRockTechTask
{
    public class Program
    {
        // defining a custom class to store the scraped data 
        public class Product
        {
            public string? ProductName { get; set; }
            public string? Price { get; set; }
            public double? Rating { get; set; }
        }

        public static void Main()
        {
            string filePath = "test.txt";

            try
            {
                // Load the HTML file content
                string html = File.ReadAllText(filePath);

                // Create a new HtmlDocument
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // creating the list that will keep the scraped data 
                var products = new List<Product>();

                // Find and extract the desired information using XPath selectors
                // var itemNodes = doc.DocumentNode.SelectNodes("//div[@class='item']");

                // Find and extract the desired information using CSS selectors
                var itemNodes = doc.DocumentNode.QuerySelectorAll(".item");

                if (itemNodes != null)
                {
                    foreach (var itemNode in itemNodes)
                    {
                        //CSS selectors
                        var productNameNode = itemNode.QuerySelector("figure img");
                        var priceNode = itemNode.QuerySelector(".price-display.formatted span").FirstChild;
                        var ratingString = itemNode.Attributes["rating"].Value;

                        //XPath selectors
                        // var productNameNode = itemNode.SelectSingleNode(".//figure//img");
                        // var priceNode = itemNode.SelectSingleNode(".//span[@class='price-display formatted']//span[@style='display: none']");
                        // string ratingString = itemNode.GetAttributeValue("rating", "0");

                        if (productNameNode != null && priceNode != null)
                        {
                            string productName = HtmlEntity.DeEntitize(productNameNode.Attributes["alt"].Value);
                            string price = priceNode.InnerText.Replace("$", "");
                            //second way:
                            // string price = priceNode.InnerText.Split("$")[1];

                            double normalizedRating;
                            if (double.TryParse(ratingString, out double rating))
                            {
                                normalizedRating = (rating > 5) ? Math.Round(rating / 10 * 5, 1) : rating;
                            }
                            else
                            {
                                normalizedRating = 0;
                            }

                            // Create a new Product object
                            Product product = new()
                            {
                                ProductName = productName,
                                Price = price,
                                Rating = normalizedRating
                            };

                            products.Add(product);
                        }
                        else
                        {
                            Console.WriteLine("No item nodes found in the HTML.");
                        }
                    }

                    // Convert the products list to a JSON array
                    string json = JsonConvert.SerializeObject(products, Formatting.Indented);
                    Console.WriteLine(json);
                }
                else
                {
                    Console.WriteLine("No item nodes found in the HTML.");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File '{filePath}' not found.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error occurred while reading the file: " + ex.Message);
            }
        }
    }
}
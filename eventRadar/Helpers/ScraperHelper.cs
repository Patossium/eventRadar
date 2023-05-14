using eventRadar.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using ScrapySharp.Network;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace eventRadar.Helpers
{
    [ExcludeFromCodeCoverage]
    public class ScraperHelper
    {
        private static ScrapingBrowser browser = new ScrapingBrowser();
        public static List<Category> GetCategories(string url, List<string> blackCategories, Website website)
        {
            var html = GetHtml(url);
            List<Category> categories = new List<Category>();
            var categoryLinks = html.OwnerDocument.QuerySelectorAll(website.CategoryLink);
            foreach (var link in categoryLinks)
            {
                string result = link.InnerText;
                result = Regex.Replace(result, @"^\s+|\s+$", "");
                string urlLink = link.Attributes["href"].Value;
                if (!urlLink.Contains(url))
                {
                    urlLink = url + urlLink;
                }
                Category category = new Category();
                category.Name = result;
                category.SourceUrl = urlLink;
                if (categories.IsNullOrEmpty() && !blackCategories.Contains(category.Name))
                {
                    categories.Add(category);
                }
                else
                {
                    if (categories.Find(x => x.Name == category.Name && x.SourceUrl == category.SourceUrl) == null && !blackCategories.Contains(category.Name))
                    {
                        categories.Add(category);
                    }
                }
            }
            return categories;
        }
        public static Location GetLocationInfo(string url, Website website)
        {
            Location temp = new Location();
            var htmlNode = GetHtml(url);

            var nodes = htmlNode.OwnerDocument.DocumentNode.SelectNodes(website.FullLocationPath);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == 0)
                {
                    temp.Address = nodes[i].InnerText;
                }
                if (i == 1)
                {
                    temp.City = nodes[i].InnerText;
                }
                if (i == 2)
                {
                    temp.Country = nodes[i].InnerText;
                }
            }
            return temp;
        }
        public static HtmlNode GetHtml(string url)
        {
            browser.Encoding = Encoding.UTF8;

            WebPage webPage = browser.NavigateToPage(new Uri(url));
            return webPage.Html;
        }
    }
}

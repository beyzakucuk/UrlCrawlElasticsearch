using BeyzaKucukOdev.Interfaces;
using BeyzaKucukOdev.Model;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Nest;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;

namespace BeyzaKucukOdev.Pages {
    public class IndexModel : PageModel {
        private readonly IEsHelper esHelper;

        public IndexModel(IEsHelper esHelp) {
            esHelper = esHelp;
        }
        public async void OnGet() {

            try {
                Uri siteUrl = new("https://www.sozcu.com.tr/");
                ChromeOptions options = new();
                ChromeDriver driver = new(options);
                HtmlDocument doc = new();
                esHelper.CreateNewsIndex();
                await GetDatas(driver, doc, GetUrls(driver, doc, siteUrl));
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
        private static List<string> GetUrls(ChromeDriver driver, HtmlDocument doc, Uri siteUrl) {
            List<string> urlList = new();
            driver.Navigate().GoToUrl(siteUrl);
            doc.LoadHtml(driver.PageSource);
            var nodes = doc.DocumentNode.SelectNodes(NodeConstants.A).Where(x => x.OuterHtml.Contains("https://www.sozcu.com.tr/") &&
                                                                    !x.OuterHtml.Contains(UrlConstants.NavLink) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.DropdownItem) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.CampaignFooter) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.Kvkk) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.Reklam) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.Secim2023) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.FinansKategori) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.GundemKategori) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.Arama) &&
                                                                    !x.OuterHtml.Contains(UrlConstants.Sozcu)&&
                                                                    !x.OuterHtml.Contains("https://www.sozcu.com.tr/2023/medya/"));

            foreach(var node in nodes) {
                string url = Regex.Match(node.OuterHtml, RegexConstants.TakeUrlFromHref).Groups[1].Value;
                urlList.Add(url);
            }
            urlList = urlList.Distinct().ToList();
            return urlList;
        }
        private async Task GetDatas(ChromeDriver driver, HtmlDocument doc, List<string> urlList) {
            var bulkDescriptor = new BulkDescriptor();
            int bulkId = 0;
            foreach (var url in urlList) {
                driver.Navigate().GoToUrl(url);
                doc.LoadHtml(driver.PageSource);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(NodeConstants.Script);

                if (!nodes.IsNullOrEmpty()) {
                    var jsonScripts = nodes.FirstOrDefault(x => x.OuterHtml.Contains(TagConstants.FullTextStart))?.OuterHtml;
                    if (!string.IsNullOrEmpty(jsonScripts)) {
                        Data data = new() {
                            Id = Guid.NewGuid().ToString(),
                            Url = url,
                            Title = Substring(jsonScripts, TagConstants.TitleStart, TagConstants.TitleStop),
                            FullText = Substring(jsonScripts, TagConstants.FullTextStart, TagConstants.FullTextStop),
                            DatePublished = Substring(jsonScripts, TagConstants.DatePublishedStart, TagConstants.DatePublishedStop),
                        };
                        bulkDescriptor.Index<Data>(d => d.Id(bulkId).Document(data));
                        bulkId ++;
                    }
                }
            }
            await  esHelper.AddAllNewsIndex(bulkDescriptor);
        }
        private static string Substring(string node, string startString, string stopString) {
            return node.Substring(node.IndexOf(startString) + startString.Length,
                                  node.IndexOf(stopString) - node.IndexOf(startString) - startString.Length);
        }
    }
}
using BeyzaKucukOdev.Interfaces;
using BeyzaKucukOdev.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace BeyzaKucukOdev.Pages {
    public class GetNews : PageModel {
        [BindProperty]
        public string? TextBoxValue { get; set; }
        public List<Data> AllNews { get; set; } = new();
        public List<Data> FilteredNews { get; set; } = new();
        private readonly IEsHelper esHelper;
        public GetNews(IEsHelper esHelp) {
            esHelper = esHelp;
        }
        public void OnGet() {
            try {
                AllNews.Clear();
                AllNews.AddRange(esHelper.GetAllNewsIndex());

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
        public void OnPost() {
            if (!string.IsNullOrEmpty(TextBoxValue)) {
                bool safeSearch = Regex.IsMatch(TextBoxValue, RegexConstants.SqlInjection);
                if (safeSearch) {
                    FilteredNews.Clear();
                    string text = TextBoxValue;
                    try {
                        FilteredNews.AddRange(esHelper.SearchNewsIndex(text));
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}

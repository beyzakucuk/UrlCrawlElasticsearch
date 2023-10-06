using BeyzaKucukOdev.Interfaces;
using BeyzaKucukOdev.Model;
using Nest;

namespace BeyzaKucukOdev.ElasticHelper {
    /// <summary>
    /// Elasticsearch connection and operations
    /// </summary>
    sealed class EsHelper : IEsHelper {
        private static readonly Uri node = new("http://localhost:9200");
        private static readonly ConnectionSettings connSettings = new(node);
        private static readonly ElasticClient client = new(connSettings);
        private static readonly string indexName = "news";
        /// <summary>
        /// Elsticsearch config connection settings
        /// </summary>
        public EsHelper() {
            string userName = "beyzaku";
            string password = "beyzaku";
            connSettings.DefaultIndex(indexName);
            connSettings.BasicAuthentication(userName, password);
            connSettings.EnableHttpCompression();
            connSettings.DefaultMappingFor<Data>(i => i
                       .IndexName(indexName))
                       .PrettyJson()
                       .RequestTimeout(TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Create index if index is not exists. If index is exists, clean documents in the index
        /// </summary>
        public void CreateNewsIndex() {
            if (!client.Indices.Exists(indexName).Exists) {
                string aliasName = "newnews";
                var createIndexDescriptor = new CreateIndexDescriptor(indexName)
                                                .Map<Data>(m => m.AutoMap().Properties(props => props
                                                                           .Text(text => text
                                                                               .Name(p => p.Id)
                                                                           )
                                                                           .Text(text => text
                                                                               .Name(p => p.Url)
                                                                           )
                                                                           .Text(text => text
                                                                               .Name(p => p.Title)
                                                                               .Analyzer("turkish_analyzer")
                                                                           )
                                                                           .Text(text => text
                                                                               .Name(p => p.FullText)
                                                                               .Analyzer("turkish_analyzer")
                                                                           )
                                                                           .Text(text => text
                                                                               .Name(p => p.DatePublished)
                                                                               .Analyzer("turkish_analyzer")
                                                                           )
                                                                       ))
                                              .Aliases(a => a.Alias(aliasName))
                                              .Settings(s => s
                                              .NumberOfShards(5)
                                              .NumberOfReplicas(1)
                                                 .Analysis(a => a
                                                     .TokenFilters(tf => tf
                                                         .Lowercase("turkish_lowercase", sa => sa
                                                             .Language("turkish")))
                                                     .Analyzers(aa => aa
                                                       .Custom("turkish_analyzer", ca => ca
                                                          .Tokenizer("standart")
                                                              .Filters("turkish_lowercase")))
                                                     .TokenFilters(tf => tf
                                                        .Lowercase("turkish_lowercase", lc => lc
                                                          .Language("turkish")))));
                client.Indices.Create(createIndexDescriptor);
            }
            else {
                var createIndexDescriptor = new DeleteIndexDescriptor(indexName);
                client.Indices.Delete(createIndexDescriptor);
            }
        }
        /// <summary>
        /// Search the news 
        /// </summary>
        /// <param name="searchText">Text that you look for</param>
        /// <returns>List of data that it contains the text</returns>
        public List<Data> SearchNewsIndex(string searchText) {
            var searchResponse = client.Search<Data>(s => s.Size(200).Query(q => q.Bool(b => b.Should(q => q
                                                                     .MultiMatch(mm => mm
                                                                     .Fields(f => f.Field(d => d.Title).Field(d => d.FullText))
                                                                     .Query(searchText).Type(TextQueryType.BoolPrefix)),sh => sh
                                                                     .Wildcard(w => w.Field(p => p.Title).Value("*" + searchText + "*")), sh => sh
                                                                     .Wildcard(w => w.Field(p => p.FullText).Value("*" + searchText + "*"))
                                                                     ))));
            return searchResponse.Documents.ToList();
        }
        /// <summary>
        /// Get all news
        /// </summary>
        /// <returns>List of data that it is all</returns>
        public List<Data> GetAllNewsIndex() {
            var searchResponse = client.Search<Data>(s => s.Size(300).Query(q => q.MatchAll())
                                                           .Fields(f => f.Field(d => d.Title).Field(d => d.DatePublished).Field(d => d.FullText)));
            return searchResponse.Documents.ToList();
        }
        /// <summary>
        /// Insert all documents at the same time async
        /// </summary>
        /// <param name="bulkDescriptor">Insert operations</param>
        /// <returns></returns>
        public async Task AddAllNewsIndex(BulkDescriptor bulkDescriptor) {
            await client.BulkAsync(bulkDescriptor);
        }
    }
}

using BeyzaKucukOdev.Model;
using Nest;

namespace BeyzaKucukOdev.Interfaces {
    /// <summary>
    /// Elasticsearch operations
    /// </summary>
    public interface IEsHelper {
        /// <summary>
        /// Create index if index is not exists. If index is exists, clean documents in the index
        /// </summary>
        public void CreateNewsIndex();
        /// <summary>
        /// Search the news 
        /// </summary>
        /// <param name="searchText">Text that you look for</param>
        /// <returns>List of data that it contains the text</returns>
        public List<Data> SearchNewsIndex(string searchText);
        /// <summary>
        /// Get all news
        /// </summary>
        /// <returns>List of data that it is all</returns>
        public List<Data> GetAllNewsIndex();
        /// <summary>
        /// Insert all documents at the same time async
        /// </summary>
        /// <param name="bulkDescriptor">Insert operations</param>
        /// <returns></returns>
        public Task AddAllNewsIndex(BulkDescriptor bulkDescriptor);
    }
}

namespace BeyzaKucukOdev.Model {
    /// <summary>
    /// Data manipulation for properties context
    /// </summary>
    public static class TagConstants {
        /// <summary>
        /// "\"headline\": \""
        /// </summary>
        public const string TitleStart = "\"headline\": \"";
        /// <summary>
        /// "\",\r\n    \"name\":"
        /// </summary>
        public const string TitleStop = "\",\r\n    \"name\":";
        /// <summary>
        /// "\"articleBody\": \""
        /// </summary>
        public const string FullTextStart = "\"articleBody\": \"";
        /// <summary>
        /// "\",\r\n    \"articleSection\":"
        /// </summary>
        public const string FullTextStop = "\",\r\n    \"articleSection\":";
        /// <summary>
        /// "\"datePublished\": \""
        /// </summary>
        public const string DatePublishedStart = "\"datePublished\": \"";
        /// <summary>
        /// "\",\r\n    \"dateCreated\":"
        /// </summary>
        public const string DatePublishedStop = "\",\r\n    \"dateCreated\":";
    }
}

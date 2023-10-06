namespace BeyzaKucukOdev.Model {
    /// <summary>
    /// Regex rules
    /// </summary>
    public static class RegexConstants {
        /// <summary>
        ///  @"href=""([^""]+)"""
        /// </summary>
        public const string TakeUrlFromHref = @"href=""([^""]+)""";
        /// <summary>
        /// @"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s]*$"
        /// </summary>
        public const string SqlInjection = @"^[a-zA-Z0-9ğüşıöçĞÜŞİÖÇ\s]*$";

    }
}

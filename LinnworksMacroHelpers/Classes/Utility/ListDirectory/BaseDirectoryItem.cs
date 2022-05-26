namespace LinnworksMacroHelpers.Classes.Utility
{
    public class BaseDirectoryItem
    {
        /// <summary>
        /// Item path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Item type
        /// </summary>
        public DirectoryItemType Type { get; set; }

        /// <summary>
        /// Directory Item type
        /// Specifies the item type when listing a directory
        /// </summary>
        public enum DirectoryItemType
        {
            Directory,
            File
        }
    }
}
namespace VaraniumSharp.FileTransferManager.Models
{
    /// <summary>
    /// DirectorySizeInfo
    /// </summary>
    internal sealed class DirectorySizeInfo
    {
        #region Public Methods

        public static DirectorySizeInfo operator +(DirectorySizeInfo s1, DirectorySizeInfo s2)
        {
            return new DirectorySizeInfo
            {
                DirectoryCount = s1.DirectoryCount + s2.DirectoryCount,
                FileCount = s1.FileCount + s2.FileCount,
                Size = s1.Size + s2.Size
            };
        }

        #endregion

        #region Variables

        /// <summary>
        /// Number of directories
        /// </summary>
        public long DirectoryCount;

        /// <summary>
        /// Number of files
        /// </summary>
        public long FileCount;

        /// <summary>
        /// Size of the directory
        /// </summary>
        public long Size;

        #endregion
    }
}
namespace VaraniumSharp.FileTransferManager.Enumerations
{
    /// <summary>
    /// Result for a file transfer
    /// </summary>
    public enum TransferResult
    {
        /// <summary>
        /// Indicates the file was transferred successfully
        /// </summary>
        Success, 
        
        /// <summary>
        /// Indicate that a failure occured during the file transfer
        /// </summary>
        Failed, 
        
        /// <summary>
        /// Indicate that the file transfer was cancelled
        /// </summary>
        Cancelled
    }
}
using System;
using VaraniumSharp.FileTransferManager.Enumerations;

namespace VaraniumSharp.FileTransferManager.Models
{
    /// <summary>
    /// Contains the details for tracking file transfer progress
    /// </summary>
    public class TransferProgress
    {
        #region Constructor

        /// <summary>
        /// Construct and populated
        /// </summary>
        /// <param name="startedTimestamp">Timestamp of when the transfer was started</param>
        /// <param name="bytesTransferred">The number of bytes that have been transferred</param>
        public TransferProgress(DateTime startedTimestamp, long bytesTransferred)
        {
            BytesTransferred = bytesTransferred;
            BytesPerSecond = BytesTransferred / DateTime.Now.Subtract(startedTimestamp).TotalSeconds;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The number of bytes transferred per second
        /// </summary>
        public double BytesPerSecond { get; }

        /// <summary>
        /// The number of bytes that has been transferred
        /// </summary>
        public long BytesTransferred { get; set; }

        /// <summary>
        /// Fraction of bytes transferred
        /// </summary>
        private double Fraction => BytesTransferred / (double)Total;

        /// <summary>
        /// The percentage of the transfer that has been completed
        /// </summary>
        public double Percentage => 100.0 * Fraction;

        /// <summary>
        /// The file being processed
        /// </summary>
        public string ProcessedFile { get; set; }

        /// <summary>
        /// The size of the stream
        /// </summary>
        public long StreamSize { get; set; }

        /// <summary>
        /// The total size of the file
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// The amount of bytes that has been transferred
        /// </summary>
        public long Transferred { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the amount of data transferred as a formatted string
        /// </summary>
        /// <param name="suffixStyle">Style of the suffix for display</param>
        /// <param name="decimalPlaces">Number of decimal places to output</param>
        /// <returns>Formatted string with bytes transferred</returns>
        public string GetBytesTransferredFormatted(SuffixStyle suffixStyle, int decimalPlaces)
        {
            return Helpers.ToSizeWithSuffix(BytesTransferred, suffixStyle, decimalPlaces);
        }

        /// <summary>
        /// Get the speed at which the data is being transferred
        /// </summary>
        /// <param name="suffixStyle">Style of the suffix for display</param>
        /// <param name="decimalPlaces">Number of decimal places to output</param>
        /// <returns>Formatted string for the transfer speed</returns>
        public string GetDataPerSecondFormatted(SuffixStyle suffixStyle, int decimalPlaces)
        {
            return $"{Helpers.ToSizeWithSuffix((long)BytesPerSecond, suffixStyle, decimalPlaces)}/sec";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Total: {Total}, BytesTransferred: {BytesTransferred}, Percentage: {Percentage}";
        }

        #endregion
    }
}

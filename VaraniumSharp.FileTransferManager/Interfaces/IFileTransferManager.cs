using System;
using System.Threading;
using System.Threading.Tasks;
using VaraniumSharp.FileTransferManager.Enumerations;
using VaraniumSharp.FileTransferManager.Models;

namespace VaraniumSharp.FileTransferManager.Interfaces
{
    /// <summary>
    /// Handle moving/copying of files with progress feedback
    /// </summary>
    public interface IFileTransferManager
    {
        #region Public Methods

        /// <summary>
        /// Copy a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to copy from</param>
        /// <param name="destination">Destination to copy to</param>
        /// <param name="progress">Action that returns copy progress</param>
        /// <param name="continueOnFailure">Indicate if copying should continue on failure</param>
        /// <param name="copyContentOfDirectory">Indicate if the contents of a directory should be copied</param>
        /// <returns>Result of the copy operation</returns>
        TransferResult CopyWithProgress(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, bool copyContentOfDirectory = false);

        /// <summary>
        /// Copy a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to copy from</param>
        /// <param name="destination">Destination to copy to</param>
        /// <param name="progress">Action that returns copy progress</param>
        /// <param name="continueOnFailure">Indicate if copying should continue on failure</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <param name="copyContentOfDirectory">Indicate if the contents of a directory should be copied</param>
        /// <returns>Result of the copy operation</returns>
        /// <exception cref="ArgumentException">Thrown if the path is not a directory or file or if it does not exist</exception>
        TransferResult CopyWithProgress(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, CancellationToken cancellationToken, bool copyContentOfDirectory = false);

        /// <summary>
        /// Copy a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to copy from</param>
        /// <param name="destination">Destination to copy to</param>
        /// <param name="progress">Action that returns copy progress</param>
        /// <param name="continueOnFailure">Indicate if copying should continue on failure</param>
        /// <param name="copyContentOfDirectory">Indicate if the contents of a directory should be copied</param>
        /// <returns>Result of the copy operation</returns>
        Task<TransferResult> CopyWithProgressAsync(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, bool copyContentOfDirectory = false);

        /// <summary>
        /// Copy a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to copy from</param>
        /// <param name="destination">Destination to copy to</param>
        /// <param name="progress">Action that returns copy progress</param>
        /// <param name="continueOnFailure">Indicate if copying should continue on failure</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <param name="copyContentOfDirectory">Indicate if the contents of a directory should be copied</param>
        /// <returns>Result of the copy operation</returns>
        Task<TransferResult> CopyWithProgressAsync(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, CancellationToken cancellationToken, bool copyContentOfDirectory = false);

        /// <summary>
        /// Move a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to move from</param>
        /// <param name="destination">Destination to move to</param>
        /// <param name="progress">Action that returns move progress</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <returns>Result of the move operation</returns>
        TransferResult MoveWithProgress(string source, string destination, Action<TransferProgress> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Move a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to move from</param>
        /// <param name="destination">Destination to move to</param>
        /// <param name="progress">Action that returns move progress</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <returns>Result of the move operation</returns>
        Task<TransferResult> MoveWithProgressAsync(string source, string destination, Action<TransferProgress> progress, CancellationToken cancellationToken);

        #endregion
    }
}
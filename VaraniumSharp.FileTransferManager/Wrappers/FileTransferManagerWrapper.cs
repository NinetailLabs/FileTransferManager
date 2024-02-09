using System;
using System.Threading;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;
using VaraniumSharp.FileTransferManager.Enumerations;
using VaraniumSharp.FileTransferManager.Interfaces;
using VaraniumSharp.FileTransferManager.Models;

namespace VaraniumSharp.FileTransferManager.Wrappers
{
    /// <summary>
    /// Wrapper for the <see cref="FileTransferManager"/> methods
    /// </summary>
    [AutomaticContainerRegistration(typeof(IFileTransferManager), ServiceReuse.Singleton)]
    public sealed class FileTransferManagerWrapper : IFileTransferManager
    {
        #region Public Methods

        /// <inheritdoc />
        public TransferResult CopyWithProgress(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, bool copyContentOfDirectory = false)
        {
            return FileTransferManager.CopyWithProgress(source, destination, progress, continueOnFailure, copyContentOfDirectory);
        }

        /// <inheritdoc />
        public TransferResult CopyWithProgress(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, CancellationToken cancellationToken, bool copyContentOfDirectory = false)
        {
            return FileTransferManager.CopyWithProgress(source, destination, progress, continueOnFailure, cancellationToken, copyContentOfDirectory);
        }

        /// <inheritdoc />
        public Task<TransferResult> CopyWithProgressAsync(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, bool copyContentOfDirectory = false)
        {
            return FileTransferManager.CopyWithProgressAsync(source, destination, progress, continueOnFailure, copyContentOfDirectory);
        }

        /// <inheritdoc />
        public Task<TransferResult> CopyWithProgressAsync(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, CancellationToken cancellationToken, bool copyContentOfDirectory = false)
        {
            return FileTransferManager.CopyWithProgressAsync(source, destination, progress, continueOnFailure, cancellationToken, copyContentOfDirectory);
        }

        /// <inheritdoc />
        public TransferResult MoveWithProgress(string source, string destination, Action<TransferProgress> progress, CancellationToken cancellationToken)
        {
            return FileTransferManager.MoveWithProgress(source, destination, progress, cancellationToken);
        }

        /// <inheritdoc />
        public Task<TransferResult> MoveWithProgressAsync(string source, string destination, Action<TransferProgress> progress, CancellationToken cancellationToken)
        {
            return FileTransferManager.MoveWithProgressAsync(source, destination, progress, cancellationToken);
        }

        #endregion
    }
}
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VaraniumSharp.FileTransferManager.Enumerations;
using VaraniumSharp.FileTransferManager.Models;

namespace VaraniumSharp.FileTransferManager
{
    /// <summary>
    /// Handle moving/copying of files with progress feedback
    /// </summary>
    public static class FileTransferManager
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
        public static TransferResult CopyWithProgress(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, bool copyContentOfDirectory = false)
        {
            return CopyWithProgress(source, destination, progress, continueOnFailure, CancellationToken.None, copyContentOfDirectory);
        }

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
        public static TransferResult CopyWithProgress(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, CancellationToken cancellationToken, bool copyContentOfDirectory = false)
        {
            var isDir = source.IsDirFile();
            if (isDir == null)
            {
                throw new ArgumentException("Source parameter has to be file or directory! " + source);
            }
            
            if (isDir == true)
            {
                return CopyDirectoryWithProgress(source, destination, progress, continueOnFailure, cancellationToken, copyContentOfDirectory);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return TransferResult.Cancelled;
            }

            var destinationFile = Helpers.CorrectFileDestinationPath(source, destination);

            return CopyFileWithProgress(source, destinationFile, progress, cancellationToken);
        }

        /// <summary>
        /// Copy a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to copy from</param>
        /// <param name="destination">Destination to copy to</param>
        /// <param name="progress">Action that returns copy progress</param>
        /// <param name="continueOnFailure">Indicate if copying should continue on failure</param>
        /// <param name="copyContentOfDirectory">Indicate if the contents of a directory should be copied</param>
        /// <returns>Result of the copy operation</returns>
        public static Task<TransferResult> CopyWithProgressAsync(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, bool copyContentOfDirectory = false)
        {
            return CopyWithProgressAsync(source, destination, progress, continueOnFailure, CancellationToken.None, copyContentOfDirectory);
        }

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
        public static Task<TransferResult> CopyWithProgressAsync(string source, string destination, Action<TransferProgress> progress, bool continueOnFailure, CancellationToken cancellationToken, bool copyContentOfDirectory = false)
        {
            return Task.Run(() =>
            {
                try
                {
                    return CopyWithProgress(source, destination, progress, continueOnFailure, cancellationToken, copyContentOfDirectory);
                }
                catch
                {
                    return TransferResult.Failed;
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Move a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to move from</param>
        /// <param name="destination">Destination to move to</param>
        /// <param name="progress">Action that returns move progress</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <returns>Result of the move operation</returns>
        public static TransferResult MoveWithProgress(string source, string destination, Action<TransferProgress> progress, CancellationToken cancellationToken)
        {
            var startTimestamp = DateTime.Now;
            NativeMethods.CopyProgressRoutine lpProgressRoutine = (size, transferred, streamSize, bytesTransferred, number, reason, file, destinationFile, data) =>
            {
                var fileProgress = new TransferProgress(startTimestamp, bytesTransferred)
                {
                    Total = size,
                    Transferred = transferred,
                    StreamSize = streamSize,
                    BytesTransferred = bytesTransferred,
                    ProcessedFile = source
                };
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return NativeMethods.CopyProgressResult.PROGRESS_CANCEL;
                    }
                    progress(fileProgress);
                    return NativeMethods.CopyProgressResult.PROGRESS_CONTINUE;
                }
                catch (Exception)
                {
                    return NativeMethods.CopyProgressResult.PROGRESS_STOP;
                }
            };

            if (cancellationToken.IsCancellationRequested)
            {
                return TransferResult.Cancelled;
            }

            if (!NativeMethods.MoveFileWithProgress(source, destination, lpProgressRoutine, IntPtr.Zero, NativeMethods.MoveFileFlags.MOVE_FILE_REPLACE_EXISTING | NativeMethods.MoveFileFlags.MOVE_FILE_COPY_ALLOWED | NativeMethods.MoveFileFlags.MOVE_FILE_WRITE_THROUGH))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return TransferResult.Cancelled;
                }
                return TransferResult.Failed;
            }

            return TransferResult.Success;
        }

        /// <summary>
        /// Move a file or folder to a new location
        /// </summary>
        /// <param name="source">Source to move from</param>
        /// <param name="destination">Destination to move to</param>
        /// <param name="progress">Action that returns move progress</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <returns>Result of the move operation</returns>
        public static Task<TransferResult> MoveWithProgressAsync(string source, string destination, Action<TransferProgress> progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var destinationPathCorrected = destination;
                if (source.IsDirFile() == false)
                {
                    destinationPathCorrected = Helpers.CorrectFileDestinationPath(source, destination);
                }
                return MoveWithProgress(source, destinationPathCorrected, progress, cancellationToken);
            }, cancellationToken);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Copy a directory to a new location
        /// </summary>
        /// <param name="sourceDirectory">Source directory to copy from</param>
        /// <param name="destinationDirectory">Destination directory to copy to</param>
        /// <param name="progress">Action that returns move progress</param>
        /// <param name="continueOnFailure">Indicate if copying should continue on failure</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <param name="copyContentOfDirectory">Indicate if the contents of a directory should be copied</param>
        /// <returns>Result of the copy operation</returns>
        private static TransferResult CopyDirectoryWithProgress(string sourceDirectory, string destinationDirectory, Action<TransferProgress> progress, bool continueOnFailure, CancellationToken cancellationToken, bool copyContentOfDirectory)
        {
            var rootSource = new DirectoryInfo(sourceDirectory.TrimEnd('\\'));
            var rootSourceLength = rootSource.FullName.Length;
            var rootSourceSize = Helpers.DirSize(rootSource);
            long totalTransferred = 0;

            try
            {
                var destinationNewRootDir = new DirectoryInfo(destinationDirectory.TrimEnd('\\'));
                if (!copyContentOfDirectory)
                {
                    destinationNewRootDir = Directory.CreateDirectory(Path.Combine(destinationDirectory, rootSource.Name));
                }

                foreach (var directory in rootSource.EnumerateDirectories("*", SearchOption.AllDirectories))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return TransferResult.Cancelled;
                    }
                    var newName = directory.FullName.Substring(rootSourceLength + 1);
                    Directory.CreateDirectory(Path.Combine(destinationNewRootDir.FullName, newName));
                }

                foreach (var file in rootSource.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return TransferResult.Cancelled;
                    }

                    var newName = file.FullName.Substring(rootSourceLength + 1);
                    var fileCopyStartTimestamp = DateTime.Now;
                    var transferred = totalTransferred;
                    var result = CopyFileWithProgress(file.FullName, Path.Combine(destinationNewRootDir.FullName, newName), (partialProgress) =>
                    {
                        var totalProgress = new TransferProgress(fileCopyStartTimestamp, partialProgress.BytesTransferred)
                        {
                            Total = rootSourceSize.Size,
                            Transferred = transferred + partialProgress.Transferred,
                            BytesTransferred = transferred + partialProgress.Transferred,
                            StreamSize = rootSourceSize.Size,
                            ProcessedFile = file.FullName
                        };
                        progress(totalProgress);
                    }, cancellationToken);

                    if (result == TransferResult.Failed && !continueOnFailure)
                    {
                        return TransferResult.Failed;
                    }

                    if (result == TransferResult.Cancelled)
                    {
                        return TransferResult.Cancelled;
                    }

                    totalTransferred += file.Length;
                }
            }
            catch (Exception)
            {
                return TransferResult.Failed;
            }
            return TransferResult.Success;
        }

        /// <summary>
        /// Copy a file to a new location
        /// </summary>
        /// <param name="sourceFile">Source file to copy</param>
        /// <param name="newFile">Destination file to copy to</param>
        /// <param name="progress">Action that returns file copy progress</param>
        /// <param name="cancellationToken">Token that can be used to cancel the copy operation</param>
        /// <returns>Result of the copy operation</returns>
        private static TransferResult CopyFileWithProgress(string sourceFile, string newFile, Action<TransferProgress> progress, CancellationToken cancellationToken)
        {
            var pbCancel = 0;
            var startTimestamp = DateTime.Now;

            NativeMethods.CopyProgressRoutine lpProgressRoutine = (size, transferred, streamSize, bytesTransferred, number, reason, file, destinationFile, data) =>
            {
                var fileProgress = new TransferProgress(startTimestamp, bytesTransferred)
                {
                    Total = size,
                    Transferred = transferred,
                    StreamSize = streamSize,
                    ProcessedFile = sourceFile
                };
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return NativeMethods.CopyProgressResult.PROGRESS_CANCEL;
                    }
                    progress(fileProgress);
                    return NativeMethods.CopyProgressResult.PROGRESS_CONTINUE;
                }
                catch (Exception)
                {
                    return NativeMethods.CopyProgressResult.PROGRESS_STOP;
                }
            };
            if (cancellationToken.IsCancellationRequested)
            {
                return TransferResult.Cancelled;
            }

            // TODO - Why is this done?
            var ctr = cancellationToken.Register(() => pbCancel = 1);

            var result = NativeMethods.CopyFileEx(sourceFile, newFile, lpProgressRoutine, IntPtr.Zero, ref pbCancel, NativeMethods.CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS);
            if (cancellationToken.IsCancellationRequested)
            {
                return TransferResult.Cancelled;
            }

            return result ? TransferResult.Success : TransferResult.Failed;
        }

        #endregion
    }
}

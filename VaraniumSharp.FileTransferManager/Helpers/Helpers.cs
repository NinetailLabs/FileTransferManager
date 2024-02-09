using System;
using System.IO;
using Microsoft.Extensions.Logging;
using VaraniumSharp.FileTransferManager.Enumerations;
using VaraniumSharp.FileTransferManager.Models;
using VaraniumSharp.Logging;

namespace VaraniumSharp.FileTransferManager
{
    /// <summary>
    /// Helper methods
    /// </summary>
    internal static class Helpers
    {
        #region Private Methods

        /// <summary>
        /// Corrects destination path for folder if provided destination is only directory not a full filename 
        /// </summary>
        /// <param name="source">Source path</param>
        /// <param name="destination">Destination path</param>
        /// <returns>Corrected destination if destination is a directory</returns>
        internal static string CorrectFileDestinationPath(string source, string destination)
        {
            var destinationFile = destination;
            if (destination.IsDirFile() == true)
            {
                destinationFile = Path.Combine(destination, Path.GetFileName(source));
            }
            return destinationFile;
        }

        /// <summary>
        /// Retrieve the size information for a directory
        /// </summary>
        /// <param name="directoryInfo">Directory info to retrieve size info for</param>
        /// <returns>Size information for a directory</returns>
        internal static DirectorySizeInfo DirSize(DirectoryInfo directoryInfo)
        {
            var size = new DirectorySizeInfo();

            try
            {
                // Add file sizes.
                var fis = directoryInfo.GetFiles();
                foreach (var fi in fis)
                {
                    size.Size += fi.Length;
                }
                size.FileCount += fis.Length;

                // Add subdirectory sizes.
                var dis = directoryInfo.GetDirectories();
                size.DirectoryCount += dis.Length;
                foreach (var di in dis)
                {
                    size += DirSize(di);
                }
            }
            catch (Exception exception)
            {
                var logger = StaticLogger.LoggerFactory.CreateLogger(nameof(Helpers));
                logger.LogError(exception, "An error occured while retrieving DirSize");
            }

            return size;
        }

        /// <summary>
        /// Retrieve the suffix for a specific style
        /// </summary>
        /// <param name="style">Suffix style to retrieve entry for</param>
        /// <param name="index">Index of the entry to retrieve</param>
        /// <returns>Appropriate suffix for the style</returns>
        private static string GetSuffixAtIndex(SuffixStyle style, int index)
        {
            switch (style)
            {
                case SuffixStyle.Binary:
                    return SizeBinarySuffixes[index];
                case SuffixStyle.Metric:
                    return SizeMetricSuffixes[index];
                case SuffixStyle.Windows:
                    return SizeWindowsSuffixes[index];
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Check if a file path is a directory, file or neither
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>Returns true if the path is a dir, false if it's a file and null if it's neither or doesn't exist.</returns>
        internal static bool? IsDirFile(this string path)
        {
            bool? result = null; 
            if (Directory.Exists(path) || File.Exists(path))
            {
                // get the file attributes for file or directory 
                var fileAttr = File.GetAttributes(path);
                result = fileAttr.HasFlag(FileAttributes.Directory);
            }
            return result;
        }

        /// <summary>
        /// Convert a byte size to the appropriate style
        /// </summary>
        /// <param name="value">Bytes to convert</param>
        /// <param name="style">Style of the output</param>
        /// <param name="decimalPlaces">Number of decimal places in the output</param>
        /// <returns>Formatted byte value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="decimalPlaces"/> is less than 0</exception>
        internal static string ToSizeWithSuffix(long value, SuffixStyle style, int decimalPlaces = 1)
        {
            var newBase = 1024;
            if (style == SuffixStyle.Metric)
            {
                newBase = 1000;
            }

            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
            }

            if (value == 0)
            {
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
            }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int)Math.Log(value, newBase);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            if (style == SuffixStyle.Metric)
            {
                adjustedSize = value / (decimal)Math.Pow(newBase, mag);
            }

            // make adjustment when the value is large enough that
            // it would round up to higher magnitude
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= newBase;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                                adjustedSize,
                                GetSuffixAtIndex(style, mag));
        }

        #endregion

        #region Variables

        // 1 KB = 1024 bytes
        private static readonly string[] SizeWindowsSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        // 1 KiB = 1024 bytes
        private static readonly string[] SizeBinarySuffixes = { "bytes", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

        // 1 kB = 1000 bytes
        private static readonly string[] SizeMetricSuffixes = { "bytes", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        #endregion
    }
}

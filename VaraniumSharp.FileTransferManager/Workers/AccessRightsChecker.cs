using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace VaraniumSharp.FileTransferManager
{
    /// <summary>
    /// Checks the access rights for a file
    /// </summary>
    public static class AccessRightsChecker
    {
        #region Public Methods

        /// <summary>
        /// Test a directory or file for file access permissions
        /// </summary>
        /// <param name="itemPath">Full path to file or directory </param>
        /// <param name="accessRight">File System right tested</param>
        /// <returns>State [bool]</returns>
        public static bool ItemHasPermission(string itemPath, FileSystemRights accessRight)
        {
            if (string.IsNullOrEmpty(itemPath))
            {
                return false;
            }

            var isDir = itemPath.IsDirFile();
            if (isDir == null)
            {
                return false;
            }

            try
            {
                AuthorizationRuleCollection rules;
                if (isDir == true)
                {
                    var dirInfo = new DirectoryInfo(itemPath);
                    rules = dirInfo.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
                }
                else
                {
                    var fileInfo = new FileInfo(itemPath);
                    rules = fileInfo.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier));
                }

                var identity = WindowsIdentity.GetCurrent();
                var userSid = identity.User?.Value ?? "Unknown";

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (rule.IdentityReference.ToString() == userSid || (identity.Groups?.Contains(rule.IdentityReference) ?? false))
                    {
                        if ((accessRight & rule.FileSystemRights) == accessRight)
                        {
                            if (rule.AccessControlType == AccessControlType.Deny)
                            {
                                return false;
                            }

                            if (rule.AccessControlType == AccessControlType.Allow)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            // TODO - Should not swallow exceptions, though maybe this was done on purpose?
            catch { }
            return false;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SplayCode.Data
{
    /// <summary>
    /// Utility class that provides methods for adding files into the virtual space. 
    /// </summary>
    class ImportManager
    {
        private static ImportManager instance;
        public static ImportManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImportManager();
                }
                return instance;
            }
        }

        private ImportManager()
        {
        }

        public string GetFileName(string filePath)
        {
            Uri pathUri = new Uri(filePath);
            return (pathUri.Segments[pathUri.Segments.Length - 1]);
        }

        /// <summary>
        /// Call this function to add a file or multiple files from a directory into the virtual space.
        /// </summary>
        /// <param name="filePath">Absolute path of the file/directory to add.</param>
        /// <param name="cursorPosition">The point in virtual space at which to place the file. If null, places it at the
        /// default position.</param>
        /// <param name="doNotResetBlockCounter">Choose not to reset the block position counter (for drag-and-dropping 
        /// multiple files only).</param>
        public void AddSingleOrMultipleFiles(string filePath, Point? cursorPosition, bool doNotResetBlockCounter)
        {
            FileAttributes attr = File.GetAttributes(filePath);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                string[] extensions = { ".cs", ".xml", ".xaml", ".html", ".css", ".cpp", ".c", ".js", ".json", ".php", ".py", ".ts", ".txt", ".snk", ".config", ".vsixmanifest", ".vsct", ".resx", ".java", ".sln", ".md", ".gitignore", ".csproj", ".user", ".manifest", ".cache", ".resources", ".pkgdef", ".pdb", ".lref", ".tlog", ".vsix" };
                var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

                string[] files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories).ToArray();
                foreach (string s in files)
                {
                    if (allowedExtensions.Contains(Path.GetExtension(s)))
                    {
                        if (HandleDuplicateFiles(s))
                        {
                            if (cursorPosition == null)
                            {
                                BlockManager.Instance.AddBlock(GetFileName(s), s);
                            }
                            else
                            {
                                BlockManager.Instance.AddBlock(GetFileName(s), s, cursorPosition.Value.X, cursorPosition.Value.Y);
                            }
                        }
                    } else
                    {
                        MessageBox.Show("\"" + GetFileName(s) + "\"" + " cannot be open in SplayCode. " + "\"" + Path.GetExtension(s) + "\" is not supported.", "Unspported file type", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                string[] extensions = { ".cs", ".xml", ".xaml", ".html", ".css", ".cpp", ".c", ".js", ".json", ".php", ".py", ".ts", ".txt", ".snk", ".config", ".vsixmanifest", ".vsct", ".resx", ".java", ".sln", ".md", ".gitignore", ".csproj", ".user", ".manifest", ".cache", ".resources", ".pkgdef", ".pdb", ".lref", ".tlog", ".vsix"};
                var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
                if (allowedExtensions.Contains(Path.GetExtension(filePath)))
                {
                    if (HandleDuplicateFiles(filePath))
                    {
                        if (cursorPosition == null)
                        {
                            BlockManager.Instance.AddBlock(GetFileName(filePath), filePath);
                        }
                        else
                        {
                            BlockManager.Instance.AddBlock(GetFileName(filePath), filePath, cursorPosition.Value.X,
                                cursorPosition.Value.Y);
                        }
                    }
                } else
                {
                    MessageBox.Show("\"" + GetFileName(filePath) + "\"" + " cannot be open in SplayCode. " + "\"" + Path.GetExtension(filePath) + "\" is not supported.", "Unspported file type", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            // resets the placement position if it's a drag-n-drop operation
            if (cursorPosition != null && !doNotResetBlockCounter)
            {
                VirtualSpaceControl.Instance.ResetMultipleBlockCounter();
            }
        }

        public bool HandleDuplicateFiles(string filePath)
        {
            MessageBoxResult res = new MessageBoxResult();

            if (BlockManager.Instance.BlockAlreadyExists(filePath))
            {
                res = MessageBox.Show("\"" + GetFileName(filePath) + "\" is already added in the layout. Proceed with adding the file?",
                      "Duplicate file", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            if (res == MessageBoxResult.No)
            {
                return false;
            }
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SplayCode.Data
{
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

        public void AddSingleOrMultipleFiles(string filePath/*, Point cursorPosition*/)
        {
            // TODO need to check the nature of the string eg. directory/file/multiple/invalid etc
            FileAttributes attr = File.GetAttributes(filePath);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                string[] extensions = { ".cs", ".xml", ".xaml", ".html", ".css", ".cpp", ".c", ".js", ".json", ".php", ".py", ".ts", ".txt" };
                var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

                string[] files = Directory.EnumerateFiles(filePath, "*.*", SearchOption.AllDirectories).ToArray();
                foreach (string s in files)
                {
                    if (allowedExtensions.Contains(Path.GetExtension(s)))
                    {
                        if (HandleDuplicateFiles(s))
                        {
                            BlockManager.Instance.AddBlock(GetFileName(s), s);
                        }
                    }
                }
            }
            else
            {
                if (HandleDuplicateFiles(filePath))
                {
                    BlockManager.Instance.AddBlock(GetFileName(filePath), filePath);
                }
            }
        }

        public bool HandleDuplicateFiles(string filePath)
        {
            MessageBoxResult res = new MessageBoxResult();

            if (BlockManager.Instance.BlockAlreadyExists(filePath))
            {
                res = MessageBox.Show("The file is already added in the layout. Proceed with adding the file?",
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

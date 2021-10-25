/// Written by: Yulia Danilova
/// Creation Date: 28th of June, 2021
/// Purpose: Model for files and directories displayed in custom folder/file browser dialogs

using System;
using Devonia.Infrastructure.Enums;

namespace Devonia.ViewModels.Common.Models
{
    public class FileSystemEntity 
    {
        #region =============================================================== PROPERTIES ==================================================================================
        public FileSystemItemTypes FileSystemItemType { get; set; }
        public int VirtualId { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string IconSource { get; set; }
        public bool IsSelected { get; set; }
        #endregion
    }
}

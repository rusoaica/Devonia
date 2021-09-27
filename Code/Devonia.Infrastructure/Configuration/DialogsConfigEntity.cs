/// Written by: Yulia Danilova
/// Creation Date: 14th of May, 2021
/// Purpose: Model for strongly typed dialogs configuration values
#region ========================================================================= USING =====================================================================================
using System.Collections.Generic;
#endregion

namespace Devonia.Infrastructure.Configuration
{
    public class DialogsConfigEntity
    {
        #region =============================================================== PROPERTIES ==================================================================================
        public string LastDirectory { get; set; }
        public double DialogsWidth { get; set; }
        public double DialogsHeight { get; set; }
        public double PreviewPanelWidth { get; set; }
        public double NavigationPanelWidth { get; set; }
        public double DirectoriesPanelWidth { get; set; }
        public List<string> FavoritePaths { get; set; }
        public int DialogNavigationFilterSelectedIndex { get; set; }
        #endregion
    }
}

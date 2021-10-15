/// Written by: Yulia Danilova
/// Creation Date: 14th of May, 2021
/// Purpose: Model for strongly typed dialogs configuration values
#region ========================================================================= USING =====================================================================================
using System.Collections.Generic;
#endregion

namespace Devonia.Infrastructure.Configuration
{
    public class ExplorerConfigEntity
    {
        #region =============================================================== PROPERTIES ==================================================================================
        public string LastDirectory { get; set; }
        public double PreviewPanelWidth { get; set; }
        public double NavigationPanelWidth { get; set; }
        public double DirectoriesPanelWidth { get; set; }
        public List<string> FavoritePaths { get; set; }
        public string ItemsSelectionBackgroundColor { get; set; }
        public string ItemsSelectionBorderColor { get; set; }
        public string ItemsSelectionForegroundColor { get; set; }
        public string ItemsBackgroundColorFirst { get; set; }
        public string ItemsBackgroundColorSecond { get; set; }
        public string ItemsBorderColorFirst { get; set; }
        public string ItemsBorderColorSecond { get; set; }
        public string ItemsForegroundColor { get; set; }
        public string ItemsFont { get; set; }
        public int ItemsFontSize { get; set; }
        public int ItemsFontStyle { get; set; }
        public int ItemsFontWeight { get; set; }
        public int DialogNavigationFilterSelectedIndex { get; set; }
        #endregion
    }
}

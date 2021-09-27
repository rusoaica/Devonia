/// Written by: Yulia Danilova
/// Creation Date: 14th of May, 2021
/// Purpose: Model for strongly typed application configuration values

namespace Devonia.Infrastructure.Configuration
{
    public class ApplicationConfigEntity
    {
        #region =============================================================== PROPERTIES ==================================================================================
        public int? MainWindowWidth { get; set; }
        public int? MainWindowHeight { get; set; }
        public int? MainWindowPositionX { get; set; }
        public int? MainWindowPositionY { get; set; }
        public int? StartupWindowPositionX { get; set; }
        public int? StartupWindowPositionY { get; set; }
        #endregion
    }
}

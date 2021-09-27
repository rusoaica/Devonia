/// Written by: Yulia Danilova
/// Creation Date: 05th of November, 2019
/// Purpose: Custom generic model with properties for the displayed member, hidden value and mouse hover tooltip text

namespace Devonia.Models.Common.Models.Common
{
    public class SearchEntity
    {
        #region ================================================================ PROPERTIES =================================================================================
        public string Text { get; set; }
        public string MediaItemPath { get; set; }
        public object Value { get; set; }
        public object Hover { get; set; }
        public string[] Tags { get; set; }
        public string[] Actors { get; set; }
        public string[] Genres { get; set; }
        #endregion

        #region ================================================================= METHODS ===================================================================================
        /// <summary>
        /// Customized ToString() method
        /// </summary>
        /// <returns>Custom string value showing relevant data for current class</returns>
        public override string ToString()
        {
            return Text;
        }
        #endregion
    }
}

//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    /// Defines the class of commonly used file filters.
    /// </summary>
    public static class CommonFileDialogStandardFilters
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private static CommonFileDialogFilter officeFilesFilter;

        /// <summary>
        /// </summary>
        private static CommonFileDialogFilter pictureFilesFilter;

        /// <summary>
        /// </summary>
        private static CommonFileDialogFilter textFilesFilter;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value that specifies the filter for Microsoft Office files.
        /// </summary>
        public static CommonFileDialogFilter OfficeFiles
        {
            get
            {
                return officeFilesFilter ?? (officeFilesFilter = new CommonFileDialogFilter("Office Files", "*.doc, *.docx, *.xls, *.xlsx, *.ppt, *.pptx"));
            }
        }

        /// <summary>
        ///   Gets a value that specifies the filter for picture files.
        /// </summary>
        public static CommonFileDialogFilter PictureFiles
        {
            get
            {
                return pictureFilesFilter ?? (pictureFilesFilter = new CommonFileDialogFilter("All Picture Files", "*.bmp, *.jpg, *.jpeg, *.png, *.ico"));
            }
        }

        /// <summary>
        ///   Gets a value that specifies the filter for *.txt files.
        /// </summary>
        public static CommonFileDialogFilter TextFiles
        {
            get
            {
                return textFilesFilter ?? (textFilesFilter = new CommonFileDialogFilter("Text Files", "*.txt"));
            }
        }

        #endregion
    }
}
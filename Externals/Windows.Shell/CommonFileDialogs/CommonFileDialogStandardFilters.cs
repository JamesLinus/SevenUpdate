//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.
namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Defines the class of commonly used file filters.
    /// </summary>
    public static class CommonFileDialogStandardFilters
    {
        private static CommonFileDialogFilter textFilesFilter;

        private static CommonFileDialogFilter pictureFilesFilter;

        private static CommonFileDialogFilter officeFilesFilter;

        /// <summary>
        ///   Gets a value that specifies the filter for *.txt files.
        /// </summary>
        public static CommonFileDialogFilter TextFiles { get { return textFilesFilter ?? (textFilesFilter = new CommonFileDialogFilter("Text Files", "*.txt")); } }

        /// <summary>
        ///   Gets a value that specifies the filter for picture files.
        /// </summary>
        public static CommonFileDialogFilter PictureFiles { get { return pictureFilesFilter ?? (pictureFilesFilter = new CommonFileDialogFilter("All Picture Files", "*.bmp, *.jpg, *.jpeg, *.png, *.ico")); } }

        /// <summary>
        ///   Gets a value that specifies the filter for Microsoft Office files.
        /// </summary>
        public static CommonFileDialogFilter OfficeFiles { get { return officeFilesFilter ?? (officeFilesFilter = new CommonFileDialogFilter("Office Files", "*.doc, *.docx, *.xls, *.xlsx, *.ppt, *.pptx")); } }
    }
}
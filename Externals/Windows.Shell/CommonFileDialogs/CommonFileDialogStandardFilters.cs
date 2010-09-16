#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

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
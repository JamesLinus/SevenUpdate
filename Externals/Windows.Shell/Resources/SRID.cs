#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
#endregion

namespace Microsoft.Windows.Controls
{
    using System;

    // A wrapper around string identifiers.
    /// <summary>
    /// </summary>
    internal struct SRID
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly string _string;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        private SRID(string s)
        {
            this._string = s;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public static SRID CalendarAutomationPeerBlackoutDayHelpText
        {
            get
            {
                return new SRID("CalendarAutomationPeer_BlackoutDayHelpText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarAutomationPeerCalendarButtonLocalizedControlType
        {
            get
            {
                return new SRID("CalendarAutomationPeer_CalendarButtonLocalizedControlType");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarAutomationPeerDayButtonLocalizedControlType
        {
            get
            {
                return new SRID("CalendarAutomationPeer_DayButtonLocalizedControlType");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarAutomationPeerDecadeMode
        {
            get
            {
                return new SRID("CalendarAutomationPeer_DecadeMode");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarAutomationPeerMonthMode
        {
            get
            {
                return new SRID("CalendarAutomationPeer_MonthMode");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarAutomationPeerYearMode
        {
            get
            {
                return new SRID("CalendarAutomationPeer_YearMode");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarCheckSelectionModeInvalidOperation
        {
            get
            {
                return new SRID("Calendar_CheckSelectionMode_InvalidOperation");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarCollectionMultiThreadedCollectionChangeNotSupported
        {
            get
            {
                return new SRID("CalendarCollection_MultiThreadedCollectionChangeNotSupported");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarNextButtonName
        {
            get
            {
                return new SRID("Calendar_NextButtonName");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarOnDisplayModePropertyChangedInvalidValue
        {
            get
            {
                return new SRID("Calendar_OnDisplayModePropertyChanged_InvalidValue");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarOnFirstDayOfWeekChangedInvalidValue
        {
            get
            {
                return new SRID("Calendar_OnFirstDayOfWeekChanged_InvalidValue");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarOnSelectedDateChangedInvalidOperation
        {
            get
            {
                return new SRID("Calendar_OnSelectedDateChanged_InvalidOperation");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarOnSelectedDateChangedInvalidValue
        {
            get
            {
                return new SRID("Calendar_OnSelectedDateChanged_InvalidValue");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarOnSelectionModeChangedInvalidValue
        {
            get
            {
                return new SRID("Calendar_OnSelectionModeChanged_InvalidValue");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarPreviousButtonName
        {
            get
            {
                return new SRID("Calendar_PreviousButtonName");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID CalendarUNSelectableDates
        {
            get
            {
                return new SRID("Calendar_UnSelectableDates");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID ClipboardCopyModeDisabled
        {
            get
            {
                return new SRID("ClipboardCopyMode_Disabled");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridBeginEditCommandText
        {
            get
            {
                return new SRID("DataGrid_BeginEditCommandText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridCancelEditCommandText
        {
            get
            {
                return new SRID("DataGrid_CancelEditCommandText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridCannotSelectCell
        {
            get
            {
                return new SRID("DataGrid_CannotSelectCell");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridCellItemAutomationPeerNameCoreFormat
        {
            get
            {
                return new SRID("DataGridCellItemAutomationPeer_NameCoreFormat");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridColumnDisplayIndexOutOfRange
        {
            get
            {
                return new SRID("DataGrid_ColumnDisplayIndexOutOfRange");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridColumnIndexOutOfRange
        {
            get
            {
                return new SRID("DataGrid_ColumnIndexOutOfRange");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridCommitEditCommandText
        {
            get
            {
                return new SRID("DataGrid_CommitEditCommandText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridDeleteCommandText
        {
            get
            {
                return new SRID("DataGrid_DeleteCommandText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridDisplayIndexOutOfRange
        {
            get
            {
                return new SRID("DataGrid_DisplayIndexOutOfRange");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridDuplicateDisplayIndex
        {
            get
            {
                return new SRID("DataGrid_DuplicateDisplayIndex");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridInvalidColumnReuse
        {
            get
            {
                return new SRID("DataGrid_InvalidColumnReuse");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridInvalidSortDescription
        {
            get
            {
                return new SRID("DataGrid_InvalidSortDescription");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridLengthInfinity
        {
            get
            {
                return new SRID("DataGridLength_Infinity");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridLengthInvalidType
        {
            get
            {
                return new SRID("DataGridLength_InvalidType");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridNewColumnInvalidDisplayIndex
        {
            get
            {
                return new SRID("DataGrid_NewColumnInvalidDisplayIndex");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridNullColumn
        {
            get
            {
                return new SRID("DataGrid_NullColumn");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridProbableInvalidSortDescription
        {
            get
            {
                return new SRID("DataGrid_ProbableInvalidSortDescription");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridReadonlyCellsItemsSource
        {
            get
            {
                return new SRID("DataGrid_ReadonlyCellsItemsSource");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridRowCannotSelectRowWhenCells
        {
            get
            {
                return new SRID("DataGridRow_CannotSelectRowWhenCells");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridSelectAllCommandText
        {
            get
            {
                return new SRID("DataGrid_SelectAllCommandText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridSelectAllKey
        {
            get
            {
                return new SRID("DataGrid_SelectAllKey");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DataGridSelectAllKeyDisplayString
        {
            get
            {
                return new SRID("DataGrid_SelectAllKeyDisplayString");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DatePickerAutomationPeerLocalizedControlType
        {
            get
            {
                return new SRID("DatePickerAutomationPeer_LocalizedControlType");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DatePickerDropDownButtonName
        {
            get
            {
                return new SRID("DatePicker_DropDownButtonName");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DatePickerOnSelectedDateFormatChangedInvalidValue
        {
            get
            {
                return new SRID("DatePicker_OnSelectedDateFormatChanged_InvalidValue");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DatePickerTextBoxDefaultWatermarkText
        {
            get
            {
                return new SRID("DatePickerTextBox_DefaultWatermarkText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DatePickerTextBoxTemplatePartIsOfIncorrectType
        {
            get
            {
                return new SRID("DatePickerTextBox_TemplatePartIsOfIncorrectType");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID DatePickerWatermarkText
        {
            get
            {
                return new SRID("DatePicker_WatermarkText");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID SelectedCellsCollectionDuplicateItem
        {
            get
            {
                return new SRID("SelectedCellsCollection_DuplicateItem");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID SelectedCellsCollectionInvalidItem
        {
            get
            {
                return new SRID("SelectedCellsCollection_InvalidItem");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID VirtualizedCellInfoCollectionDoesNotSupportIndexChanges
        {
            get
            {
                return new SRID("VirtualizedCellInfoCollection_DoesNotSupportIndexChanges");
            }
        }

        /// <summary>
        /// </summary>
        public static SRID VirtualizedCellInfoCollectionIsReadOnly
        {
            get
            {
                return new SRID("VirtualizedCellInfoCollection_IsReadOnly");
            }
        }

        /// <summary>
        /// </summary>
        public string String
        {
            get
            {
                return this._string;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator ==(SRID x, SRID y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator !=(SRID x, SRID y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
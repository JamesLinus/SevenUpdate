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

namespace Microsoft.Windows.Controls
{
    // A wrapper around string identifiers.
    internal struct SRID
    {
        private readonly string _string;

        private SRID(string s)
        {
            _string = s;
        }

        public string String { get { return _string; } }

        public static SRID DataGrid_SelectAllCommandText { get { return new SRID("DataGrid_SelectAllCommandText"); } }

        public static SRID DataGrid_SelectAllKey { get { return new SRID("DataGrid_SelectAllKey"); } }

        public static SRID DataGrid_SelectAllKeyDisplayString { get { return new SRID("DataGrid_SelectAllKeyDisplayString"); } }

        public static SRID DataGrid_BeginEditCommandText { get { return new SRID("DataGrid_BeginEditCommandText"); } }

        public static SRID DataGrid_CommitEditCommandText { get { return new SRID("DataGrid_CommitEditCommandText"); } }

        public static SRID DataGrid_CancelEditCommandText { get { return new SRID("DataGrid_CancelEditCommandText"); } }

        public static SRID DataGrid_DeleteCommandText { get { return new SRID("DataGrid_DeleteCommandText"); } }

        public static SRID DataGridCellItemAutomationPeer_NameCoreFormat { get { return new SRID("DataGridCellItemAutomationPeer_NameCoreFormat"); } }

        public static SRID CalendarAutomationPeer_CalendarButtonLocalizedControlType { get { return new SRID("CalendarAutomationPeer_CalendarButtonLocalizedControlType"); } }

        public static SRID CalendarAutomationPeer_DayButtonLocalizedControlType { get { return new SRID("CalendarAutomationPeer_DayButtonLocalizedControlType"); } }

        public static SRID CalendarAutomationPeer_BlackoutDayHelpText { get { return new SRID("CalendarAutomationPeer_BlackoutDayHelpText"); } }

        public static SRID Calendar_NextButtonName { get { return new SRID("Calendar_NextButtonName"); } }

        public static SRID Calendar_PreviousButtonName { get { return new SRID("Calendar_PreviousButtonName"); } }

        public static SRID DatePickerAutomationPeer_LocalizedControlType { get { return new SRID("DatePickerAutomationPeer_LocalizedControlType"); } }

        public static SRID DatePickerTextBox_DefaultWatermarkText { get { return new SRID("DatePickerTextBox_DefaultWatermarkText"); } }

        public static SRID DatePicker_DropDownButtonName { get { return new SRID("DatePicker_DropDownButtonName"); } }

        public static SRID DataGrid_ColumnIndexOutOfRange { get { return new SRID("DataGrid_ColumnIndexOutOfRange"); } }

        public static SRID DataGrid_ColumnDisplayIndexOutOfRange { get { return new SRID("DataGrid_ColumnDisplayIndexOutOfRange"); } }

        public static SRID DataGrid_DisplayIndexOutOfRange { get { return new SRID("DataGrid_DisplayIndexOutOfRange"); } }

        public static SRID DataGrid_InvalidColumnReuse { get { return new SRID("DataGrid_InvalidColumnReuse"); } }

        public static SRID DataGrid_DuplicateDisplayIndex { get { return new SRID("DataGrid_DuplicateDisplayIndex"); } }

        public static SRID DataGrid_NewColumnInvalidDisplayIndex { get { return new SRID("DataGrid_NewColumnInvalidDisplayIndex"); } }

        public static SRID DataGrid_NullColumn { get { return new SRID("DataGrid_NullColumn"); } }

        public static SRID DataGrid_ReadonlyCellsItemsSource { get { return new SRID("DataGrid_ReadonlyCellsItemsSource"); } }

        public static SRID DataGrid_InvalidSortDescription { get { return new SRID("DataGrid_InvalidSortDescription"); } }

        public static SRID DataGrid_ProbableInvalidSortDescription { get { return new SRID("DataGrid_ProbableInvalidSortDescription"); } }

        public static SRID DataGridLength_InvalidType { get { return new SRID("DataGridLength_InvalidType"); } }

        public static SRID DataGridLength_Infinity { get { return new SRID("DataGridLength_Infinity"); } }

        public static SRID DataGrid_CannotSelectCell { get { return new SRID("DataGrid_CannotSelectCell"); } }

        public static SRID DataGridRow_CannotSelectRowWhenCells { get { return new SRID("DataGridRow_CannotSelectRowWhenCells"); } }

        public static SRID SelectedCellsCollection_InvalidItem { get { return new SRID("SelectedCellsCollection_InvalidItem"); } }

        public static SRID SelectedCellsCollection_DuplicateItem { get { return new SRID("SelectedCellsCollection_DuplicateItem"); } }

        public static SRID VirtualizedCellInfoCollection_IsReadOnly { get { return new SRID("VirtualizedCellInfoCollection_IsReadOnly"); } }

        public static SRID VirtualizedCellInfoCollection_DoesNotSupportIndexChanges { get { return new SRID("VirtualizedCellInfoCollection_DoesNotSupportIndexChanges"); } }

        public static SRID ClipboardCopyMode_Disabled { get { return new SRID("ClipboardCopyMode_Disabled"); } }

        public static SRID Calendar_OnDisplayModePropertyChanged_InvalidValue { get { return new SRID("Calendar_OnDisplayModePropertyChanged_InvalidValue"); } }

        public static SRID Calendar_OnFirstDayOfWeekChanged_InvalidValue { get { return new SRID("Calendar_OnFirstDayOfWeekChanged_InvalidValue"); } }

        public static SRID Calendar_OnSelectedDateChanged_InvalidValue { get { return new SRID("Calendar_OnSelectedDateChanged_InvalidValue"); } }

        public static SRID Calendar_OnSelectedDateChanged_InvalidOperation { get { return new SRID("Calendar_OnSelectedDateChanged_InvalidOperation"); } }

        public static SRID CalendarCollection_MultiThreadedCollectionChangeNotSupported { get { return new SRID("CalendarCollection_MultiThreadedCollectionChangeNotSupported"); } }

        public static SRID Calendar_CheckSelectionMode_InvalidOperation { get { return new SRID("Calendar_CheckSelectionMode_InvalidOperation"); } }

        public static SRID Calendar_OnSelectionModeChanged_InvalidValue { get { return new SRID("Calendar_OnSelectionModeChanged_InvalidValue"); } }

        public static SRID Calendar_UnSelectableDates { get { return new SRID("Calendar_UnSelectableDates"); } }

        public static SRID DatePickerTextBox_TemplatePartIsOfIncorrectType { get { return new SRID("DatePickerTextBox_TemplatePartIsOfIncorrectType"); } }

        public static SRID DatePicker_OnSelectedDateFormatChanged_InvalidValue { get { return new SRID("DatePicker_OnSelectedDateFormatChanged_InvalidValue"); } }

        public static SRID DatePicker_WatermarkText { get { return new SRID("DatePicker_WatermarkText"); } }

        public static SRID CalendarAutomationPeer_MonthMode { get { return new SRID("CalendarAutomationPeer_MonthMode"); } }

        public static SRID CalendarAutomationPeer_YearMode { get { return new SRID("CalendarAutomationPeer_YearMode"); } }

        public static SRID CalendarAutomationPeer_DecadeMode { get { return new SRID("CalendarAutomationPeer_DecadeMode"); } }
    }
}
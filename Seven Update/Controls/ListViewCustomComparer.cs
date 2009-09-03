#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
// 
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace SevenUpdate.Controls
{
    public abstract class ListViewCustomComparer : IComparer
    {
        protected Dictionary<string, ListSortDirection> SortColumns = new Dictionary<string, ListSortDirection>();

        #region IComparer Members

        public abstract int Compare(object x, object y);

        #endregion

        public void AddSort(string sortColumn, ListSortDirection dir)
        {
            ClearSort();

            SortColumns.Add(sortColumn, dir);
        }

        public void ClearSort()
        {
            SortColumns.Clear();
        }

        protected List<string> GetSortColumnList()
        {
            var result = new List<string>();
            var temp = new Stack<string>();

            foreach (var col in SortColumns.Keys)
                temp.Push(col);

            while (temp.Count > 0)
                result.Add(temp.Pop());

            return result;
        }
    }
}
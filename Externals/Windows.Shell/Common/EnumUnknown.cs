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

#region

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Shell
{
    internal class EnumUnknownClass : IEnumUnknown
    {
        private readonly List<ICondition> conditionList = new List<ICondition>();
        private int current = -1;

        internal EnumUnknownClass(IEnumerable<ICondition> conditions)
        {
            conditionList.AddRange(conditions);
        }

        #region IEnumUnknown Members

        public HRESULT Next(uint requestedNumber, ref IntPtr buffer, ref uint fetchedNumber)
        {
            current++;

            if (current < conditionList.Count)
            {
                buffer = Marshal.GetIUnknownForObject(conditionList[current]);
                fetchedNumber = 1;
                return HRESULT.S_OK;
            }
            return HRESULT.S_FALSE;
        }

        public HRESULT Skip(uint number)
        {
            var temp = current + (int) number;

            if (temp > (conditionList.Count - 1))
                return HRESULT.S_FALSE;
            current = temp;
            return HRESULT.S_OK;
        }

        public HRESULT Reset()
        {
            current = -1;
            return HRESULT.S_OK;
        }

        public HRESULT Clone(out IEnumUnknown result)
        {
            result = new EnumUnknownClass(conditionList.ToArray());
            return HRESULT.S_OK;
        }

        #endregion
    }
}
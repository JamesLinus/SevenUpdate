//Copyright (c) Microsoft Corporation.  All rights reserved.

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
        private List<ICondition> conditionList = new List<ICondition>();
        private int current = -1;

        internal EnumUnknownClass(ICondition[] conditions)
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
            int temp = current + (int) number;

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
//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// </summary>
    internal class EnumUnknownClass : IEnumUnknown
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly List<II> conditionList = new List<II>();

        /// <summary>
        /// </summary>
        private int current = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="conditions">
        /// </param>
        internal EnumUnknownClass(IEnumerable<II> conditions)
        {
            this.conditionList.AddRange(conditions);
        }

        #endregion

        #region Implemented Interfaces

        #region IEnumUnknown

        /// <summary>
        /// </summary>
        /// <param name="result">
        /// </param>
        /// <returns>
        /// </returns>
        public HRESULT Clone(out IEnumUnknown result)
        {
            result = new EnumUnknownClass(this.conditionList.ToArray());
            return HRESULT.S_OK;
        }

        /// <summary>
        /// </summary>
        /// <param name="requestedNumber">
        /// </param>
        /// <param name="buffer">
        /// </param>
        /// <param name="fetchedNumber">
        /// </param>
        /// <returns>
        /// </returns>
        public HRESULT Next(uint requestedNumber, ref IntPtr buffer, ref uint fetchedNumber)
        {
            this.current++;

            if (this.current < this.conditionList.Count)
            {
                buffer = Marshal.GetIUnknownForObject(this.conditionList[this.current]);
                fetchedNumber = 1;
                return HRESULT.S_OK;
            }

            return HRESULT.SFalse;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public HRESULT Reset()
        {
            this.current = -1;
            return HRESULT.S_OK;
        }

        /// <summary>
        /// </summary>
        /// <param name="number">
        /// </param>
        /// <returns>
        /// </returns>
        public HRESULT Skip(uint number)
        {
            var temp = this.current + (int)number;

            if (temp > (this.conditionList.Count - 1))
            {
                return HRESULT.SFalse;
            }

            this.current = temp;
            return HRESULT.S_OK;
        }

        #endregion

        #endregion
    }
}
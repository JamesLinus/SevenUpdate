// ***********************************************************************
// <copyright file="TaskDialogProgressBar.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>Provides a visual representation of the progress of a long running operation.</summary>
    public class TaskDialogProgressBar : TaskDialogBar
    {
        #region Constants and Fields

        /// <summary>The maximum value.</summary>
        private int maximum = TaskDialogDefaults.ProgressBarMaximumValue;

        /// <summary>The minimum value.</summary>
        private int minimum = TaskDialogDefaults.ProgressBarMinimumValue;

        /// <summary>The progress bar value.</summary>
        private int value = TaskDialogDefaults.ProgressBarMinimumValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TaskDialogProgressBar" /> class.</summary>
        protected TaskDialogProgressBar()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TaskDialogProgressBar" /> class.</summary>
        /// <param name="name">The name of the control.</param>
        protected TaskDialogProgressBar(string name) : base(name)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TaskDialogProgressBar" /> class.</summary>
        /// <param name="minimum">The minimum value for this control.</param>
        /// <param name="maximum">The maximum value for this control.</param>
        /// <param name="value">The current value for this control.</param>
        protected TaskDialogProgressBar(int minimum, int maximum, int value)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the maximum value for the control.</summary>
        public int Maximum
        {
            get
            {
                return this.maximum;
            }

            set
            {
                this.CheckPropertyChangeAllowed("Maximum");

                // Check if min / max differ
                if (value < this.Minimum)
                {
                    throw new ArgumentException(Properties.Resources.MaximumValueGreater, "value");
                }

                this.maximum = value;
                this.ApplyPropertyChange("Maximum");
            }
        }

        /// <summary>Gets or sets the minimum value for the control.</summary>
        public int Minimum
        {
            get
            {
                return this.minimum;
            }

            set
            {
                this.CheckPropertyChangeAllowed("Minimum");

                // Check for positive numbers
                if (value < 0)
                {
                    throw new ArgumentException(Properties.Resources.MinimumValuePositive, "value");
                }

                // Check if min / max differ
                if (value >= this.Maximum)
                {
                    throw new ArgumentException(Properties.Resources.MinimumLessValue, "value");
                }

                this.minimum = value;
                this.ApplyPropertyChange("Minimum");
            }
        }

        /// <summary>Gets or sets the current value for the control.</summary>
        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.CheckPropertyChangeAllowed("Value");

                // Check for positive numbers
                if (value < this.Minimum)
                {
                    throw new ArgumentException(Properties.Resources.ValueGreater, "value");
                }

                if (value > this.Maximum)
                {
                    throw new ArgumentException(Properties.Resources.ValueLess, "value");
                }

                this.value = value;
                this.ApplyPropertyChange("Value");
            }
        }

        /// <summary>Gets a value indicating whether this instance has valid values.</summary>
        /// <value><see langword="true" /> if this instance has valid values; otherwise, <see langword="false" />.</value>
        internal bool HasValidValues
        {
            get
            {
                return this.minimum <= this.value && this.value <= this.maximum;
            }
        }

        #endregion

        #region Methods

        /// <summary>Resets the control to its minimum value.</summary>
        protected internal override void Reset()
        {
            base.Reset();
            this.value = this.minimum;
        }

        #endregion
    }
}
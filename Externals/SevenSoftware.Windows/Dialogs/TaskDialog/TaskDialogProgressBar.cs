// ***********************************************************************
// <copyright file="TaskDialogProgressBar.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;

    using SevenSoftware.Windows.Properties;

    /// <summary>Provides a visual representation of the progress of a long running operation.</summary>
    public class TaskDialogProgressBar : TaskDialogBar
    {
        #region Constants and Fields

        /// <summary>The maximum value.</summary>
        private int maximum = TaskDialogDefaults.ProgressBarMaximumValue;

        /// <summary>The minimum value.</summary>
        private int minimum;

        /// <summary>The current progress bar value.</summary>
        private int value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogProgressBar" /> class. Creates a new instance of
        ///   this class.
        /// </summary>
        public TaskDialogProgressBar()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogProgressBar" /> class. Creates a new instance of
        ///   this class with the specified name. And using the default values: Min = 0, Max = 100, Current = 0
        /// </summary>
        /// <param name="name">The name of the control.</param>
        public TaskDialogProgressBar(string name) : base(name)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogProgressBar" /> class. Creates a new instance of
        ///   this class with the specified minimum, maximum and current values.
        /// </summary>
        /// <param name="minimum">The minimum value for this control.</param>
        /// <param name="maximum">The maximum value for this control.</param>
        /// <param name="value">The current value for this control.</param>
        public TaskDialogProgressBar(int minimum, int maximum, int value)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Value = value;
        }

        #endregion

        #region Public Properties

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
                    throw new ArgumentException(Resources.TaskDialogProgressBarMaxValueGreaterThanMin, "value");
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
                    throw new ArgumentException(Resources.TaskDialogProgressBarMinValueGreaterThanZero, "value");
                }

                // Check if min / max differ
                if (value >= this.Maximum)
                {
                    throw new ArgumentException(Resources.TaskDialogProgressBarMinValueLessThanMax, "value");
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
                if (value < this.Minimum || value > this.Maximum)
                {
                    throw new ArgumentException(Resources.TaskDialogProgressBarValueInRange, "value");
                }

                this.value = value;
                this.ApplyPropertyChange("Value");
            }
        }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether the progress bar's value is between its minimum and maximum.</summary>
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
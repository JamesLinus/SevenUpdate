// ***********************************************************************
// <copyright file="TaskDialogProgressBar.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides a visual representation of the progress of a long running operation.
    /// </summary>
    public class TaskDialogProgressBar : TaskDialogBar
    {
        #region Constants and Fields

        /// <summary>
        ///   The maximum value
        /// </summary>
        private int maximum = TaskDialogDefaults.ProgressBarMaximumValue;

        /// <summary>
        ///   The minimum value
        /// </summary>
        private int minimum = TaskDialogDefaults.ProgressBarMinimumValue;

        /// <summary>
        ///   The progress bar value
        /// </summary>
        private int value = TaskDialogDefaults.ProgressBarMinimumValue;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected TaskDialogProgressBar()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name.
        ///   And using the default values: Min = 0, Max = 100, Current = 0
        /// </summary>
        /// <param name="name">
        /// The name of the control.
        /// </param>
        protected TaskDialogProgressBar(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified 
        ///   minimum, maximum and current values.
        /// </summary>
        /// <param name="minimum">
        /// The minimum value for this control.
        /// </param>
        /// <param name="maximum">
        /// The maximum value for this control.
        /// </param>
        /// <param name="value">
        /// The current value for this control.
        /// </param>
        protected TaskDialogProgressBar(int minimum, int maximum, int value)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the maximum value for the control.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", 
            Justification = "Value is standard for progressbar's current value property")]
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
                    throw new ArgumentException("Maximum value provided must be greater than the minimum value", "value");
                }

                this.maximum = value;
                this.ApplyPropertyChange("Maximum");
            }
        }

        /// <summary>
        ///   Gets or sets the minimum value for the control.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", 
            Justification = "Value is standard for progressbar's current value property")]
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
                    throw new ArgumentException("Minimum value provided must be a positive number", "value");
                }

                // Check if min / max differ
                if (value >= this.Maximum)
                {
                    throw new ArgumentException("Minimum value provided must less than the maximum value", "value");
                }

                this.minimum = value;
                this.ApplyPropertyChange("Minimum");
            }
        }

        /// <summary>
        ///   Gets or sets the current value for the control.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", 
            Justification = "Value is standard for progressbar's current value property")]
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
                    throw new ArgumentException("Value provided must be greater than or equal to minimum value", "value");
                }

                if (value > this.Maximum)
                {
                    throw new ArgumentException("Value provided must be less than or equal to the maximum value", "value");
                }

                this.value = value;
                this.ApplyPropertyChange("Value");
            }
        }

        /// <summary>
        /// </summary>
        internal bool HasValidValues
        {
            get
            {
                return this.minimum <= this.value && this.value <= this.maximum;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the control to its minimum value.
        /// </summary>
        protected internal override void Reset()
        {
            base.Reset();
            this.value = this.minimum;
        }

        #endregion
    }
}
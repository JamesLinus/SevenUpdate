// <copyright file="TaskDialogProgressBar.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System;
using SevenSoftware.Windows.Properties;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Provides a visual representation of the progress of a long running operation.</summary>
    public class TaskDialogProgressBar : TaskDialogBar
    {
        /// <summary>The maximum value.</summary>
        int maximum = TaskDialogDefaults.ProgressBarMaximumValue;

        /// <summary>The minimum value.</summary>
        int minimum;

        /// <summary>The current progress bar value.</summary>
        int value;

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
            Minimum = minimum;
            Maximum = maximum;
            Value = value;
        }

        /// <summary>Gets or sets the maximum value for the control.</summary>
        public int Maximum
        {
            get { return maximum; }

            set
            {
                CheckPropertyChangeAllowed("Maximum");

                // Check if min / max differ
                if (value < Minimum)
                {
                    throw new ArgumentException(Resources.TaskDialogProgressBarMaxValueGreaterThanMin, "value");
                }

                maximum = value;
                ApplyPropertyChange("Maximum");
            }
        }

        /// <summary>Gets or sets the minimum value for the control.</summary>
        public int Minimum
        {
            get { return minimum; }

            set
            {
                CheckPropertyChangeAllowed("Minimum");

                // Check for positive numbers
                if (value < 0)
                {
                    throw new ArgumentException(Resources.TaskDialogProgressBarMinValueGreaterThanZero, "value");
                }

                // Check if min / max differ
                if (value >= Maximum)
                {
                    throw new ArgumentException(Resources.TaskDialogProgressBarMinValueLessThanMax, "value");
                }

                minimum = value;
                ApplyPropertyChange("Minimum");
            }
        }

        /// <summary>Gets or sets the current value for the control.</summary>
        public int Value
        {
            get { return value; }

            set
            {
                CheckPropertyChangeAllowed("Value");

                // Check for positive numbers
                if (value < Minimum || value > Maximum)
                {
                    throw new ArgumentException(Resources.TaskDialogProgressBarValueInRange, "value");
                }

                this.value = value;
                ApplyPropertyChange("Value");
            }
        }

        /// <summary>Gets a value indicating whether the progress bar's value is between its minimum and maximum.</summary>
        internal bool HasValidValues
        {
            get { return minimum <= value && value <= maximum; }
        }

        /// <summary>Resets the control to its minimum value.</summary>
        protected internal override void Reset()
        {
            base.Reset();
            value = minimum;
        }
    }
}
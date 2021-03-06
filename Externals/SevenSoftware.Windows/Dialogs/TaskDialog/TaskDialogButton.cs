// <copyright file="TaskDialogButton.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Implements a button that can be hosted in a task dialog.</summary>
    public class TaskDialogButton : TaskDialogButtonBase
    {
        /// <summary>Indicates whether to show the UAC icon.</summary>
        bool showElevationIcon;

        /// <summary>Indicates a value indicating whether the elevation icon will be used.</summary>
        bool useElevationIcon;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButton" /> class. Creates a new instance of this
        ///   class.
        /// </summary>
        public TaskDialogButton()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButton" /> class. Creates a new instance of this
        ///   class with the specified property settings.
        /// </summary>
        /// <param name="name">The name of the button.</param>
        /// <param name="text">The button label.</param>
        public TaskDialogButton(string name, string text) : base(name, text)
        {
        }

        /// <summary>Gets or sets a value indicating whether the elevation icon is displayed.</summary>
        /// <value><c>True</c> if [show elevation icon]; otherwise, <c>False</c>.</value>
        public bool ShowElevationIcon
        {
            get { return showElevationIcon; }

            set
            {
                CheckPropertyChangeAllowed("ShowElevationIcon");
                showElevationIcon = value;
                ApplyPropertyChange("ShowElevationIcon");
            }
        }

        /// <summary>Gets or sets a value indicating whether the elevation icon is displayed.</summary>
        public bool UseElevationIcon
        {
            get { return useElevationIcon; }

            set
            {
                CheckPropertyChangeAllowed("ShowElevationIcon");
                useElevationIcon = value;
                ApplyPropertyChange("ShowElevationIcon");
            }
        }
    }
}
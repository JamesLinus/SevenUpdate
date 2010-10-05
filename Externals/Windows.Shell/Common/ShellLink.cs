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

    /// <summary>
    /// Represents a link to existing FileSystem or Virtual item.
    /// </summary>
    public class ShellLink : ShellObject
    {
        #region Constants and Fields

        /// <summary>
        ///   Path for this file e.g. c:\Windows\file.txt,
        /// </summary>
        private string internalPath;

        /// <summary>
        /// </summary>
        private string internalTargetLocation;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal ShellLink(IShellItem2 shellItem)
        {
            this.nativeShellItem = shellItem;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The path for this link
        /// </summary>
        public virtual string Path
        {
            get
            {
                if (this.internalPath == null && this.NativeShellItem != null)
                {
                    this.internalPath = base.ParsingName;
                }

                return this.internalPath;
            }

            protected set
            {
                this.internalPath = value;
            }
        }

        /// <summary>
        ///   Gets the location to which this link points to.
        /// </summary>
        public string TargetLocation
        {
            get
            {
                if (string.IsNullOrEmpty(this.internalTargetLocation) && this.NativeShellItem2 != null)
                {
                    this.internalTargetLocation = this.Properties.System.Link.TargetParsingPath.Value;
                }

                return this.internalTargetLocation;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                this.internalTargetLocation = value;

                if (this.NativeShellItem2 != null)
                {
                    this.Properties.System.Link.TargetParsingPath.Value = this.internalTargetLocation;
                }
            }
        }

        /// <summary>
        ///   Gets the ShellObject to which this link points to.
        /// </summary>
        public ShellObject TargetShellObject
        {
            get
            {
                return ShellObjectFactory.Create(this.TargetLocation);
            }
        }

        /// <summary>
        ///   Gets or sets the link's title
        /// </summary>
        public string Title
        {
            get
            {
                return this.NativeShellItem2 != null ? this.Properties.System.Title.Value : null;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (this.NativeShellItem2 != null)
                {
                    this.Properties.System.Title.Value = value;
                }
            }
        }

        #endregion
    }
}
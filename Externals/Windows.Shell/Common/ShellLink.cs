//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents a link to existing FileSystem or Virtual item.
    /// </summary>
    public class ShellLink : ShellObject
    {
        /// <summary>
        ///   Path for this file e.g. c:\Windows\file.txt,
        /// </summary>
        private string internalPath;

        #region Internal Constructors

        internal ShellLink(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }

        #endregion

        #region Public Properties

        private string internalTargetLocation;

        /// <summary>
        ///   The path for this link
        /// </summary>
        public virtual string Path
        {
            get
            {
                if (internalPath == null && NativeShellItem != null)
                    internalPath = base.ParsingName;
                return internalPath;
            }
            protected set { internalPath = value; }
        }

        /// <summary>
        ///   Gets the location to which this link points to.
        /// </summary>
        public string TargetLocation
        {
            get
            {
                if (string.IsNullOrEmpty(internalTargetLocation) && NativeShellItem2 != null)
                    internalTargetLocation = Properties.System.Link.TargetParsingPath.Value;

                return internalTargetLocation;
            }
            set
            {
                if (value == null)
                    return;

                internalTargetLocation = value;

                if (NativeShellItem2 != null)
                    Properties.System.Link.TargetParsingPath.Value = internalTargetLocation;
            }
        }

        /// <summary>
        ///   Gets the ShellObject to which this link points to.
        /// </summary>
        public ShellObject TargetShellObject { get { return ShellObjectFactory.Create(TargetLocation); } }

        /// <summary>
        ///   Gets or sets the link's title
        /// </summary>
        public string Title
        {
            get { return NativeShellItem2 != null ? Properties.System.Title.Value : null; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (NativeShellItem2 != null)
                    Properties.System.Title.Value = value;
            }
        }

        #endregion
    }
}
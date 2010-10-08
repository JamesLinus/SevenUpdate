// ***********************************************************************
// <copyright file="Config.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>
    /// Automatic Update option Seven Update can use
    /// </summary>
    [ProtoContract]
    [DataContract]
    [DefaultValue(Install)]
    public enum AutoUpdateOption
    {
        /// <summary>
        ///   Download and Installs updates automatically
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Install = 0, 

        /// <summary>
        ///   Downloads Updates automatically
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Download = 1, 

        /// <summary>
        ///   Only checks and notifies the user of updates
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Notify = 2, 

        /// <summary>
        ///   No automatic checking
        /// </summary>
        [ProtoEnum]
        [EnumMember]
        Never = 3
    }

    /// <summary>
    /// Configuration options
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    public sealed class Config : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The automatic update setting
        /// </summary>
        private AutoUpdateOption autoOption;

        /// <summary>
        ///   A value that indicates whether to treat <see cref = "Importance.Recommended" /> updates the same as <see cref = "Importance.Important" /> updates
        /// </summary>
        private bool includeRecommended;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets which automatic update option Seven Update should use
        /// </summary>
        /// <value>The automatic update option</value>
        [ProtoMember(1)]
        [DataMember]
        public AutoUpdateOption AutoOption
        {
            get
            {
                return this.autoOption;
            }

            set
            {
                this.autoOption = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("AutoOption");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Seven Update is to included recommended updates when automatically downloading updates
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if recommended updates should be treated as important updates otherwise, <see langword = "false" />.
        /// </value>
        [ProtoMember(2)]
        [DataMember]
        public bool IncludeRecommended
        {
            get
            {
                return this.includeRecommended;
            }

            set
            {
                this.includeRecommended = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("IncludeRecommended");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="name">
        /// The name of the property that changed
        /// </param>
        private void OnPropertyChanged(string name)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
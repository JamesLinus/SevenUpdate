// ***********************************************************************
// <copyright file="LocaleString.cs"
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
    /// Contains a string indicating the language and a value
    /// </summary>
    [ProtoContract]
    [DataContract]
    public sealed class LocaleString : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The ISO language code
        /// </summary>
        private string lang;

        /// <summary>
        ///   The value of the string
        /// </summary>
        private string value;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets an ISO language code
        /// </summary>
        /// <value>The iso code</value>
        [ProtoMember(1)] [DataMember] public string Lang
        {
            get
            {
                return this.lang;
            }

            set
            {
                this.lang = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Lang");
            }
        }

        /// <summary>
        ///   Gets or sets the value of the string
        /// </summary>
        /// <value>The value.</value>
        [ProtoMember(2)] [DataMember] public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;

                // Call OnPropertyChanged whenever the property is updated
                this.OnPropertyChanged("Value");
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
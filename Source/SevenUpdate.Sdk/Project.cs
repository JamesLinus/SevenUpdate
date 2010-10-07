// ***********************************************************************
// Assembly         : SevenUpdate.Sdk
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.Sdk
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>
    /// Contains data specifying the application name and it's updates
    /// </summary>
    [ProtoContract]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Sua))]
    [KnownType(typeof(ObservableCollection<Update>))]
    public class Project : INotifyPropertyChanged
    {
        #region Constants and Fields

        /// <summary>
        ///   The localized name of the application
        /// </summary>
        private string applicationName;

        /// <summary>
        ///   The collection of localized update names
        /// </summary>
        private ObservableCollection<string> updateNames;

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the localized application name
        /// </summary>
        /// <value>The name of the application.</value>
        [ProtoMember(1)]
        [DataMember]
        public string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                this.applicationName = value;
                this.OnPropertyChanged("ApplicationName");
            }
        }

        /// <summary>
        ///   Gets or sets the update names.
        /// </summary>
        /// <value>The update names.</value>
        [ProtoMember(2)]
        [DataMember]
        public ObservableCollection<string> UpdateNames
        {
            get
            {
                return this.updateNames;
            }

            set
            {
                this.updateNames = value;
                this.OnPropertyChanged("UpdateNames");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When a property has changed, call the <see cref="OnPropertyChanged"/> Event
        /// </summary>
        /// <param name="name">
        /// The name of the property changed
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
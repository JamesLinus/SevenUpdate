// <copyright file="SearchCompletedEventArgs.cs" project="SevenUpdate.Base">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using ProtoBuf;

    /// <summary>Provides event data for the SearchCompleted event.</summary>
    [ProtoContract]
    [DataContract]
    public sealed class SearchCompletedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="SearchCompletedEventArgs" /> class.</summary>
        /// <param name="applications">The collection of applications to update.</param>
        /// <param name="importantCount">The number of important updates.</param>
        /// <param name="recommendedCount">The number of recommended updates.</param>
        /// <param name="optionalCount">The number of optional updates.</param>
        public SearchCompletedEventArgs(
            IEnumerable<Sui> applications, int importantCount, int recommendedCount, int optionalCount)
        {
            this.Applications = applications;
            this.ImportantCount = importantCount;
            this.OptionalCount = optionalCount;
            this.RecommendedCount = recommendedCount;
        }

        /// <summary>Initializes a new instance of the <see cref="SearchCompletedEventArgs" /> class.</summary>
        public SearchCompletedEventArgs()
        {
        }

        /// <summary>Gets a collection of applications that contain updates to install.</summary>
        /// <value>The applications.</value>
        [ProtoMember(1)]
        [DataMember]
        public IEnumerable<Sui> Applications { get; private set; }

        /// <summary>Gets or sets the important updates count.</summary>
        /// <value>The important count.</value>
        [ProtoMember(2)]
        [DataMember]
        public int ImportantCount { get; set; }

        /// <summary>Gets or sets the optional updates count.</summary>
        /// <value>The optional count.</value>
        [ProtoMember(3)]
        [DataMember]
        public int OptionalCount { get; set; }

        /// <summary>Gets the recommended updates count.</summary>
        /// <value>The recommended count.</value>
        [ProtoMember(4)]
        [DataMember]
        public int RecommendedCount { get; private set; }
    }
}
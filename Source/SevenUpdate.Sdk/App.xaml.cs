// ***********************************************************************
// <copyright file="App.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.Sdk
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;

    using SevenUpdate.Sdk.Properties;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="InstanceAwareApplication.Startup"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="isFirstInstance">
        /// If set to <c>true</c> the current instance is the first application instance.
        /// </param>
        protected override void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            Utilities.SerializationError += Core.SerializationError;
            Utilities.Locale = Settings.Default.locale;
            base.OnStartup(e, isFirstInstance);

            if (!isFirstInstance)
            {
                this.Shutdown(1);
            }

            Directory.CreateDirectory(Core.UserStore);
            Core.SetJumpList();
        }

        /// <summary>
        /// Raises the <see cref="InstanceAwareApplication.StartupNextInstance"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="InstanceAwareApplication.StartupNextInstanceEventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);

            if (e.GetArgs().Length <= 0)
            {
                return;
            }

            switch (e.GetArgs()[0])
            {
                case @"-newproject":
                    Core.NewProject();
                    break;

                case @"-newupdate":
                    Core.AppIndex = Convert.ToInt32(e.GetArgs()[1], CultureInfo.CurrentCulture);

                    Core.NewUpdate();
                    break;

                case @"-edit":
                    Core.AppIndex = Convert.ToInt32(e.GetArgs()[1], CultureInfo.CurrentCulture);
                    Core.UpdateIndex = Convert.ToInt32(e.GetArgs()[2], CultureInfo.CurrentCulture);
                    Core.EditItem();
                    break;
            }
        }

        #endregion
    }
}
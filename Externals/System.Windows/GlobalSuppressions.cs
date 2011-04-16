// ***********************************************************************
// <copyright file="GlobalSuppressions.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// <summary>
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.
// .</summary>
// ***********************************************************************
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfigIconUnion.#Spacer")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfigIconUnion.#MainIcon")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfigIconUnion.#Icon")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CanCancel", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#CanCancel")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Scope = "type", Target = "System.Windows.Internal.Margins")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uac", Scope = "type", Target = "System.Windows.Controls.UacButton")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dwm", Scope = "type", Target = "System.Windows.Internal.DwmMessages")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "System.Windows.Controls.CommandLink.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Scope = "member", Target = "System.Windows.Controls.ProgressIndicator.#.cctor()")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Theming", Scope = "type", Target = "System.Windows.Internal.EnableThemingInScope")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.ApplicationServices.AppRestartRecoveryNativeMethods.#GetApplicationRestartSettings(System.IntPtr,System.IntPtr,System.UInt32&,System.Windows.ApplicationServices.RestartRestrictions&)")]
[assembly: SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods.#TaskDialogIndirect(System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfig,System.Int32&,System.Int32&,System.Boolean&)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Scope = "type", Target = "System.Windows.Internal.Result")]
[assembly: SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Scope = "type", Target = "System.Windows.Internal.BlurBehindOptions")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#StartupLocation")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#StandardButtons")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#HyperlinksEnabled")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#OwnerWindowHandle")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#ProgressBar")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#FooterCheckBoxText")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#ExpansionMode")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#DetailsExpandedLabel")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#DetailsExpanded")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#DetailsCollapsedLabel")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#CanCancel")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Dialogs.TaskDialog.ThrowIfDialogShowing(System.String)", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#Caption")]
[assembly: SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "type", Target = "System.Windows.InstanceAwareApplication")]
[assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "System.Windows.ValidationRules.UrlInputRule.#Validate(System.Object,System.Globalization.CultureInfo)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Scope = "type", Target = "System.Windows.Dialogs.TaskDialogResult")]
[assembly: SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type", Target = "System.Windows.Dialogs.TaskDialogProgressBarState")]
[assembly: SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type", Target = "System.Windows.Dialogs.TaskDialogResult")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.ValidationRules")]
[assembly: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.Windows.Controls.AdornerExtensions.#RemoveAdorners`1(System.Windows.Documents.AdornerLayer,System.Windows.UIElement)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "System.Windows.Controls.AdornerExtensions.#Contains`1(System.Windows.Documents.AdornerLayer,System.Windows.UIElement)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Dialogs.NativeTaskDialog.#SelectedRadioButtonID")]
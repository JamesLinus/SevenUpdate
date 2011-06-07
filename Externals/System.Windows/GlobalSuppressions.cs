// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type,
// member, etc.
//
// To add a suppression to this file, right-click the message in the Error List, point to "Suppress Message(s)", and
// click "In Project Suppression File". You do not need to add suppressions to this file manually.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CanCancel", Scope = "member", Target = "System.Windows.Dialogs.TaskDialog.#CanCancel")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uac", Scope = "type", Target = "System.Windows.Controls.UacButton")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dwm", Scope = "type", Target = "System.Windows.Internal.DwmMessages")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogProgressBar.#Value")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogProgressBar.#Minimum")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogProgressBar.#Maximum")]
[assembly: SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods.#TaskDialogIndirect(System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfig,System.Int32&,System.Int32&,System.Boolean&)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Scope = "type", Target = "System.Windows.Internal.BlurBehindOptions")]
[assembly: SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Scope = "type", Target = "System.Windows.Internal.Result")]
[assembly: SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member", Target = "System.Windows.StartupNextInstanceEventArgs.#.ctor(System.String[],System.Boolean)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfigIconUnion.#Spacer")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfigIconUnion.#MainIcon")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "System.Windows.Dialogs.TaskDialogNativeMethods+TaskDialogConfigIconUnion.#Icon")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Scope = "type", Target = "System.Windows.Internal.Margins")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Dialogs.NativeTaskDialog.#SelectedRadioButtonID")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.ValidationRules")]
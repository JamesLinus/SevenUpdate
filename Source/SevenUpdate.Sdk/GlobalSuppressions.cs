// ***********************************************************************
// <copyright file="GlobalSuppressions.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "XamlGeneratedNamespace.GeneratedInternalTypeHelper.#SetPropertyValue(System.Reflection.PropertyInfo,System.Object,System.Object,System.Globalization.CultureInfo)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "XamlGeneratedNamespace.GeneratedInternalTypeHelper.#GetPropertyValue(System.Reflection.PropertyInfo,System.Object,System.Globalization.CultureInfo)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member", Target = "XamlGeneratedNamespace.GeneratedInternalTypeHelper.#CreateDelegate(System.Type,System.Object,System.String)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "XamlGeneratedNamespace.GeneratedInternalTypeHelper.#AddEventHandler(System.Reflection.EventInfo,System.Object,System.Delegate)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member", Target = "SevenUpdate.Sdk.Core.#OpenFileDialog(System.String,System.String,System.Boolean,System.String,System.Boolean)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member", Target = "SevenUpdate.Sdk.Core.#SaveFileDialog(System.String,System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "sevenalive", Scope = "resource", Target = "SevenUpdate.Sdk.Properties.Resources.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "resource", Target = "SevenUpdate.Sdk.Properties.Resources.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "protobuf-net", Scope = "resource", Target = "SevenUpdate.Sdk.Properties.Resources.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "startmenu", Scope = "resource", Target = "SevenUpdate.Sdk.Properties.Resources.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "sevenupdate", Scope = "resource", Target = "SevenUpdate.Sdk.Properties.Resources.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "changelog", Scope = "resource", Target = "SevenUpdate.Sdk.Properties.Resources.resources")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "lnk", Scope = "member", Target = "SevenUpdate.Sdk.Core.#OpenFileDialog(System.String,System.String,System.Boolean,System.String,System.Boolean)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "lnk", Scope = "member", Target = "SevenUpdate.Sdk.Core.#SaveFileDialog(System.String,System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "sua", Scope = "member", Target = "SevenUpdate.Sdk.Core.#OpenFileDialog(System.String,System.String,System.Boolean,System.String,System.Boolean)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "sua", Scope = "member", Target = "SevenUpdate.Sdk.Core.#SaveFileDialog(System.String,System.String,System.String)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type", Target = "SevenUpdate.Sdk.Converters.StringToLocaleStringConverter")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type", Target = "SevenUpdate.Sdk.Converters.DateConverter")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type", Target = "SevenUpdate.Sdk.Converters.BooleanToLabelConverter")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "SevenUpdate.Sdk.ValidationRules.LocaleStringRule.#PropertyName")]
// ***********************************************************************
// <copyright file="GlobalSuppressions.cs"
//            project="SevenUpdate.Base"
//            assembly="SevenUpdate.Base"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// <summary>
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.
// </summary>
// ***********************************************************************
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Sua.#AppUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Sua.#HelpUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Sua.#SuiUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Suh.#AppUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Suh.#HelpUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Suh.#InfoUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Update.#InfoUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Update.#LicenseUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Scope = "member", Target = "SevenUpdate.Update.#DownloadUrl")]
[assembly: SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Scope = "member", Target = "SevenUpdate.Utilities.#DownloadFile(System.String)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Scope = "member", Target = "SevenUpdate.Utilities.#Deserialize`1(System.IO.Stream,System.String)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "SevenUpdate.Utilities.ReportError(System.String,System.String)", Scope = "member", Target = "SevenUpdate.Download.#ReportDownloadError(System.Object,SharpBits.Base.ErrorNotificationEventArgs)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "SevenUpdate.Utilities.ReportError(System.String,System.String)", Scope = "member", Target = "SevenUpdate.Install.#UpdateFile(SevenUpdate.UpdateFile)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "SevenUpdate.Utilities.ReportError(System.String,System.String)", Scope = "member", Target = "SevenUpdate.Search.#SearchForUpdates(System.Collections.Generic.IEnumerable`1<SevenUpdate.Sua>)")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.string.Format(System.String,System.Object)", Scope = "member", Target = "SevenUpdate.Utilities.#GetHash(System.String)")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Scope = "member", Target = "SevenUpdate.Localestring.#Lang")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Scope = "member", Target = "SevenUpdate.Localestring.#PropertyChanged")]
[assembly: SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Scope = "member", Target = "SevenUpdate.Localestring.#Value")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "FileNotFound", Scope = "member", Target = "SevenUpdate.Install.#UpdateFile(SevenUpdate.UpdateFile)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sua", Scope = "type", Target = "SevenUpdate.Sua")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Suh", Scope = "type", Target = "SevenUpdate.Suh")]
[assembly: SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Scope = "member", Target = "SevenUpdate.Utilities.#DownloadFile(System.String,System.Boolean)")]
[assembly: SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "hideExceptions", Scope = "member", Target = "SevenUpdate.Utilities.#DownloadFile(System.String,System.Boolean)")]
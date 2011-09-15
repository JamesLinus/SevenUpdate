// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to this project.
// Project-level suppressions either have no target or are given a specific target and scoped to a namespace, type,
// member, etc. To add a suppression to this file, right-click the message in the Error List, point to "Suppress
// Message(s)", and click "In Project Suppression File". You do not need to add suppressions to this file manually.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "WPF")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "WPF", 
        Scope = "namespace", Target = "WPFLocalizeExtension.Engine")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "WPF", 
        Scope = "namespace", Target = "WPFLocalizeExtension.Extensions")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member", 
        Target =
            "WPFLocalizeExtension.Extensions.BaseLocalizeExtension`1.#SetTargetValue(System.Windows.DependencyObject,System.Object,System.Object)"
        )]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", 
        MessageId = "WPFLocalizeExtension.Extensions.NativeMethods.DeleteObject(System.IntPtr)", Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocImageExtension.#FormatOutput(System.Object)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "FlowDirection", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocFlowDirectionExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "IProvideValueTarget"
        , Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.OddsFormatExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OddsFormatType", 
        Scope = "member", 
        Target =
            "WPFLocalizeExtension.Engine.OddsFormatManager.#SetOddsFormatFromDependencyProperty(System.Windows.DependencyObject,System.Windows.DependencyPropertyChangedEventArgs)"
        )]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocBrushExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocDoubleExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocFlowDirectionExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocImageExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocTextExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocTextUpperExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocThicknessExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "WeakReference", 
        Scope = "member", 
        Target =
            "WPFLocalizeExtension.Engine.ObjectDependencyManager.#AddObjectDependency(System.WeakReference,System.Object)"
        )]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ResourceKey", 
        Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.LocTextLowerExtension.#ProvideValue(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Scope = "member", 
        Target =
            "WPFLocalizeExtension.Engine.Localize.#ParseKey(System.String,System.String&,System.String&,System.String&)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Scope = "member", 
        Target =
            "WPFLocalizeExtension.Engine.Localize.#ParseKey(System.String,System.String&,System.String&,System.String&)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Scope = "member", 
        Target =
            "WPFLocalizeExtension.Engine.Localize.#ParseKey(System.String,System.String&,System.String&,System.String&)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Scope = "member", 
        Target =
            "WPFLocalizeExtension.Extensions.BaseLocalizeExtension`1.#ResolveLocalizedValue(!0&,System.Globalization.CultureInfo)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Scope = "member", 
        Target = "WPFLocalizeExtension.Extensions.BaseLocalizeExtension`1.#ResolveLocalizedValue(!0&)")]

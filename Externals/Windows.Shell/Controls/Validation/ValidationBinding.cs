#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

#endregion

namespace Microsoft.Windows.Controls
{
    [MarkupExtensionReturnType(typeof (string)), Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable, Readability = Readability.Unreadable)] 
    
    public class ValidationBinding : MarkupExtension
    {
        #region Private Fields

        // Our binding object, pre-initialized with PropertyChanged trigger.

        // Rule collection
        private readonly StringToValidationRuleConverter ruleConverter = new StringToValidationRuleConverter();
        private readonly List<ValidationRule> rules = new List<ValidationRule>();
        private Binding binding = new Binding {UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged};

        private ValidationRule rule;

        // Convert string to rule instance.

        #endregion

        #region Public properties

        [DefaultValue(null)]
        public object AsyncState { get { return binding.AsyncState; } set { binding.AsyncState = value; } }

        [Browsable(false)]
        public Binding Binding { get { return binding; } set { binding = value; } }

        [DefaultValue(false)]
        public bool BindsDirectlyToSource { get { return binding.BindsDirectlyToSource; } set { binding.BindsDirectlyToSource = value; } }

        [DefaultValue(null)]
        public IValueConverter Converter { get { return binding.Converter; } set { binding.Converter = value; } }

        [TypeConverter(typeof (CultureInfoIetfLanguageTagConverter)), DefaultValue(null)]
        public CultureInfo ConverterCulture { get { return binding.ConverterCulture; } set { binding.ConverterCulture = value; } }

        [DefaultValue(null)]
        public object ConverterParameter { get { return binding.ConverterParameter; } set { binding.ConverterParameter = value; } }

        [DefaultValue(null)]
        public string ElementName { get { return binding.ElementName; } set { binding.ElementName = value; } }

        [DefaultValue(null)]
        public object FallbackValue { get { return binding.FallbackValue; } set { binding.FallbackValue = value; } }

        [DefaultValue(false)]
        public bool IsAsync { get { return binding.IsAsync; } set { binding.IsAsync = value; } }

        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode { get { return binding.Mode; } set { binding.Mode = value; } }

        [DefaultValue(false)]
        public bool NotifyOnSourceUpdated { get { return binding.NotifyOnSourceUpdated; } set { binding.NotifyOnSourceUpdated = value; } }

        [DefaultValue(false)]
        public bool NotifyOnTargetUpdated { get { return binding.NotifyOnTargetUpdated; } set { binding.NotifyOnTargetUpdated = value; } }

        [DefaultValue(false)]
        public bool NotifyOnValidationError { get { return binding.NotifyOnValidationError; } set { binding.NotifyOnValidationError = value; } }

        [DefaultValue(null)]
        public PropertyPath Path { get { return binding.Path; } set { binding.Path = value; } }

        [DefaultValue(null)]
        public RelativeSource RelativeSource { get { return binding.RelativeSource; } set { binding.RelativeSource = value; } }

        [DefaultValue(null)]
        public object Source { get { return binding.Source; } set { binding.Source = value; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter { get { return binding.UpdateSourceExceptionFilter; } set { binding.UpdateSourceExceptionFilter = value; } }

        [DefaultValue(UpdateSourceTrigger.PropertyChanged)]
        public UpdateSourceTrigger UpdateSourceTrigger { get { return binding.UpdateSourceTrigger; } set { binding.UpdateSourceTrigger = value; } }

        [DefaultValue(false)]
        public bool ValidatesOnDataErrors { get { return binding.ValidatesOnDataErrors; } set { binding.ValidatesOnDataErrors = value; } }

        [DefaultValue(false)]
        public bool ValidatesOnExceptions { get { return binding.ValidatesOnExceptions; } set { binding.ValidatesOnExceptions = value; } }

        [DefaultValue(null)]
        public string XPath { get { return binding.XPath; } set { binding.XPath = value; } }

        [DefaultValue(null)]
        public Collection<ValidationRule> ValidationRules { get { return binding.ValidationRules; } }

        [DefaultValue(null), EditorBrowsable(EditorBrowsableState.Always), TypeConverter(typeof (StringToValidationRuleConverter)), ConstructorArgument("ruleName")]
        
        
        
        public ValidationRule Rule
        {
            get { return rule; }
            set
            {
                rule = value;
                rules.Add(rule);
            }
        }

        #endregion

        #region Constructor

        public ValidationBinding()
        {
        }

        public ValidationBinding(string ruleName)
        {
            AddRule(ruleName);
        }

        public ValidationBinding(string ruleName, string path) : this(ruleName)
        {
            Path = new PropertyPath(path);
        }

        public ValidationBinding(string ruleName1, string ruleName2, string path) : this(ruleName1, path)
        {
            AddRule(ruleName2);
        }

        public ValidationBinding(string ruleName1, string ruleName2, string ruleName3, string path) : this(ruleName1, ruleName2, path)
        {
            AddRule(ruleName3);
        }

        #endregion

        #region Implementation

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof (IProvideValueTarget)) as IProvideValueTarget;

            DependencyObject targetDependencyObject;
            DependencyProperty targetDependencyProperty;

            CheckCanReceiveMarkupExtension(this, provideValueTarget, out targetDependencyObject, out targetDependencyProperty);

            if (targetDependencyObject == null || targetDependencyProperty == null)
                return this;

            // Fetch metadata - unused
            var metadata = targetDependencyProperty.GetMetadata(targetDependencyObject.DependencyObjectType) as FrameworkPropertyMetadata;

            if ((metadata != null && !metadata.IsDataBindingAllowed) || targetDependencyProperty.ReadOnly)
                throw new ArgumentException("");

            // Two-way binding requires a path
            if ((binding.Mode == BindingMode.TwoWay || binding.Mode == BindingMode.Default) && binding.XPath == null && (binding.Path == null || String.IsNullOrEmpty(binding.Path.Path)))
                throw new InvalidOperationException("Two way binding has no Path.");

            if (rules.Count > 0)
            {
                foreach (var r in rules)
                    binding.ValidationRules.Add(r);
            }

            return binding.ProvideValue(serviceProvider);
        }

        /// <summary>
        ///   This static method validates the holder of the current extension, where it is being defined. It detects the various scenarios
        ///   where a MarkupExtension is allowed to be set and tests their candidacy for the assignment.
        /// </summary>
        /// <param name = "markupExtension"></param>
        /// <param name = "provideValueTarget"></param>
        /// <param name = "targetDependencyObject"></param>
        /// <param name = "targetDependencyProperty"></param>
        public static void CheckCanReceiveMarkupExtension(MarkupExtension markupExtension, IProvideValueTarget provideValueTarget, out DependencyObject targetDependencyObject,
                                                          out DependencyProperty targetDependencyProperty)
        {
            targetDependencyObject = null;
            targetDependencyProperty = null;

            if (provideValueTarget == null)
                return;

            var targetObject = provideValueTarget.TargetObject;

            if (targetObject == null)
                return;

            var targetType = targetObject.GetType();
            var targetProperty = provideValueTarget.TargetProperty;

            if (targetProperty != null)
            {
                targetDependencyProperty = targetProperty as DependencyProperty;
                if (targetDependencyProperty != null)
                {
                    targetDependencyObject = targetObject as DependencyObject;
                    Debug.Assert(targetDependencyObject != null, "DependencyProperties can only be set on DependencyObjects");
                }
                else
                {
                    //throw new XamlParseException("Type not assignable.");

                    var targetMember = targetProperty as MemberInfo;
                    if (targetMember != null)
                    {
                        // If targetMember is a MemberInfo, then its the CLR Property case.
                        // Setters, Triggers, DataTriggers & Conditions are the special cases of
                        // CLR properties in which DynamicResource and Bindings are allowed.
                        // Since StyleHelper.ProcessSharedPropertyValue prevents calls to ProvideValue
                        // in such cases, there is no need for special code to handle them here. 

                        // Find the MemberType

                        Debug.Assert(targetMember is PropertyInfo || targetMember is MethodInfo, "TargetMember is either a CLR property or an attached static setter method");

                        Type memberType;

                        var propertyInfo = targetMember as PropertyInfo;
                        if (propertyInfo != null)
                            memberType = propertyInfo.PropertyType;
                        else
                        {
                            var methodInfo = (MethodInfo) targetMember;
                            var parameterInfos = methodInfo.GetParameters();
                            Debug.Assert(parameterInfos.Length == 2, "The signature of a static settor must contain two parameters");
                            memberType = parameterInfos[1].ParameterType;
                        }

                        // Check if the MarkupExtensionType is assignable to the given MemberType
                        // This check is to allow properties such as the following
                        // - DataTrigger.Binding
                        // - Condition.Binding 
                        // - HierarchicalDataTemplate.ItemsSource
                        // - GridViewColumn.DisplayMemberBinding 

                        if (!typeof (MarkupExtension).IsAssignableFrom(memberType) || !memberType.IsAssignableFrom(markupExtension.GetType()))
                            throw new XamlParseException("Type not assignable.");
                    }
                    else
                    {
                        // This is the Collection ContentProperty case
                        // Example:
                        // <DockPanel>
                        //   <Button /> 
                        //   <DynamicResource ResourceKey="foo" />
                        // </DockPanel> 

                        // Collection<BindingBase> used in MultiBinding is a special
                        // case of a Collection that can contain a Binding. 

                        if (!typeof (BindingBase).IsAssignableFrom(markupExtension.GetType()) || !typeof (Collection<BindingBase>).IsAssignableFrom(targetProperty.GetType()))
                            throw new XamlParseException("Type not assignable.");
                    }
                }
            }
            else
            {
                // This is the explicit Collection Property case
                // Example: 
                // <DockPanel> 
                // <DockPanel.Children>
                //   <Button /> 
                //   <DynamicResource ResourceKey="foo" />
                // </DockPanel.Children>
                // </DockPanel>

                // Collection<BindingBase> used in MultiBinding is a special
                // case of a Collection that can contain a Binding. 

                if (!typeof (BindingBase).IsAssignableFrom(markupExtension.GetType()) || !typeof (Collection<BindingBase>).IsAssignableFrom(targetType))
                    throw new XamlParseException("Type not assignable.");
            }
        }

        private void AddRule(string ruleName)
        {
            var validationRule = ruleConverter.ConvertFrom(null, null, ruleName) as ValidationRule;

            if (validationRule != null)
                rules.Add(validationRule);
        }

        #endregion
    }
}
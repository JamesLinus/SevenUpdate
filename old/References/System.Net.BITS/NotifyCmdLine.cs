/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms.Design;
using System.Drawing.Design;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getnotifycmdline.asp
	/// </summary>
	[
		TypeConverter(typeof(NotifyCmdLineTypeConverter)),
		Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob2_getnotifycmdline.asp")
	]
	public sealed class NotifyCmdLine : ICloneable, ISerializable
	{
		private string program;
		private string parameters;

		/// <summary>
		/// Constructor
		/// </summary>
		public NotifyCmdLine()
			: this(string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public NotifyCmdLine(string program, string parameters)
		{
			Program = program;
			Parameters = parameters;
		}

		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is NotifyCmdLine)
			{
				NotifyCmdLine other = (NotifyCmdLine)obj;
				
				return program == other.program && parameters == other.parameters;
			}
			
			return base.Equals(obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table. 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(	Properties.Resources.NotifyCmdLineToStringFormat,
									Program,
									Parameters);
		}
		
		/// <summary>
		/// The program to execute when the job enters the error or transferred state.
		/// </summary>
		[
			EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor)),
			DefaultValue(""),
			Localizable(true),
			Description("The program to execute when the job enters the error or transferred state.")
		]
		public string Program
		{
			get { return program; }
			set { program = value != null ? value : string.Empty; }
		}

		/// <summary>
		/// The arguments of the program in NotifyCmdLine.Program
		/// </summary>
		[
			DefaultValue(""),
			Localizable(true),
			Description("The arguments of the program in NotifyCmdLine.Program")
		]
		public string Parameters
		{
			get { return parameters; }
			set { parameters = value != null ? value : string.Empty; }
		}

		internal string ArgProgram
		{
			get { return program == string.Empty ? null : program; }
		}

		internal string ArgParam
		{
			get { return program == string.Empty || parameters == string.Empty
							? null
							: parameters; }
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new NotifyCmdLine(program, parameters);
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public NotifyCmdLine(SerializationInfo UpdateInfo, StreamingContext context) 
        {
			program = (string)UpdateInfo.GetValue("Program", typeof(string));
			parameters = (string)UpdateInfo.GetValue("Parameters", typeof(string));
        }

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object.
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo UpdateInfo, StreamingContext context)
        {
			UpdateInfo.AddValue("Program", program);
			UpdateInfo.AddValue("Parameters", parameters);
        }
	
		#endregion
	}

	internal class NotifyCmdLineTypeConverter : ExpandableObjectConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return	destinationType == typeof(string) ||
					destinationType == typeof(InstanceDescriptor) ||
					base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value == null)
					return string.Empty;

				return value.ToString();
			}
			else if (destinationType == typeof(InstanceDescriptor) && value is NotifyCmdLine)
			{
				NotifyCmdLine ncl = (NotifyCmdLine)value;
				object[] args = null;
				MemberInfo mi;

				if (ncl == new NotifyCmdLine())
					mi = typeof(JobProxySettings).GetConstructor(Type.EmptyTypes);
				else
				{
					mi = typeof(NotifyCmdLine).GetConstructor(	new Type[]
																{
																	typeof(string),
																	typeof(string)
																});
					args = new object[] { ncl.Program, ncl.Parameters };
				}
				
				return new InstanceDescriptor(mi, args);
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string s = value as string;

				if (s == null || s == string.Empty)
					return new NotifyCmdLine();

				Regex reg = new Regex(Properties.Resources.NotifyCmdLineRegex);
				Match match = reg.Match(s);
				
				if (!match.Success)
					throw new FormatException();
					
				return new NotifyCmdLine(	match.Groups[1].Value,
											match.Groups[2].Value);
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}

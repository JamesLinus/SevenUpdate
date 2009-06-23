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

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getproxysettings.asp
	/// </summary>
	[
		TypeConverter(typeof(JobProxySettingsTypeConverter)),
		Serializable,
		Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob_getproxysettings.asp")
	]
	public sealed class JobProxySettings : ICloneable, ISerializable
	{
		private ProxyUsage proxyUsage;
		private string proxyList;
		private string proxyBypassList;

		/// <summary>
		/// Constructor
		/// </summary>
		public JobProxySettings()
			: this(ProxyUsage.Preconfig, string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public JobProxySettings(ProxyUsage usage, string proxy, string proxyBypass)
		{
			proxyUsage = usage;
			proxyList = proxy;
			proxyBypassList = proxyBypass;
		}

		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is JobProxySettings)
			{
				JobProxySettings other = (JobProxySettings)obj;
				
				return	proxyUsage == other.proxyUsage &&
						proxyList == other.proxyList &&
						proxyBypassList == other.proxyBypassList;
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
			return string.Format(	Properties.Resources.JobProxySettingsToStringFormat,
									ProxyUsage,
									ProxyList,
									ProxyBypassList);
		}

		/// <summary>
		/// Specifies the proxy settings the job uses to transfer the files. For a list of proxy options, see the BG_JOB_PROXY_USAGE enumeration.
		/// </summary>
		[
			DefaultValue(ProxyUsage.Preconfig),
			Description("Specifies the proxy settings the job uses to transfer the files. For a list of proxy options, see the BG_JOB_PROXY_USAGE enumeration.")
		]
		public ProxyUsage ProxyUsage
		{
			get { return proxyUsage; }
			set { proxyUsage = value; }
		}

		/// <summary>
		/// String that contains one or more proxies to use to transfer files. The list is space-delimited. For details on the format of the string, see the Listing Proxy Servers section of Enabling Internet Functionality.
		/// </summary>
		[
			DefaultValue(""),
			Localizable(true),
			Description("String that contains one or more proxies to use to transfer files. The list is space-delimited. For details on the format of the string, see the Listing Proxy Servers section of Enabling Internet Functionality.")
		]
		public string ProxyList
		{
			get { return proxyList; }
			set
			{
				proxyList = value != null ? value : string.Empty;
			}
		}

		/// <summary>
		/// String that contains an optional list of host names or IP addresses, or both, that were not routed through the proxy. The list is space-delimited. For details on the format of the string, see the Listing the Proxy Bypass section of Enabling Internet Functionality.
		/// </summary>
		[
			DefaultValue(""),
			Localizable(true),
			Description("String that contains an optional list of host names or IP addresses, or both, that were not routed through the proxy. The list is space-delimited. For details on the format of the string, see the Listing the Proxy Bypass section of Enabling Internet Functionality.")
		]
		public string ProxyBypassList
		{
			get { return proxyBypassList; }
			set { proxyBypassList = value != null ? value : string.Empty; }
		}

		internal string ArgProxyList
		{
			get
			{
				return ProxyUsage == ProxyUsage.Override ? ProxyList : null;
			}
		}

		internal string ArgProxyBypassList
		{
			get
			{
				return ProxyUsage == ProxyUsage.Override ? ProxyBypassList : null;
			}
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new JobProxySettings(proxyUsage, proxyList, proxyBypassList);
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public JobProxySettings(SerializationInfo UpdateInfo, StreamingContext context) 
		{
			proxyUsage = (ProxyUsage)UpdateInfo.GetValue("ProxyUsage", typeof(ProxyUsage));
			proxyList = (string)UpdateInfo.GetValue("ProxyList", typeof(string));
			proxyBypassList = (string)UpdateInfo.GetValue("ProxyBypassList", typeof(string));
		}

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object.
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo UpdateInfo, StreamingContext context)
		{
			UpdateInfo.AddValue("ProxyUsage", proxyUsage);
			UpdateInfo.AddValue("ProxyList", proxyList);
			UpdateInfo.AddValue("ProxyBypassList", proxyBypassList);
		}

		#endregion
	}

	internal class JobProxySettingsTypeConverter : ExpandableObjectConverter
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
			else if (destinationType == typeof(InstanceDescriptor) && value is JobProxySettings)
			{
				JobProxySettings jps = (JobProxySettings)value;
				object[] args = null;
				MemberInfo mi;
				
				if (jps == new JobProxySettings())
					mi = typeof(JobProxySettings).GetConstructor(Type.EmptyTypes);
				else
				{
					mi = typeof(JobProxySettings).GetConstructor(	new Type[]
																	{
																		typeof(ProxyUsage),
																		typeof(string),
																		typeof(string)
																	});
					args = new object[]
									{
										jps.ProxyUsage,
										jps.ProxyList,
										jps.ProxyBypassList
									};
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
					return new JobProxySettings();

				Regex reg = new Regex(Properties.Resources.JobProxySettingsRegEx);
				Match match = reg.Match(s);

				if (!match.Success)
					throw new FormatException();

				return new JobProxySettings((ProxyUsage)Enum.Parse(	typeof(ProxyUsage),
																	match.Groups[1].Value),
											match.Groups[2].Value,
											match.Groups[3].Value);
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}

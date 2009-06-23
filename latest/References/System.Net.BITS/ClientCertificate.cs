/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_setclientcertificatebyid.asp
	/// </summary>
	[
		TypeConverter(typeof(ClientCertificateTypeConverter)),
		Serializable,
		Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/ibackgroundcopyjob4_setclientcertificatebyid.asp")
	]
	public sealed class ClientCertificate : ICloneable, ISerializable
	{
		/// <summary>
		/// An empty CertHashBlob
		/// </summary>
		public const string EmptyId = "0000000000000000000000000000000000000000";
		
		/// <summary>
		/// string CertHashBlob to byte[]
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static byte[] FromString(string p)
		{
			byte[] b = new byte[20];

			for (int i = 0; i < 20; i++)
				b[i] = byte.Parse(p.Substring(i * 2, 2), NumberStyles.HexNumber);

			return b;
		}

		/// <summary>
		/// byte[] CertHashBlob to string
		/// </summary>
		/// <param name="certHashBlob"></param>
		/// <returns></returns>
		public static string FormatBytes(byte[] certHashBlob)
		{
			if (certHashBlob.Length != 20)
				throw new ArgumentOutOfRangeException();
			
			StringBuilder sb = new StringBuilder(40);

			foreach (byte b in certHashBlob)
				sb.Append(b.ToString("X2"));

			return sb.ToString();
		}

		private CertStoreLocation storeLocation = CertStoreLocation.None;
		private string storeName = string.Empty;
		private string certHashBlob = EmptyId;
		private string subjectName = string.Empty;
		private bool setByName = false;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public ClientCertificate()
		{
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ClientCertificate(CertStoreLocation storeLocation, string storeName, byte[] certHashBlob)
		{
			this.storeLocation = storeLocation;
			this.StoreName = storeName;
			this.CertHashBlob = FormatBytes(certHashBlob);
			this.subjectName = string.Empty;
			this.setByName = false;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ClientCertificate(CertStoreLocation storeLocation, string storeName, string subjectName)
		{
			this.storeLocation = storeLocation;
			this.StoreName = storeName;
			this.certHashBlob = string.Empty;
			this.SubjectName = subjectName;
			this.setByName = true;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public ClientCertificate(	CertStoreLocation storeLocation,
									string storeName,
									string certHashBlob,
									string subjectName,
									bool setByName)
		{
			this.storeLocation = storeLocation;
			this.storeName = storeName;
			this.certHashBlob = certHashBlob;
			this.subjectName = subjectName;
			this.setByName = setByName;
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(	Properties.Resources.ClientCertificateToStringFormat,
									storeLocation,
									storeName,
									certHashBlob,
									subjectName,
									setByName);
		}

		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is ClientCertificate)
			{
				ClientCertificate other = (ClientCertificate)obj;

				return	storeLocation == other.storeLocation &&
						storeName == other.storeName &&
						certHashBlob == other.certHashBlob &&
						subjectName == other.subjectName &&
						setByName == other.setByName;
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
		/// Identifies the location of a system store to use for looking up the certificate.
		/// For possible values, see the BG_CERT_STORE_LOCATION enumeration.
		/// </summary>
		[
			Description("Identifies the location of a system store to use for looking up the certificate. For possible values, see the BG_CERT_STORE_LOCATION enumeration."),
			Category("Behavior"),
			DefaultValue(CertStoreLocation.None)
		]
		public CertStoreLocation StoreLocation
		{
			get { return storeLocation; }
			set { storeLocation = value; }
		}

		/// <summary>
		/// String that contains the name of the certificate store. The string is limited to 256 characters, including the null terminator.
		/// You can specify one of the following system stores or an application-defined store. The store can be a local or remote store.
		/// Value 	Meaning
		/// CA 		Certification authority certificates
		/// MY 		Personal certificates
		/// ROOT 	Root certificates
		/// SPC 	Software Publisher Certificate
		/// </summary>
		[
			Description("String that contains the name of the certificate store. The string is limited to 256 characters, including the null terminator. You can specify one of the following system stores or an application-defined store. The store can be a local or remote store.\r\nValue 	Meaning\r\nCA 		Certification authority certificates\r\nMY 		Personal certificate\r\nROOT 	Root certificates\r\nSPC 	Software Publisher Certificate"),
			Category("Behavior"),
			DefaultValue(""),
			Localizable(true)
		]
		public string StoreName
		{
			get { return storeName; }
			set { storeName = value != null ? value : string.Empty; }
		}
		
		/// <summary>
		/// SHA1 hash that identifies the certificate. Use a 20 byte buffer for the hash. For more information, see Remarks.
		/// </summary>
		[
			Description("SHA1 hash that identifies the certificate. Use a 20 byte buffer for the hash. For more information, see Remarks."),
			Category("Behavior"),
			DefaultValue("0000000000000000000000000000000000000000")
		]
		public string CertHashBlob
		{
			get { return certHashBlob; }
			set
			{
				if (value == null)
					certHashBlob = EmptyId;
				else
					certHashBlob = FormatBytes(FromString(value));
			}
		}

		internal byte[] ArgCertHashBlob
		{
			get { return FromString(certHashBlob); }
			set { certHashBlob = FormatBytes(value); }
		}
		
		/// <summary>
		/// String that contains the simple subject name of the certificate.
		/// If the subject name contains multiple relative distinguished names (RDNs), you can specify one or more adjacent RDNs.
		/// If you specify more than one RDN, the list is comma-delimited. The string is limited to 256 characters, including the null terminator.
		/// You cannot specify an empty subject name.
		/// Do not include the object identifier in the name. You must specify the RDNs in the reverse order from what the certificate displays.
		/// For example, if the subject name in the certificate is "CN=name1, OU=name2, O=name3", specify the subject name as "name3, name2, name1". 
		/// </summary>
		[
			Description("String that contains the simple subject name of the certificate. If the subject name contains multiple relative distinguished names (RDNs), you can specify one or more adjacent RDNs. If you specify more than one RDN, the list is comma-delimited. The string is limited to 256 characters, including the null terminator. You cannot specify an empty subject name. Do not include the object identifier in the name. You must specify the RDNs in the reverse order from what the certificate displays. For example, if the subject name in the certificate is \"CN=name1, OU=name2, O=name3\", specify the subject name as \"name3, name2, name1\"."),
			Category("Behavior"),
			DefaultValue(""),
			Localizable(true)
		]
		public string SubjectName
		{
			get { return subjectName; }
			set { subjectName = value != null ? value : string.Empty; }
		}
		
		/// <summary>
		/// Use the SubjectName and not the CertHashBlob to set the certificate on the job.
		/// </summary>
		[
			Description("Use the SubjectName and not the CertHashBlob to set the certificate on the job."),
			Category("Behavior"),
			DefaultValue(false)
		]
		public bool SetByName
		{
			get { return setByName; }
			set { setByName = value; }
		}

		internal void Reset()
		{
			StoreLocation = CertStoreLocation.None;
			SubjectName = string.Empty;
			CertHashBlob = EmptyId;
			StoreName = string.Empty;
			SetByName = false;
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new ClientCertificate(StoreLocation, StoreName, CertHashBlob, SubjectName, SetByName);
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public ClientCertificate(SerializationInfo UpdateInfo, StreamingContext context) 
        {
			StoreLocation = (CertStoreLocation)UpdateInfo.GetValue("StoreLocation", typeof(CertStoreLocation));
			StoreName = (string)UpdateInfo.GetValue("StoreName", typeof(string));
			CertHashBlob = (string)UpdateInfo.GetValue("CertHashBlob", typeof(string));
			SubjectName = (string)UpdateInfo.GetValue("SubjectName", typeof(string));
			SetByName = (bool)UpdateInfo.GetValue("SetByName", typeof(bool));
		}

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object.
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
        public void GetObjectData(SerializationInfo UpdateInfo, StreamingContext context)
        {
			UpdateInfo.AddValue("StoreLocation", StoreLocation);
			UpdateInfo.AddValue("StoreName", StoreName);
			UpdateInfo.AddValue("CertHashBlob", CertHashBlob);
			UpdateInfo.AddValue("SubjectName", SubjectName);
			UpdateInfo.AddValue("SetByName", SetByName);
		}

		#endregion
	}

	internal class ClientCertificateTypeConverter : ExpandableObjectConverter
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
			else if (destinationType == typeof(InstanceDescriptor))
			{
				if (value == null)
					return null;

				ClientCertificate cc = (ClientCertificate)value;
				MemberInfo Member = typeof(ClientCertificate).GetConstructor(	new Type[]
																				{
																					typeof(CertStoreLocation),
																					typeof(string),
																					typeof(string),
																					typeof(string),
																					typeof(bool)
																				});
				object[] Arguments =	new object[]
										{
											cc.StoreLocation,
											cc.StoreName,
											cc.CertHashBlob,
											cc.SubjectName,
											cc.SetByName
										};

				return new InstanceDescriptor(Member, Arguments);
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
					return new ClientCertificate();

				Regex reg = new Regex(Properties.Resources.ClientCertificateRegex);
				Match match = reg.Match(s);

				if (!match.Success)
					throw new FormatException();

				return new ClientCertificate(	(CertStoreLocation)Enum.Parse(	typeof(CertStoreLocation),
																				match.Groups[1].Value),
												match.Groups[2].Value,
												match.Groups[3].Value,
												match.Groups[4].Value,
												bool.Parse(match.Groups[5].Value));
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}

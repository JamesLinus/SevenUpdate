/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_file_range.asp
	/// </summary>
	[
		TypeConverter(typeof(FileRangeTypeConverter)),
		Serializable,
		Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_file_range.asp")
	]
	public class FileRange : ICloneable, ISerializable
	{
		/// <summary>
		/// To indicate that the range extends to the end of the file, specify BG_LENGTH_TO_EOF.
		/// </summary>
		public const long BG_LENGTH_TO_EOF = -1;
		private long initialOffset;
		private long length;

		/// <summary>
		/// Constructor
		/// </summary>
		public FileRange()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public FileRange(long initialOffset, long length)
		{
			this.initialOffset = initialOffset;
			this.length = length;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		internal FileRange(_BG_FILE_RANGE range)
		{
			this.initialOffset = range.InitialOffset;
			this.length = range.Length;
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(	Properties.Resources.FileRangeToStringFormat,
									initialOffset,
									length);
		}
		
		/// <summary>
		/// Zero-based offset to the beginning of the range of bytes to download from a file.
		/// </summary>
		[
			DefaultValue((long)0),
			Category("Behavior"),
			Description("Zero-based offset to the beginning of the range of bytes to download from a file.")
		]
		public long InitialOffset { get { return initialOffset; } set { initialOffset = value; } }

		/// <summary>
		/// Number of bytes in the range. To indicate that the range extends to the end of the file, specify BG_LENGTH_TO_EOF.
		/// </summary>
		[
			DefaultValue((long)0),
			Category("Behavior"),
			Description("Number of bytes in the range. To indicate that the range extends to the end of the file, specify BG_LENGTH_TO_EOF.")
		]
		public long Length { get { return length; } set { length = value; } }

		internal void CopyTo(ref _BG_FILE_RANGE range)
		{
			range.InitialOffset = initialOffset;
			range.Length = length;
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new FileRange(initialOffset, length);
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
		public FileRange(SerializationInfo UpdateInfo, StreamingContext context) 
        {
			initialOffset = (long)UpdateInfo.GetValue("InitialOffset", typeof(long));
			length = (long)UpdateInfo.GetValue("Length", typeof(long));
        }

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object.
		/// </summary>
		/// <param name="UpdateInfo"></param>
		/// <param name="context"></param>
        public void GetObjectData(SerializationInfo UpdateInfo, StreamingContext context)
        {
			UpdateInfo.AddValue("InitialOffset", initialOffset);
			UpdateInfo.AddValue("Length", length);
        }
	
		#endregion
	}
	
	internal class FileRangeTypeConverter : ExpandableObjectConverter
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
		        
				FileRange range = (FileRange)value;
				MemberInfo Member = typeof(FileRange).GetConstructor(	new Type[]
																		{
																			typeof(long),
																			typeof(long)
																		});
		        		        
				if (Member != null)
				{
					object[] Arguments = new object[] { range.InitialOffset, range.Length };

					return new InstanceDescriptor(Member, Arguments);
				}

				return null;
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
					return new FileRange();
					
				Regex reg = new Regex(Properties.Resources.FileRangeRegex);
				Match match = reg.Match(s);

				if (!match.Success)
					throw new FormatException();

				return new FileRange(	long.Parse(match.Groups[1].Value),
										long.Parse(match.Groups[2].Value));
			}

			return base.ConvertFrom(context, culture, value);
		}
	}
}

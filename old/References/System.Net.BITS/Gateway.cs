/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.ComponentModel;

namespace System.Net.BITS
{
	/// <summary>
	/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_gateway_info.asp
	/// </summary>
	[Description("http://msdn.microsoft.com/library/default.asp?url=/library/en-us/bits/bits/bg_gateway_info.asp")]
	public sealed class Gateway
	{
		/// <summary>
		/// Constructor
		/// </summary>
		internal Gateway(_BG_GATEWAY_INFO UpdateInfo)
		{
			UniqueDeviceName = UpdateInfo.UniqueDeviceName;
			FriendlyName = UpdateInfo.FriendlyName;
			Type = UpdateInfo.Type;
			PresentationURL = UpdateInfo.PresentationURL;
			ManufacturerName = UpdateInfo.ManufacturerName;
			ManufacturerURL = UpdateInfo.ManufacturerURL;
			ModelName = UpdateInfo.ModelName;
			ModelNumber = UpdateInfo.ModelNumber;
			Description = UpdateInfo.Description;
			ModelURL = UpdateInfo.ModelURL;
			UPC = UpdateInfo.UPC;
			SerialNumber = UpdateInfo.SerialNumber;
			BITSDeviceID =UpdateInfo.BITSDeviceID;
			InBytes = UpdateInfo.InBytes;
			OutBytes = UpdateInfo.OutBytes;
			SupportsCounters = UpdateInfo.SupportsCounters;
		}   

		/// <summary>
		/// Null-terminated string that contains the unique name of the device in the form, uuid:UUID
		/// </summary>
		public readonly string UniqueDeviceName;
		
		/// <summary>
		/// Null-terminated string that contains the name of the device to use in a user interface.
		/// </summary>
		public readonly string FriendlyName;
		
		/// <summary>
		/// Null-terminated string that contains the device type.
		/// </summary>
		public readonly string Type;
		
		/// <summary>
		/// Null-terminated string that contains the URL for a presentation of the device.
		/// </summary>
		public readonly string PresentationURL;
		
		/// <summary>
		/// Null-terminated string that contains the name of the manufacturer of the device.
		/// </summary>
		public readonly string ManufacturerName;
		
		/// <summary>
		/// Null-terminated string that contains the URL to the manufacturer's web site.
		/// </summary>
		public readonly string ManufacturerURL;
		
		/// <summary>
		/// Null-terminated string that contains the model name of the device.
		/// </summary>
		public readonly string ModelName;
		
		/// <summary>
		/// Null-terminated string that contains the model number of the device.
		/// </summary>
		public readonly string ModelNumber;
		
		/// <summary>
		/// Null-terminated string that contains a description of the device.
		/// </summary>
		public readonly string Description;
		
		/// <summary>
		/// Null-terminated string that contains the URL to a web site for more information about the model.
		/// </summary>
		public readonly string ModelURL;
		
		/// <summary>
		/// Null-terminated string that contains the universal product code of the device.
		/// </summary>
		public readonly string UPC;
		
		/// <summary>
		/// Null-terminated string that contains the manufacturer's serial number of the device.
		/// </summary>
		public readonly string SerialNumber;
		
		/// <summary>
		/// GUID that uniquely identifies this device to BITS.
		/// </summary>
		public readonly Guid BITSDeviceID;
		
		/// <summary>
		/// Number of bytes entering the gateway device.
		/// </summary>
		public readonly uint InBytes;
		
		/// <summary>
		/// Number of bytes leaving the gateway device.
		/// </summary>
		public readonly uint OutBytes;
		
		/// <summary>
		/// TRUE if the gateway device supports the InBytes and OutBytes counters, otherwise, FALSE.
		/// </summary>
		public readonly int SupportsCounters;
	}
}

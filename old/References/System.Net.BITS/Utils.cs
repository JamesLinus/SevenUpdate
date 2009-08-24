/*
 * Rodger Bernstein 2006
 * rodger.bernstein@cetialpha5.net
 * No warranty of any kind implied or otherwise. Use this software at your own peril.
 * 
 * Version: 1.0 - Initial release
*/
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace System.Net.BITS
{
	internal static class Utils
	{
		internal delegate void DoAction();

		internal static DateTime ToDateTime(_FILETIME time)
		{
			return DateTime.FromFileTime(((long)time.dwHighDateTime << 32) + time.dwLowDateTime);
		}

		internal static void Release<T>(ref T i, DoAction action) where T : class
		{
			try
			{
				action();
			}
			finally
			{
				if (i != null)
					Marshal.ReleaseComObject(i);
				
				i = null;
			}
		}

		internal static void TaskFree(ref IntPtr mem, DoAction action)
		{
			try
			{
				action();
			}
			finally
			{
				Marshal.FreeCoTaskMem(mem);
				mem = IntPtr.Zero;
			}
		}

		internal delegate U Factory<T, U>(T t);

		internal static void Walk<T, U>(ref IntPtr mem,
										int count,
										Factory<T, U> factory,
										out U[] list)
		{
			try
			{
				IntPtr walk = mem;

				list = new U[count];

				for (int i = 0; i < count; i++)
				{
					list[i] = factory((T)Marshal.PtrToStructure(walk, typeof(T)));
					walk = (IntPtr)((int)walk + Marshal.SizeOf(typeof(T)));
				}
			}
			finally
			{
				Marshal.FreeCoTaskMem(mem);
				mem = IntPtr.Zero;
			}
		}
	}
}

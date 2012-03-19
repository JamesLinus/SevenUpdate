// <copyright file="HeaderBlock.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Tar
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>This class is intended for internal use only, by the Tar library.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 512)]
    internal struct HeaderBlock
    {
        /// <summary>type of file</summary>
        public byte TypeFlag;

        /// <summary>name of file. A directory is indicated by a trailing slash (/) in its name.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] name;

        /// <summary>file mode</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] mode;

        /// <summary>owner user ID</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] uid;

        /// <summary>owner group ID</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] gid;

        /// <summary>length of file in bytes, encoded as octal digits in ASCII</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        private byte[] size;

        /// <summary>modify time of file</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        private byte[] modifiedTime;

        /// <summary>checksum for header (use all blanks for chksum itself, when calculating)</summary>
        /// <remarks>The checksum is calculated by taking the sum of the unsigned byte values of the header block with the eight checksum bytes taken to be ascii spaces (decimal value 32). It is stored as a six digit octal number with leading zeroes followed by a null and then a space.</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] checkSum;

        /// <summary>name of linked file (only if typeflag = '2')</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private byte[] linkName;

        /// <summary>USTAR indicator</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        private byte[] magic;

        /// <summary>USTAR version</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private byte[] version;

        /// <summary>owner user name</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private byte[] userName;

        /// <summary>owner group name</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        private byte[] groupName;

        /// <summary>device major number</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] devMajor;

        /// <summary>device minor number</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] devMinor;

        /// <summary>prefix for file name</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 155)]
        private byte[] prefix;

        /// <summary>Not used</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        private byte[] pad;

        public static HeaderBlock CreateHeaderBlock()
        {
            var hb = new HeaderBlock
                {
                    name = new byte[100], 
                    mode = new byte[8], 
                    uid = new byte[8], 
                    gid = new byte[8], 
                    size = new byte[12], 
                    modifiedTime = new byte[12], 
                    checkSum = new byte[8], 
                    linkName = new byte[100], 
                    magic = new byte[6], 
                    version = new byte[2], 
                    userName = new byte[32], 
                    groupName = new byte[32], 
                    devMajor = new byte[8], 
                    devMinor = new byte[8], 
                    prefix = new byte[155], 
                    pad = new byte[12], 
                };

            Array.Copy(Encoding.ASCII.GetBytes("ustar "), 0, hb.magic, 0, 6);
            hb.version[0] = hb.version[1] = (byte)TarEntryType.File;

            return hb;
        }

        public bool VerifyChksum()
        {
            int stored = this.GetChksum();
            int calculated = this.SetChksum();

            return stored == calculated;
        }

        /// <summary>The tar spec says that the Checksum must be stored as a six digit octal number with leading zeroes followed by a null and then a space.</summary>
        /// <remarks>Various implementations do not adhere to this, so reader programs should be flexible. A more compatible approach may be to use the first white-space-trimmed six digits for checksum. In addition, some older tar implementations treated bytes as signed. Readers must calculate the checksum both ways, and treat it as good if either the signed or unsigned sum matches the included checksum.</remarks>
        public int GetChksum()
        {
            // special case
            bool allZeros = true;
            Array.ForEach(
                this.checkSum, 
                x =>
                    {
                        if (x != 0)
                        {
                            allZeros = false;
                        }
                    });

            if (allZeros)
            {
                return 256;
            }

            // validation 6 and 7 have to be 0 and 0x20, in some order.
            if (
                !(((this.checkSum[6] == 0) && (this.checkSum[7] == 0x20))
                  || ((this.checkSum[7] == 0) && (this.checkSum[6] == 0x20))))
            {
                return -1;
            }

            string v = Encoding.ASCII.GetString(this.checkSum, 0, 6).Trim();
            return Convert.ToInt32(v, 8);
        }

        public int SetChksum()
        {
            // first set the checksum to all ASCII _space_ (dec 32)
            byte[] a = Encoding.ASCII.GetBytes(new string(' ', 8));
            Array.Copy(a, 0, this.checkSum, 0, a.Length); // always 8

            // then sum all the bytes
            const int rawSize = 512;
            IntPtr buffer = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(this, buffer, false);
            var block = new byte[rawSize];
            Marshal.Copy(buffer, block, 0, rawSize);
            Marshal.FreeHGlobal(buffer);

            // format as octal
            int sum = 0;
            Array.ForEach(block, x => sum += x);
            string s = "000000" + Convert.ToString(sum, 8);

            // put that into the checksum block
            a = Encoding.ASCII.GetBytes(s.Substring(s.Length - 6));
            Array.Copy(a, 0, this.checkSum, 0, a.Length); // always 6
            this.checkSum[6] = 0;
            this.checkSum[7] = 0x20;

            return sum;
        }

        public void SetSize(int sz)
        {
            string ssz = string.Format("          {0} ", Convert.ToString(sz, 8));

            // get last 12 chars
            byte[] a = Encoding.ASCII.GetBytes(ssz.Substring(ssz.Length - 12));
            Array.Copy(a, 0, this.size, 0, a.Length); // always 12
        }

        public int GetSize()
        {
            return Convert.ToInt32(Encoding.ASCII.GetString(this.size).TrimNull(), 8);
        }

        public void InsertLinkName(string linkName)
        {
            // if greater than 100, then an exception occurs
            byte[] a = Encoding.ASCII.GetBytes(linkName);
            Array.Copy(a, 0, this.linkName, 0, a.Length);
        }

        public void InsertName(string itemName, string fileName)
        {
            if (itemName.Length <= 100)
            {
                byte[] a = Encoding.ASCII.GetBytes(itemName);
                Array.Copy(a, 0, this.name, 0, a.Length);
            }
            else
            {
                byte[] a = Encoding.ASCII.GetBytes(itemName);
                Array.Copy(a, a.Length - 100, this.name, 0, 100);
                Array.Copy(a, 0, this.prefix, 0, a.Length - 100);
            }

            // insert the modified time for the file or directory, also
            DateTime dt = File.GetLastWriteTimeUtc(fileName);
            int timeT = TimeConverter.DateTime2TimeT(dt);
            string time = "     " + Convert.ToString(timeT, 8) + " ";
            byte[] a1 = Encoding.ASCII.GetBytes(time.Substring(time.Length - 12));
            Array.Copy(a1, 0, this.modifiedTime, 0, a1.Length); // always 12
        }

        public DateTime GetMtime()
        {
            int timeT = Convert.ToInt32(Encoding.ASCII.GetString(this.modifiedTime).TrimNull(), 8);
            return DateTime.SpecifyKind(TimeConverter.TimeT2DateTime(timeT), DateTimeKind.Utc);
        }

        public string GetName()
        {
            string n;
            string m = this.GetMagic();
            if (m != null && m.Equals("ustar"))
            {
                n = (this.prefix[0] == 0)
                        ? Encoding.ASCII.GetString(this.name).TrimNull()
                        : Encoding.ASCII.GetString(this.prefix).TrimNull()
                          + Encoding.ASCII.GetString(this.name).TrimNull();
            }
            else
            {
                n = Encoding.ASCII.GetString(this.name).TrimNull();
            }

            return n;
        }

        private string GetMagic()
        {
            string m = (this.magic[0] == 0) ? null : Encoding.ASCII.GetString(this.magic).Trim();
            return m;
        }
    }
}
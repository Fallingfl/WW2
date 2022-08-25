using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BO4Console
{
	// Token: 0x02000006 RID: 6
	public class memory
	{
		// Token: 0x06000015 RID: 21
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(uint dwAccess, bool inherit, int pid);

		// Token: 0x06000016 RID: 22
		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr handle);

		// Token: 0x06000017 RID: 23
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, [In] [Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesRead);

		// Token: 0x06000018 RID: 24
		[DllImport("kernel32.dll")]
		public static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, [In] [Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesWritten);

		// Token: 0x06000019 RID: 25
		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		// Token: 0x0600001A RID: 26
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x0600001B RID: 27 RVA: 0x0000269C File Offset: 0x0000089C
		~memory()
		{
			if (this.ProcessHandle != IntPtr.Zero)
			{
				memory.CloseHandle(this.ProcessHandle);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000026E0 File Offset: 0x000008E0
		public bool AttackProcess(string _ProcessName)
		{
			Process[] processesByName = Process.GetProcessesByName(_ProcessName);
			if (processesByName.Length != 0)
			{
				this.BaseModule = processesByName[0].MainModule.BaseAddress;
				this.CurProcess = processesByName[0];
				this.ProcessID = processesByName[0].Id;
				this.ProcessName = _ProcessName;
				this.ProcessHandle = memory.OpenProcess(56U, false, this.ProcessID);
				return this.ProcessHandle != IntPtr.Zero;
			}
			return false;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002754 File Offset: 0x00000954
		internal float GetPointerInt(long v1, float[] v2, int v3)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000275B File Offset: 0x0000095B
		public bool IsOpen()
		{
			return !(this.ProcessName == string.Empty) && this.AttackProcess(this.ProcessName);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002784 File Offset: 0x00000984
		internal static IntPtr GetBaseAddress(string ProcessName)
		{
			IntPtr result;
			try
			{
				result = Process.GetProcessesByName(ProcessName)[0].MainModule.BaseAddress;
			}
			catch
			{
				result = IntPtr.Zero;
			}
			return result;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000027C0 File Offset: 0x000009C0
		public long FindPattern(long startAddress, long endAddress, string pattern, string mask)
		{
			byte[] array = new byte[endAddress - startAddress];
			IntPtr intPtr;
			memory.ReadProcessMemory(this.ProcessHandle, startAddress, array, (ulong)((long)array.Length), out intPtr);
			for (long num = 0L; num < (long)array.Length; num += 1L)
			{
				int num2 = 0;
				while (num2 < pattern.Length && ((char)array[(int)(checked((IntPtr)(unchecked(num + (long)num2))))] == pattern[num2] || mask[num2] == '?'))
				{
					if (num2 == pattern.Length - 1)
					{
						return startAddress + num;
					}
					num2++;
				}
			}
			return -1L;
		}

		// Token: 0x06000021 RID: 33
		[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
		protected static extern bool ReadProcessMemory2(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesRead);

		// Token: 0x06000022 RID: 34 RVA: 0x0000283C File Offset: 0x00000A3C
		private static byte[] HX2Bts(string HXS)
		{
			HXS = Regex.Replace(HXS, "[^a-fA-F0-9?]", "");
			if (HXS.Length % 2 != 0)
			{
				HXS = HXS.Substring(0, HXS.Length - 1);
			}
			byte[] array = new byte[HXS.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)((HXS.Substring(i * 2, 2) != "??") ? byte.Parse(HXS.Substring(i * 2, 2), NumberStyles.HexNumber) : 63);
			}
			return array;
		}

		// Token: 0x06000023 RID: 35
		[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
		private static extern bool WriteProcessMemory2(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, [Out] int lpNumberOfBytesWritten);

		// Token: 0x06000024 RID: 36
		[DllImport("kernel32")]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

		// Token: 0x06000025 RID: 37
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

		// Token: 0x06000026 RID: 38 RVA: 0x000028C4 File Offset: 0x00000AC4
		public int ReadPointerInt(long add, long[] offsets, int level)
		{
			long lpBaseAddress = add;
			for (int i = 0; i < level; i++)
			{
				lpBaseAddress = this.ReadInt64(lpBaseAddress) + offsets[i];
			}
			return this.ReadInt32(lpBaseAddress);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000028F4 File Offset: 0x00000AF4
		public long GetPointerInt(long add, long[] offsets, int level)
		{
			long num = add;
			for (int i = 0; i < level; i++)
			{
				num = this.ReadInt64(num) + offsets[i];
			}
			return num;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000291C File Offset: 0x00000B1C
		public int ReadInt32(long _lpBaseAddress)
		{
			byte[] array = new byte[4];
			IntPtr intPtr;
			memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, 4UL, out intPtr);
			return BitConverter.ToInt32(array, 0);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x0000294C File Offset: 0x00000B4C
		public uint ReadUInt32(long _lpBaseAddress)
		{
			byte[] array = new byte[4];
			IntPtr intPtr;
			memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, 4UL, out intPtr);
			return BitConverter.ToUInt32(array, 0);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x0000297C File Offset: 0x00000B7C
		public long ReadInt64(long _lpBaseAddress)
		{
			byte[] array = new byte[8];
			IntPtr intPtr;
			memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, 8UL, out intPtr);
			return BitConverter.ToInt64(array, 0);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000029AC File Offset: 0x00000BAC
		public ulong ReadUInt64(long _lpBaseAddress)
		{
			byte[] array = new byte[8];
			IntPtr intPtr;
			memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, 8UL, out intPtr);
			return BitConverter.ToUInt64(array, 0);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000029DC File Offset: 0x00000BDC
		public float ReadFloat(long _lpBaseAddress)
		{
			byte[] array = new byte[4];
			IntPtr intPtr;
			memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, 4UL, out intPtr);
			return BitConverter.ToSingle(array, 0);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002A0C File Offset: 0x00000C0C
		public string ReadString(long _lpBaseAddress, ulong _Size)
		{
			byte[] array = new byte[_Size];
			IntPtr intPtr;
			memory.ReadProcessMemory(this.ProcessHandle, _lpBaseAddress, array, _Size, out intPtr);
			return Encoding.UTF8.GetString(array);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002A40 File Offset: 0x00000C40
		public void WriteMemory(long MemoryAddress, byte[] Buffer)
		{
			uint num;
			memory.VirtualProtectEx(this.ProcessHandle, (IntPtr)MemoryAddress, (uint)Buffer.Length, 4U, out num);
			IntPtr intPtr;
			memory.WriteProcessMemory(this.ProcessHandle, MemoryAddress, Buffer, (ulong)Buffer.Length, out intPtr);
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002A7C File Offset: 0x00000C7C
		public void WriteInt32(long _lpBaseAddress, int _Value)
		{
			byte[] bytes = BitConverter.GetBytes(_Value);
			this.WriteMemory(_lpBaseAddress, bytes);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002A98 File Offset: 0x00000C98
		public void WriteInt64(long _lpBaseAddress, long _Value)
		{
			byte[] bytes = BitConverter.GetBytes(_Value);
			this.WriteMemory(_lpBaseAddress, bytes);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002AB4 File Offset: 0x00000CB4
		public void WriteUInt32(long _lpBaseAddress, uint _Value)
		{
			byte[] bytes = BitConverter.GetBytes(_Value);
			this.WriteMemory(_lpBaseAddress, bytes);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002AD0 File Offset: 0x00000CD0
		public void WriteFloat(long _lpBaseAddress, float _Value)
		{
			byte[] bytes = BitConverter.GetBytes(_Value);
			this.WriteMemory(_lpBaseAddress, bytes);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002AEC File Offset: 0x00000CEC
		public void WriteByte(long _lpBaseAddress, byte _Value)
		{
			byte[] bytes = BitConverter.GetBytes((short)_Value);
			IntPtr zero = IntPtr.Zero;
			memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, bytes, (ulong)(bytes.Length - 1), out zero);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002B1C File Offset: 0x00000D1C
		public void WriteXBytes(long _lpBaseAddress, byte[] _Value)
		{
			IntPtr zero = IntPtr.Zero;
			memory.WriteProcessMemory(this.ProcessHandle, _lpBaseAddress, _Value, (ulong)_Value.Length, out zero);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002B48 File Offset: 0x00000D48
		public void WriteString(long Address, string Text)
		{
			byte[] bytes = new ASCIIEncoding().GetBytes(Text);
			IntPtr zero = IntPtr.Zero;
			memory.WriteProcessMemory(this.ProcessHandle, Address, bytes, (ulong)this.ReadString(Address, 1024UL).Length, out zero);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002B8C File Offset: 0x00000D8C
		public void WriteNOP(long Address)
		{
			byte[] array = new byte[]
			{
				144,
				144,
				144,
				144,
				144
			};
			IntPtr zero = IntPtr.Zero;
			memory.WriteProcessMemory(this.ProcessHandle, Address, array, (ulong)array.Length, out zero);
		}

		// Token: 0x0400001C RID: 28
		public const uint PROCESS_VM_READ = 16U;

		// Token: 0x0400001D RID: 29
		public const uint PROCESS_VM_WRITE = 32U;

		// Token: 0x0400001E RID: 30
		public const uint PROCESS_VM_OPERATION = 8U;

		// Token: 0x0400001F RID: 31
		public const uint PAGE_READWRITE = 4U;

		// Token: 0x04000020 RID: 32
		private Process CurProcess;

		// Token: 0x04000021 RID: 33
		private IntPtr ProcessHandle;

		// Token: 0x04000022 RID: 34
		private string ProcessName;

		// Token: 0x04000023 RID: 35
		private int ProcessID;

		// Token: 0x04000024 RID: 36
		public IntPtr BaseModule;
	}
}

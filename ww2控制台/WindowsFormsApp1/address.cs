using System;

namespace BO4Console
{
	// Token: 0x02000004 RID: 4
	internal class address
	{
		// Token: 0x06000008 RID: 8 RVA: 0x000020B0 File Offset: 0x000002B0
		public static void setadd()
		{
			memory memory = new memory();
			address.baseadd = memory.GetBaseAddress("s2_mp64_ship").ToInt64();
			memory.AttackProcess("s2_mp64_ship");
		}

		// Token: 0x04000004 RID: 4
		public static long baseadd;

		// Token: 0x04000005 RID: 5
		public static long vita;
	}
}

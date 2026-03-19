using System;

namespace Tera_RBG
{
	// Token: 0x02000005 RID: 5
	internal class ThreadLocal<T>
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00004C52 File Offset: 0x00002E52
		public ThreadLocal(Func<Random> value)
		{
			this.Value = value;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00004C64 File Offset: 0x00002E64
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00004C6C File Offset: 0x00002E6C
		public object Value { get; internal set; }
	}
}

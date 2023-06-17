using System;
using System.Collections.Generic;
using Facepunch;

// Token: 0x02000949 RID: 2377
public class PooledList<T>
{
	// Token: 0x04003372 RID: 13170
	public List<T> data;

	// Token: 0x06003924 RID: 14628 RVA: 0x00154306 File Offset: 0x00152506
	public void Alloc()
	{
		if (this.data == null)
		{
			this.data = Pool.GetList<T>();
		}
	}

	// Token: 0x06003925 RID: 14629 RVA: 0x0015431B File Offset: 0x0015251B
	public void Free()
	{
		if (this.data != null)
		{
			Pool.FreeList<T>(ref this.data);
		}
	}

	// Token: 0x06003926 RID: 14630 RVA: 0x00154330 File Offset: 0x00152530
	public void Clear()
	{
		if (this.data != null)
		{
			this.data.Clear();
		}
	}
}

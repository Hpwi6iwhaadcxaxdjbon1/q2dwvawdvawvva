using System;

// Token: 0x02000284 RID: 644
public class WeakpointProperties : PrefabAttribute
{
	// Token: 0x040015A4 RID: 5540
	public bool BlockWhenRoofAttached;

	// Token: 0x06001CF4 RID: 7412 RVA: 0x000C87B9 File Offset: 0x000C69B9
	protected override Type GetIndexedType()
	{
		return typeof(WeakpointProperties);
	}
}

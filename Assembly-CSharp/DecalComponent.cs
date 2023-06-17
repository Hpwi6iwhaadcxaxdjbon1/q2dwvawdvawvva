using System;

// Token: 0x020002B6 RID: 694
public abstract class DecalComponent : PrefabAttribute
{
	// Token: 0x06001D65 RID: 7525 RVA: 0x000CAB0D File Offset: 0x000C8D0D
	protected override Type GetIndexedType()
	{
		return typeof(DecalComponent);
	}
}

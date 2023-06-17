using System;

// Token: 0x0200024F RID: 591
public class AttractionPoint : PrefabAttribute
{
	// Token: 0x040014DC RID: 5340
	public string groupName;

	// Token: 0x06001C25 RID: 7205 RVA: 0x000C431F File Offset: 0x000C251F
	protected override Type GetIndexedType()
	{
		return typeof(AttractionPoint);
	}
}

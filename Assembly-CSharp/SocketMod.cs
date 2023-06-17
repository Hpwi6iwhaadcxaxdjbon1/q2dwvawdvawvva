using System;

// Token: 0x02000270 RID: 624
public class SocketMod : PrefabAttribute
{
	// Token: 0x0400155A RID: 5466
	[NonSerialized]
	public Socket_Base baseSocket;

	// Token: 0x0400155B RID: 5467
	public Translate.Phrase FailedPhrase;

	// Token: 0x06001CA5 RID: 7333 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool DoCheck(Construction.Placement place)
	{
		return false;
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ModifyPlacement(Construction.Placement place)
	{
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x000C6D64 File Offset: 0x000C4F64
	protected override Type GetIndexedType()
	{
		return typeof(SocketMod);
	}
}

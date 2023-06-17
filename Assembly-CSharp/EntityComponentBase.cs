using System;
using Network;

// Token: 0x020003C2 RID: 962
public class EntityComponentBase : BaseMonoBehaviour
{
	// Token: 0x06002182 RID: 8578 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	protected virtual BaseEntity GetBaseEntity()
	{
		return null;
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void SaveComponent(BaseNetworkable.SaveInfo info)
	{
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void LoadComponent(BaseNetworkable.LoadInfo info)
	{
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x00007A3C File Offset: 0x00005C3C
	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}
}

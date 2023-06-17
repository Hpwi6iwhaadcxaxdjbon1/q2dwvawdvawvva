using System;
using Facepunch;
using Network;
using ProtoBuf;

// Token: 0x020000B1 RID: 177
public class PlayerModifiers : BaseModifiers<global::BasePlayer>
{
	// Token: 0x06001029 RID: 4137 RVA: 0x00087228 File Offset: 0x00085428
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerModifiers.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x00087268 File Offset: 0x00085468
	public override void ServerUpdate(BaseCombatEntity ownerEntity)
	{
		base.ServerUpdate(ownerEntity);
		this.SendChangesToClient();
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x00087278 File Offset: 0x00085478
	public ProtoBuf.PlayerModifiers Save()
	{
		ProtoBuf.PlayerModifiers playerModifiers = Pool.Get<ProtoBuf.PlayerModifiers>();
		playerModifiers.modifiers = Pool.GetList<ProtoBuf.Modifier>();
		foreach (global::Modifier modifier in this.All)
		{
			if (modifier != null)
			{
				playerModifiers.modifiers.Add(modifier.Save());
			}
		}
		return playerModifiers;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x000872EC File Offset: 0x000854EC
	public void Load(ProtoBuf.PlayerModifiers m)
	{
		base.RemoveAll();
		if (m == null || m.modifiers == null)
		{
			return;
		}
		foreach (ProtoBuf.Modifier modifier in m.modifiers)
		{
			if (modifier != null)
			{
				global::Modifier modifier2 = new global::Modifier();
				modifier2.Init((global::Modifier.ModifierType)modifier.type, (global::Modifier.ModifierSource)modifier.source, modifier.value, modifier.duration, modifier.timeRemaing);
				base.Add(modifier2);
			}
		}
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x00087380 File Offset: 0x00085580
	public void SendChangesToClient()
	{
		if (!this.dirty)
		{
			return;
		}
		base.SetDirty(false);
		using (ProtoBuf.PlayerModifiers playerModifiers = this.Save())
		{
			base.baseEntity.ClientRPCPlayer<ProtoBuf.PlayerModifiers>(null, base.baseEntity, "UpdateModifiers", playerModifiers);
		}
	}
}

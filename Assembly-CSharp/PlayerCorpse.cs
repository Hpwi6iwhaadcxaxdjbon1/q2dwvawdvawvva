using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class PlayerCorpse : global::LootableCorpse
{
	// Token: 0x04001D3A RID: 7482
	public Buoyancy buoyancy;

	// Token: 0x04001D3B RID: 7483
	public const global::BaseEntity.Flags Flag_Buoyant = global::BaseEntity.Flags.Reserved6;

	// Token: 0x04001D3C RID: 7484
	public uint underwearSkin;

	// Token: 0x060024D3 RID: 9427 RVA: 0x00003F9B File Offset: 0x0000219B
	public bool IsBuoyant()
	{
		return base.HasFlag(global::BaseEntity.Flags.Reserved6);
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x000E96E0 File Offset: 0x000E78E0
	public override bool OnStartBeingLooted(global::BasePlayer baseEntity)
	{
		return (!baseEntity.InSafeZone() || baseEntity.userID == this.playerSteamID) && base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x000E9704 File Offset: 0x000E7904
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.buoyancy == null)
		{
			Debug.LogWarning("Player corpse has no buoyancy assigned, searching at runtime :" + base.name);
			this.buoyancy = base.GetComponent<Buoyancy>();
		}
		if (this.buoyancy != null)
		{
			this.buoyancy.SubmergedChanged = new Action<bool>(this.BuoyancyChanged);
			this.buoyancy.forEntity = this;
		}
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x000E9777 File Offset: 0x000E7977
	public void BuoyancyChanged(bool isSubmerged)
	{
		if (this.IsBuoyant())
		{
			return;
		}
		base.SetFlag(global::BaseEntity.Flags.Reserved6, isSubmerged, false, false);
		base.SendNetworkUpdate_Flags();
	}

	// Token: 0x060024D7 RID: 9431 RVA: 0x000E9798 File Offset: 0x000E7998
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.lootableCorpse != null)
		{
			info.msg.lootableCorpse.underwearSkin = this.underwearSkin;
		}
		if (base.isServer && this.containers != null && this.containers.Length > 1 && !info.forDisk)
		{
			info.msg.storageBox = Pool.Get<StorageBox>();
			info.msg.storageBox.contents = this.containers[1].Save();
		}
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000E981F File Offset: 0x000E7A1F
	public override string Categorize()
	{
		return "playercorpse";
	}
}

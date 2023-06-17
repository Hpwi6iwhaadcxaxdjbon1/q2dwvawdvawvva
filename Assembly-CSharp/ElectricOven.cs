using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020003D4 RID: 980
public class ElectricOven : global::BaseOven
{
	// Token: 0x04001A3B RID: 6715
	public GameObjectRef IoEntity;

	// Token: 0x04001A3C RID: 6716
	public Transform IoEntityAnchor;

	// Token: 0x04001A3D RID: 6717
	private EntityRef<global::IOEntity> spawnedIo;

	// Token: 0x04001A3E RID: 6718
	private bool resumeCookingWhenPowerResumes;

	// Token: 0x060021C0 RID: 8640 RVA: 0x000DBBB7 File Offset: 0x000D9DB7
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.SpawnIOEnt();
		}
	}

	// Token: 0x060021C1 RID: 8641 RVA: 0x000DBBCC File Offset: 0x000D9DCC
	private void SpawnIOEnt()
	{
		if (this.IoEntity.isValid && this.IoEntityAnchor != null)
		{
			global::IOEntity ioentity = GameManager.server.CreateEntity(this.IoEntity.resourcePath, this.IoEntityAnchor.position, this.IoEntityAnchor.rotation, true) as global::IOEntity;
			ioentity.SetParent(this, true, false);
			ioentity.Spawn();
			this.spawnedIo.Set(ioentity);
		}
	}

	// Token: 0x170002CC RID: 716
	// (get) Token: 0x060021C2 RID: 8642 RVA: 0x000DBC41 File Offset: 0x000D9E41
	protected override bool CanRunWithNoFuel
	{
		get
		{
			return this.spawnedIo.IsValid(true) && this.spawnedIo.Get(true).IsPowered();
		}
	}

	// Token: 0x060021C3 RID: 8643 RVA: 0x000DBC64 File Offset: 0x000D9E64
	public void OnIOEntityFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		if (!next.HasFlag(global::BaseEntity.Flags.Reserved8) && base.IsOn())
		{
			this.StopCooking();
			this.resumeCookingWhenPowerResumes = true;
			return;
		}
		if (next.HasFlag(global::BaseEntity.Flags.Reserved8) && !base.IsOn() && this.resumeCookingWhenPowerResumes)
		{
			this.StartCooking();
			this.resumeCookingWhenPowerResumes = false;
		}
	}

	// Token: 0x060021C4 RID: 8644 RVA: 0x000DBCD4 File Offset: 0x000D9ED4
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.simpleUID == null)
		{
			info.msg.simpleUID = Pool.Get<SimpleUID>();
		}
		info.msg.simpleUID.uid = this.spawnedIo.uid;
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x000DBD20 File Offset: 0x000D9F20
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			this.spawnedIo.uid = info.msg.simpleUID.uid;
		}
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x000DBD51 File Offset: 0x000D9F51
	protected override bool CanPickupOven()
	{
		return this.children.Count == 1;
	}
}

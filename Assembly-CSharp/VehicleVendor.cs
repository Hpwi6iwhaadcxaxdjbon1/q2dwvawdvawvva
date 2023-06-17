using System;
using Facepunch;
using ProtoBuf;

// Token: 0x0200019D RID: 413
public class VehicleVendor : NPCTalking
{
	// Token: 0x0400111C RID: 4380
	public EntityRef spawnerRef;

	// Token: 0x0400111D RID: 4381
	public VehicleSpawner vehicleSpawner;

	// Token: 0x06001859 RID: 6233 RVA: 0x000B6198 File Offset: 0x000B4398
	public override string GetConversationStartSpeech(global::BasePlayer player)
	{
		if (base.ProviderBusy())
		{
			return "startbusy";
		}
		return "intro";
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x000B61AD File Offset: 0x000B43AD
	public VehicleSpawner GetVehicleSpawner()
	{
		if (!this.spawnerRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.spawnerRef.Get(base.isServer).GetComponent<VehicleSpawner>();
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x000B61DC File Offset: 0x000B43DC
	public override void UpdateFlags()
	{
		base.UpdateFlags();
		VehicleSpawner vehicleSpawner = this.GetVehicleSpawner();
		bool b = vehicleSpawner != null && vehicleSpawner.IsPadOccupied();
		base.SetFlag(global::BaseEntity.Flags.Reserved1, b, false, true);
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x000B6218 File Offset: 0x000B4418
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.spawnerRef.IsValid(true) && this.vehicleSpawner == null)
		{
			this.vehicleSpawner = this.GetVehicleSpawner();
			return;
		}
		if (this.vehicleSpawner != null && !this.spawnerRef.IsValid(true))
		{
			this.spawnerRef.Set(this.vehicleSpawner);
		}
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x000B6281 File Offset: 0x000B4481
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleVendor = Pool.Get<ProtoBuf.VehicleVendor>();
		info.msg.vehicleVendor.spawnerRef = this.spawnerRef.uid;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x000B62B5 File Offset: 0x000B44B5
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vehicleVendor != null)
		{
			this.spawnerRef.id_cached = info.msg.vehicleVendor.spawnerRef;
		}
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x00080E24 File Offset: 0x0007F024
	public override ConversationData GetConversationFor(global::BasePlayer player)
	{
		return this.conversations[0];
	}
}

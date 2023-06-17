using System;
using ConVar;
using Rust;
using UnityEngine;

// Token: 0x02000425 RID: 1061
public class SupplyDrop : LootContainer
{
	// Token: 0x04001C05 RID: 7173
	public GameObjectRef parachutePrefab;

	// Token: 0x04001C06 RID: 7174
	private const BaseEntity.Flags FlagNightLight = BaseEntity.Flags.Reserved1;

	// Token: 0x04001C07 RID: 7175
	private BaseEntity parachute;

	// Token: 0x060023E0 RID: 9184 RVA: 0x000E53B4 File Offset: 0x000E35B4
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			if (this.parachutePrefab.isValid)
			{
				this.parachute = GameManager.server.CreateEntity(this.parachutePrefab.resourcePath, default(Vector3), default(Quaternion), true);
			}
			if (this.parachute)
			{
				this.parachute.SetParent(this, "parachute_attach", false, false);
				this.parachute.Spawn();
			}
		}
		this.isLootable = false;
		base.Invoke(new Action(this.MakeLootable), 300f);
		base.InvokeRepeating(new Action(this.CheckNightLight), 0f, 30f);
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x000E546E File Offset: 0x000E366E
	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && Rust.Application.isLoadingSave)
		{
			if (this.parachute != null)
			{
				Debug.LogWarning("More than one child entity was added to SupplyDrop! Expected only the parachute.", this);
			}
			this.parachute = child;
		}
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x000E54A6 File Offset: 0x000E36A6
	private void RemoveParachute()
	{
		if (this.parachute)
		{
			this.parachute.Kill(BaseNetworkable.DestroyMode.None);
			this.parachute = null;
		}
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x000E54C8 File Offset: 0x000E36C8
	public void MakeLootable()
	{
		this.isLootable = true;
	}

	// Token: 0x060023E4 RID: 9188 RVA: 0x000E54D1 File Offset: 0x000E36D1
	private void OnCollisionEnter(Collision collision)
	{
		if ((1 << collision.collider.gameObject.layer & 1084293393) > 0)
		{
			this.RemoveParachute();
			this.MakeLootable();
		}
	}

	// Token: 0x060023E5 RID: 9189 RVA: 0x000E54FF File Offset: 0x000E36FF
	private void CheckNightLight()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, Env.time > 20f || Env.time < 7f, false, true);
	}
}

using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class NPCShopKeeper : NPCPlayer
{
	// Token: 0x04001117 RID: 4375
	public EntityRef invisibleVendingMachineRef;

	// Token: 0x04001118 RID: 4376
	public InvisibleVendingMachine machine;

	// Token: 0x04001119 RID: 4377
	private float greetDir;

	// Token: 0x0400111A RID: 4378
	private Vector3 initialFacingDir;

	// Token: 0x0400111B RID: 4379
	private global::BasePlayer lastWavedAtPlayer;

	// Token: 0x0600184D RID: 6221 RVA: 0x000B5DFA File Offset: 0x000B3FFA
	public InvisibleVendingMachine GetVendingMachine()
	{
		if (!this.invisibleVendingMachineRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.invisibleVendingMachineRef.Get(base.isServer).GetComponent<InvisibleVendingMachine>();
	}

	// Token: 0x0600184E RID: 6222 RVA: 0x000B5E28 File Offset: 0x000B4028
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube(base.transform.position + Vector3.up * 1f, new Vector3(0.5f, 1f, 0.5f));
	}

	// Token: 0x0600184F RID: 6223 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void UpdateProtectionFromClothing()
	{
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Hurt(HitInfo info)
	{
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x000B5E78 File Offset: 0x000B4078
	public override void ServerInit()
	{
		base.ServerInit();
		this.initialFacingDir = base.transform.rotation * Vector3.forward;
		base.Invoke(new Action(this.DelayedSleepEnd), 3f);
		this.SetAimDirection(base.transform.rotation * Vector3.forward);
		base.InvokeRandomized(new Action(this.Greeting), UnityEngine.Random.Range(5f, 10f), 5f, UnityEngine.Random.Range(0f, 2f));
		if (this.invisibleVendingMachineRef.IsValid(true) && this.machine == null)
		{
			this.machine = this.GetVendingMachine();
			return;
		}
		if (this.machine != null && !this.invisibleVendingMachineRef.IsValid(true))
		{
			this.invisibleVendingMachineRef.Set(this.machine);
		}
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x000B5F63 File Offset: 0x000B4163
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.shopKeeper = Pool.Get<ShopKeeper>();
		info.msg.shopKeeper.vendingRef = this.invisibleVendingMachineRef.uid;
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x000B5F97 File Offset: 0x000B4197
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.shopKeeper != null)
		{
			this.invisibleVendingMachineRef.uid = info.msg.shopKeeper.vendingRef;
		}
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x000B5FC8 File Offset: 0x000B41C8
	public override void PostServerLoad()
	{
		base.PostServerLoad();
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x000B5FD0 File Offset: 0x000B41D0
	public void DelayedSleepEnd()
	{
		this.EndSleeping();
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x000B5FD8 File Offset: 0x000B41D8
	public void GreetPlayer(global::BasePlayer player)
	{
		if (player != null)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.Gesture, "wave", null);
			this.SetAimDirection(Vector3Ex.Direction2D(player.eyes.position, this.eyes.position));
			this.lastWavedAtPlayer = player;
			return;
		}
		this.SetAimDirection(this.initialFacingDir);
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x000B6034 File Offset: 0x000B4234
	public void Greeting()
	{
		List<global::BasePlayer> list = Pool.GetList<global::BasePlayer>();
		Vis.Entities<global::BasePlayer>(base.transform.position, 10f, list, 131072, QueryTriggerInteraction.Collide);
		Vector3 position = base.transform.position;
		global::BasePlayer basePlayer = null;
		foreach (global::BasePlayer basePlayer2 in list)
		{
			if (!basePlayer2.isClient && !basePlayer2.IsNpc && !(basePlayer2 == this) && basePlayer2.IsVisible(this.eyes.position, float.PositiveInfinity) && !(basePlayer2 == this.lastWavedAtPlayer) && Vector3.Dot(Vector3Ex.Direction2D(basePlayer2.eyes.position, this.eyes.position), this.initialFacingDir) >= 0.2f)
			{
				basePlayer = basePlayer2;
				break;
			}
		}
		if (basePlayer == null && !list.Contains(this.lastWavedAtPlayer))
		{
			this.lastWavedAtPlayer = null;
		}
		if (basePlayer != null)
		{
			base.SignalBroadcast(global::BaseEntity.Signal.Gesture, "wave", null);
			this.SetAimDirection(Vector3Ex.Direction2D(basePlayer.eyes.position, this.eyes.position));
			this.lastWavedAtPlayer = basePlayer;
		}
		else
		{
			this.SetAimDirection(this.initialFacingDir);
		}
		Pool.FreeList<global::BasePlayer>(ref list);
	}
}

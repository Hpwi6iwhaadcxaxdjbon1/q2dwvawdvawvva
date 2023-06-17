using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class JunkPile : BaseEntity
{
	// Token: 0x04000862 RID: 2146
	public GameObjectRef sinkEffect;

	// Token: 0x04000863 RID: 2147
	public SpawnGroup[] spawngroups;

	// Token: 0x04000864 RID: 2148
	public NPCSpawner NPCSpawn;

	// Token: 0x04000865 RID: 2149
	private const float lifetimeMinutes = 30f;

	// Token: 0x04000866 RID: 2150
	protected bool isSinking;

	// Token: 0x06000D16 RID: 3350 RVA: 0x000707D0 File Offset: 0x0006E9D0
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("JunkPile.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00070810 File Offset: 0x0006EA10
	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.TimeOut), 1800f);
		base.InvokeRepeating(new Action(this.CheckEmpty), 10f, 30f);
		base.Invoke(new Action(this.SpawnInitial), 1f);
		this.isSinking = false;
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x00070874 File Offset: 0x0006EA74
	private void SpawnInitial()
	{
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x000708A0 File Offset: 0x0006EAA0
	public bool SpawnGroupsEmpty()
	{
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].currentPopulation > 0)
			{
				return false;
			}
		}
		return !(this.NPCSpawn != null) || this.NPCSpawn.currentPopulation <= 0;
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x000708EE File Offset: 0x0006EAEE
	public void CheckEmpty()
	{
		if (this.SpawnGroupsEmpty() && !this.PlayersNearby())
		{
			base.CancelInvoke(new Action(this.CheckEmpty));
			this.SinkAndDestroy();
		}
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x00070918 File Offset: 0x0006EB18
	public bool PlayersNearby()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(base.transform.position, this.TimeoutPlayerCheckRadius(), list, 131072, QueryTriggerInteraction.Collide);
		bool result = false;
		foreach (BasePlayer basePlayer in list)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && !(basePlayer is HumanNPC))
			{
				result = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return result;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x000709A8 File Offset: 0x0006EBA8
	public virtual float TimeoutPlayerCheckRadius()
	{
		return 15f;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x000709AF File Offset: 0x0006EBAF
	public void TimeOut()
	{
		if (this.PlayersNearby())
		{
			base.Invoke(new Action(this.TimeOut), 30f);
			return;
		}
		this.SpawnGroupsEmpty();
		this.SinkAndDestroy();
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x000709E0 File Offset: 0x0006EBE0
	public void SinkAndDestroy()
	{
		base.CancelInvoke(new Action(this.SinkAndDestroy));
		SpawnGroup[] array = this.spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		base.SetFlag(BaseEntity.Flags.Reserved8, true, true, true);
		if (this.NPCSpawn != null)
		{
			this.NPCSpawn.Clear();
		}
		base.ClientRPC(null, "CLIENT_StartSink");
		base.transform.position -= new Vector3(0f, 5f, 0f);
		this.isSinking = true;
		base.Invoke(new Action(this.KillMe), 22f);
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x00003384 File Offset: 0x00001584
	public void KillMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}
}

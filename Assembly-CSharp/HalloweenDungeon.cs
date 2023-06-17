using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class HalloweenDungeon : BasePortal
{
	// Token: 0x040010E8 RID: 4328
	public GameObjectRef dungeonPrefab;

	// Token: 0x040010E9 RID: 4329
	public EntityRef<ProceduralDynamicDungeon> dungeonInstance;

	// Token: 0x040010EA RID: 4330
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 0f;

	// Token: 0x040010EB RID: 4331
	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float lifetime = 600f;

	// Token: 0x040010EC RID: 4332
	private float secondsUsed;

	// Token: 0x040010ED RID: 4333
	private float timeAlive;

	// Token: 0x040010EE RID: 4334
	public AnimationCurve radiationCurve;

	// Token: 0x040010EF RID: 4335
	public Translate.Phrase collapsePhrase;

	// Token: 0x040010F0 RID: 4336
	public Translate.Phrase mountPhrase;

	// Token: 0x040010F1 RID: 4337
	private bool anyplayers_cached;

	// Token: 0x040010F2 RID: 4338
	private float nextPlayerCheckTime = float.NegativeInfinity;

	// Token: 0x060017FD RID: 6141 RVA: 0x000B46E5 File Offset: 0x000B28E5
	public virtual float GetLifetime()
	{
		return HalloweenDungeon.lifetime;
	}

	// Token: 0x060017FE RID: 6142 RVA: 0x000B46EC File Offset: 0x000B28EC
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.ioEntity != null)
		{
			this.dungeonInstance.uid = info.msg.ioEntity.genericEntRef3;
			this.secondsUsed = info.msg.ioEntity.genericFloat1;
			this.timeAlive = info.msg.ioEntity.genericFloat2;
		}
	}

	// Token: 0x060017FF RID: 6143 RVA: 0x000B475C File Offset: 0x000B295C
	public float GetLifeFraction()
	{
		return Mathf.Clamp01(this.secondsUsed / this.GetLifetime());
	}

	// Token: 0x06001800 RID: 6144 RVA: 0x000B4770 File Offset: 0x000B2970
	public void Update()
	{
		if (base.isClient)
		{
			return;
		}
		if (this.secondsUsed > 0f)
		{
			this.secondsUsed += Time.deltaTime;
		}
		this.timeAlive += Time.deltaTime;
		float lifeFraction = this.GetLifeFraction();
		if (this.dungeonInstance.IsValid(true))
		{
			ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
			float value = this.radiationCurve.Evaluate(lifeFraction) * 80f;
			proceduralDynamicDungeon.exitRadiation.RadiationAmountOverride = Mathf.Clamp(value, 0f, float.PositiveInfinity);
		}
		if (lifeFraction >= 1f)
		{
			this.KillIfNoPlayers();
			return;
		}
		if (this.timeAlive > 3600f && this.secondsUsed == 0f)
		{
			this.ClearAllEntitiesInRadius(80f);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001801 RID: 6145 RVA: 0x000B4840 File Offset: 0x000B2A40
	public void KillIfNoPlayers()
	{
		if (!this.AnyPlayersInside())
		{
			this.ClearAllEntitiesInRadius(80f);
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x06001802 RID: 6146 RVA: 0x000B485C File Offset: 0x000B2A5C
	public bool AnyPlayersInside()
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			this.anyplayers_cached = false;
		}
		else if (Time.time > this.nextPlayerCheckTime)
		{
			this.nextPlayerCheckTime = Time.time + 10f;
			this.anyplayers_cached = global::BaseNetworkable.HasCloseConnections(proceduralDynamicDungeon.transform.position, 80f);
		}
		return this.anyplayers_cached;
	}

	// Token: 0x06001803 RID: 6147 RVA: 0x000B48C8 File Offset: 0x000B2AC8
	private void ClearAllEntitiesInRadius(float radius)
	{
		ProceduralDynamicDungeon proceduralDynamicDungeon = this.dungeonInstance.Get(true);
		if (proceduralDynamicDungeon == null)
		{
			return;
		}
		List<global::BaseEntity> list = Pool.GetList<global::BaseEntity>();
		Vis.Entities<global::BaseEntity>(proceduralDynamicDungeon.transform.position, radius, list, -1, QueryTriggerInteraction.Collide);
		foreach (global::BaseEntity baseEntity in list)
		{
			if (baseEntity.IsValid() && !baseEntity.IsDestroyed)
			{
				global::LootableCorpse lootableCorpse;
				if ((lootableCorpse = (baseEntity as global::LootableCorpse)) != null)
				{
					lootableCorpse.blockBagDrop = true;
				}
				baseEntity.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
		Pool.FreeList<global::BaseEntity>(ref list);
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x000B4974 File Offset: 0x000B2B74
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		}
		info.msg.ioEntity.genericEntRef3 = this.dungeonInstance.uid;
		info.msg.ioEntity.genericFloat1 = this.secondsUsed;
		info.msg.ioEntity.genericFloat2 = this.timeAlive;
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x000B49EC File Offset: 0x000B2BEC
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.timeAlive += UnityEngine.Random.Range(0f, 60f);
	}

	// Token: 0x06001806 RID: 6150 RVA: 0x000B4A10 File Offset: 0x000B2C10
	public override void UsePortal(global::BasePlayer player)
	{
		if (this.GetLifeFraction() > 0.8f)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, this.collapsePhrase, Array.Empty<string>());
			return;
		}
		if (player.isMounted)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, this.mountPhrase, Array.Empty<string>());
			return;
		}
		if (this.secondsUsed == 0f)
		{
			this.secondsUsed = 1f;
		}
		base.UsePortal(player);
	}

	// Token: 0x06001807 RID: 6151 RVA: 0x00063622 File Offset: 0x00061822
	public override void Spawn()
	{
		base.Spawn();
	}

	// Token: 0x06001808 RID: 6152 RVA: 0x000B4A78 File Offset: 0x000B2C78
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.timeAlive = UnityEngine.Random.Range(0f, 60f);
			this.SpawnSubEntities();
		}
		this.localEntryExitPos.DropToGround(false, 10f);
		this.localEntryExitPos.transform.position += Vector3.up * 0.05f;
		base.Invoke(new Action(this.CheckBlocked), 0.25f);
	}

	// Token: 0x06001809 RID: 6153 RVA: 0x000B4B00 File Offset: 0x000B2D00
	public void CheckBlocked()
	{
		float num = 0.5f;
		float num2 = 1.8f;
		Vector3 position = this.localEntryExitPos.position;
		Vector3 start = position + new Vector3(0f, num, 0f);
		Vector3 end = position + new Vector3(0f, num2 - num, 0f);
		if (Physics.CheckCapsule(start, end, num, 1537286401))
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600180A RID: 6154 RVA: 0x000B4B6C File Offset: 0x000B2D6C
	public static Vector3 GetDungeonSpawnPoint()
	{
		float num = Mathf.Floor(TerrainMeta.Size.x / 200f);
		float num2 = 1000f;
		Vector3 zero = Vector3.zero;
		zero.x = -Mathf.Min(TerrainMeta.Size.x * 0.5f, 4000f) + 200f;
		zero.y = 1025f;
		zero.z = -Mathf.Min(TerrainMeta.Size.z * 0.5f, 4000f) + 200f;
		Vector3 zero2 = Vector3.zero;
		int num3 = 0;
		while ((float)num3 < num2)
		{
			int num4 = 0;
			while ((float)num4 < num)
			{
				Vector3 vector = zero + new Vector3((float)num4 * 200f, (float)num3 * 100f, 0f);
				bool flag = false;
				foreach (ProceduralDynamicDungeon proceduralDynamicDungeon in ProceduralDynamicDungeon.dungeons)
				{
					if (proceduralDynamicDungeon != null && proceduralDynamicDungeon.isServer && Vector3.Distance(proceduralDynamicDungeon.transform.position, vector) < 10f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return vector;
				}
				num4++;
			}
			num3++;
		}
		return Vector3.zero;
	}

	// Token: 0x0600180B RID: 6155 RVA: 0x000B4CCC File Offset: 0x000B2ECC
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (this.dungeonInstance.IsValid(true))
		{
			this.dungeonInstance.Get(true).Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600180C RID: 6156 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedDestroy()
	{
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x000B4CF4 File Offset: 0x000B2EF4
	public void SpawnSubEntities()
	{
		Vector3 dungeonSpawnPoint = HalloweenDungeon.GetDungeonSpawnPoint();
		if (dungeonSpawnPoint == Vector3.zero)
		{
			Debug.LogError("No dungeon spawn point");
			base.Invoke(new Action(this.DelayedDestroy), 5f);
			return;
		}
		global::BaseEntity baseEntity = GameManager.server.CreateEntity(this.dungeonPrefab.resourcePath, dungeonSpawnPoint, Quaternion.identity, true);
		ProceduralDynamicDungeon component = baseEntity.GetComponent<ProceduralDynamicDungeon>();
		component.mapOffset = base.transform.position - dungeonSpawnPoint;
		baseEntity.Spawn();
		this.dungeonInstance.Set(component);
		BasePortal exitPortal = component.GetExitPortal();
		this.targetPortal = exitPortal;
		exitPortal.targetPortal = this;
		base.LinkPortal();
		exitPortal.LinkPortal();
	}
}

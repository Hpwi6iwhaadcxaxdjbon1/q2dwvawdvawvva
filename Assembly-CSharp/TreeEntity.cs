using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class TreeEntity : ResourceEntity, IPrefabPreProcess
{
	// Token: 0x04000CBB RID: 3259
	[Header("Falling")]
	public bool fallOnKilled = true;

	// Token: 0x04000CBC RID: 3260
	public float fallDuration = 1.5f;

	// Token: 0x04000CBD RID: 3261
	public GameObjectRef fallStartSound;

	// Token: 0x04000CBE RID: 3262
	public GameObjectRef fallImpactSound;

	// Token: 0x04000CBF RID: 3263
	public GameObjectRef fallImpactParticles;

	// Token: 0x04000CC0 RID: 3264
	public SoundDefinition fallLeavesLoopDef;

	// Token: 0x04000CC1 RID: 3265
	[NonSerialized]
	public bool[] usedHeights = new bool[20];

	// Token: 0x04000CC2 RID: 3266
	public bool impactSoundPlayed;

	// Token: 0x04000CC3 RID: 3267
	private float treeDistanceUponFalling;

	// Token: 0x04000CC4 RID: 3268
	public GameObjectRef prefab;

	// Token: 0x04000CC5 RID: 3269
	public bool hasBonusGame = true;

	// Token: 0x04000CC6 RID: 3270
	public GameObjectRef bonusHitEffect;

	// Token: 0x04000CC7 RID: 3271
	public GameObjectRef bonusHitSound;

	// Token: 0x04000CC8 RID: 3272
	public Collider serverCollider;

	// Token: 0x04000CC9 RID: 3273
	public Collider clientCollider;

	// Token: 0x04000CCA RID: 3274
	public SoundDefinition smallCrackSoundDef;

	// Token: 0x04000CCB RID: 3275
	public SoundDefinition medCrackSoundDef;

	// Token: 0x04000CCC RID: 3276
	private float lastAttackDamage;

	// Token: 0x04000CCD RID: 3277
	[NonSerialized]
	protected BaseEntity xMarker;

	// Token: 0x04000CCE RID: 3278
	private int currentBonusLevel;

	// Token: 0x04000CCF RID: 3279
	private float lastDirection = -1f;

	// Token: 0x04000CD0 RID: 3280
	private float lastHitTime;

	// Token: 0x04000CD1 RID: 3281
	private int lastHitMarkerIndex = -1;

	// Token: 0x04000CD2 RID: 3282
	private float nextBirdTime;

	// Token: 0x04000CD3 RID: 3283
	private uint birdCycleIndex;

	// Token: 0x06001449 RID: 5193 RVA: 0x000A0580 File Offset: 0x0009E780
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("TreeEntity.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x0600144A RID: 5194 RVA: 0x000A05C0 File Offset: 0x0009E7C0
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x0600144B RID: 5195 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public override float BoundsPadding()
	{
		return 1f;
	}

	// Token: 0x0600144C RID: 5196 RVA: 0x000A05C8 File Offset: 0x0009E7C8
	public override void ServerInit()
	{
		base.ServerInit();
		this.lastDirection = (float)((UnityEngine.Random.Range(0, 2) == 0) ? -1 : 1);
		TreeManager.OnTreeSpawned(this);
	}

	// Token: 0x0600144D RID: 5197 RVA: 0x000A05EA File Offset: 0x0009E7EA
	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.CleanupMarker();
		TreeManager.OnTreeDestroyed(this);
	}

	// Token: 0x0600144E RID: 5198 RVA: 0x000A0600 File Offset: 0x0009E800
	public bool DidHitMarker(HitInfo info)
	{
		if (this.xMarker == null)
		{
			return false;
		}
		if (PrefabAttribute.server.Find<TreeMarkerData>(this.prefabID) != null)
		{
			Bounds bounds = new Bounds(this.xMarker.transform.position, Vector3.one * 0.2f);
			if (bounds.Contains(info.HitPositionWorld))
			{
				return true;
			}
		}
		else
		{
			Vector3 lhs = Vector3Ex.Direction2D(base.transform.position, this.xMarker.transform.position);
			Vector3 attackNormal = info.attackNormal;
			float num = Vector3.Dot(lhs, attackNormal);
			float num2 = Vector3.Distance(this.xMarker.transform.position, info.HitPositionWorld);
			if (num >= 0.3f && num2 <= 0.2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600144F RID: 5199 RVA: 0x000A06C7 File Offset: 0x0009E8C7
	public void StartBonusGame()
	{
		if (base.IsInvoking(new Action(this.StopBonusGame)))
		{
			base.CancelInvoke(new Action(this.StopBonusGame));
		}
		base.Invoke(new Action(this.StopBonusGame), 60f);
	}

	// Token: 0x06001450 RID: 5200 RVA: 0x000A0706 File Offset: 0x0009E906
	public void StopBonusGame()
	{
		this.CleanupMarker();
		this.lastHitTime = 0f;
		this.currentBonusLevel = 0;
	}

	// Token: 0x06001451 RID: 5201 RVA: 0x000A0720 File Offset: 0x0009E920
	public bool BonusActive()
	{
		return this.xMarker != null;
	}

	// Token: 0x06001452 RID: 5202 RVA: 0x000A0730 File Offset: 0x0009E930
	private void DoBirds()
	{
		if (base.isClient)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextBirdTime)
		{
			return;
		}
		if (this.bounds.extents.y < 6f)
		{
			return;
		}
		uint num = (uint)this.net.ID.Value + this.birdCycleIndex;
		if (SeedRandom.Range(ref num, 0, 2) == 0)
		{
			Effect.server.Run("assets/prefabs/npc/birds/birdemission.prefab", base.transform.position + Vector3.up * UnityEngine.Random.Range(this.bounds.extents.y * 0.65f, this.bounds.extents.y * 0.9f), Vector3.up, null, false);
		}
		this.birdCycleIndex += 1U;
		this.nextBirdTime = UnityEngine.Time.realtimeSinceStartup + 90f;
	}

	// Token: 0x06001453 RID: 5203 RVA: 0x000A0810 File Offset: 0x0009EA10
	public override void OnAttacked(HitInfo info)
	{
		bool canGather = info.CanGather;
		float num = UnityEngine.Time.time - this.lastHitTime;
		this.lastHitTime = UnityEngine.Time.time;
		this.DoBirds();
		if (!this.hasBonusGame || !canGather || info.Initiator == null || (this.BonusActive() && !this.DidHitMarker(info)))
		{
			base.OnAttacked(info);
			return;
		}
		if (this.xMarker != null && !info.DidGather && info.gatherScale > 0f)
		{
			this.xMarker.ClientRPC<int>(null, "MarkerHit", this.currentBonusLevel);
			this.currentBonusLevel++;
			info.gatherScale = 1f + Mathf.Clamp((float)this.currentBonusLevel * 0.125f, 0f, 1f);
		}
		Vector3 vector = (this.xMarker != null) ? this.xMarker.transform.position : info.HitPositionWorld;
		this.CleanupMarker();
		TreeMarkerData treeMarkerData = PrefabAttribute.server.Find<TreeMarkerData>(this.prefabID);
		if (treeMarkerData != null)
		{
			Vector3 direction;
			Vector3 vector2 = treeMarkerData.GetNearbyPoint(base.transform.InverseTransformPoint(vector), ref this.lastHitMarkerIndex, out direction);
			vector2 = base.transform.TransformPoint(vector2);
			Quaternion rot = QuaternionEx.LookRotationNormal(base.transform.TransformDirection(direction), default(Vector3));
			this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking_nospherecast.prefab", vector2, rot, true);
		}
		else
		{
			Vector3 vector3 = Vector3Ex.Direction2D(base.transform.position, vector);
			Vector3 a = Vector3.Cross(vector3, Vector3.up);
			float d = this.lastDirection;
			float t = UnityEngine.Random.Range(0.5f, 0.5f);
			Vector3 vector4 = Vector3.Lerp(-vector3, a * d, t);
			Vector3 vector5 = base.transform.InverseTransformDirection(vector4.normalized) * 2.5f;
			vector5 = base.transform.InverseTransformPoint(this.GetCollider().ClosestPoint(base.transform.TransformPoint(vector5)));
			Vector3 aimFrom = base.transform.TransformPoint(vector5);
			Vector3 vector6 = base.transform.InverseTransformPoint(info.HitPositionWorld);
			vector5.y = vector6.y;
			Vector3 vector7 = base.transform.InverseTransformPoint(info.Initiator.CenterPoint());
			float min = Mathf.Max(0.75f, vector7.y);
			float max = vector7.y + 0.5f;
			vector5.y = Mathf.Clamp(vector5.y + UnityEngine.Random.Range(0.1f, 0.2f) * ((UnityEngine.Random.Range(0, 2) == 0) ? -1f : 1f), min, max);
			Vector3 vector8 = Vector3Ex.Direction2D(base.transform.position, aimFrom);
			Vector3 a2 = vector8;
			vector8 = base.transform.InverseTransformDirection(vector8);
			Quaternion rot2 = QuaternionEx.LookRotationNormal(-vector8, Vector3.zero);
			vector5 = base.transform.TransformPoint(vector5);
			rot2 = QuaternionEx.LookRotationNormal(-a2, Vector3.zero);
			vector5 = this.GetCollider().ClosestPoint(vector5);
			Line line = new Line(this.GetCollider().transform.TransformPoint(new Vector3(0f, 10f, 0f)), this.GetCollider().transform.TransformPoint(new Vector3(0f, -10f, 0f)));
			rot2 = QuaternionEx.LookRotationNormal(-Vector3Ex.Direction(line.ClosestPoint(vector5), vector5), default(Vector3));
			this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking.prefab", vector5, rot2, true);
		}
		this.xMarker.Spawn();
		if (num > 5f)
		{
			this.StartBonusGame();
		}
		base.OnAttacked(info);
		if (this.health > 0f)
		{
			this.lastAttackDamage = info.damageTypes.Total();
			int num2 = Mathf.CeilToInt(this.health / this.lastAttackDamage);
			if (num2 < 2)
			{
				base.ClientRPC<int>(null, "CrackSound", 1);
				return;
			}
			if (num2 < 5)
			{
				base.ClientRPC<int>(null, "CrackSound", 0);
			}
		}
	}

	// Token: 0x06001454 RID: 5204 RVA: 0x000A0C38 File Offset: 0x0009EE38
	public void CleanupMarker()
	{
		if (this.xMarker)
		{
			this.xMarker.Kill(BaseNetworkable.DestroyMode.None);
		}
		this.xMarker = null;
	}

	// Token: 0x06001455 RID: 5205 RVA: 0x000A0C5C File Offset: 0x0009EE5C
	public Collider GetCollider()
	{
		if (base.isServer)
		{
			if (!(this.serverCollider == null))
			{
				return this.serverCollider;
			}
			return base.GetComponentInChildren<CapsuleCollider>();
		}
		else
		{
			if (!(this.clientCollider == null))
			{
				return this.clientCollider;
			}
			return base.GetComponent<Collider>();
		}
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x000A0CA8 File Offset: 0x0009EEA8
	public override void OnKilled(HitInfo info)
	{
		if (this.isKilled)
		{
			return;
		}
		this.isKilled = true;
		this.CleanupMarker();
		Analytics.Server.TreeKilled(info.WeaponPrefab);
		if (this.fallOnKilled)
		{
			Collider collider = this.GetCollider();
			if (collider)
			{
				collider.enabled = false;
			}
			Vector3 vector = info.attackNormal;
			if (vector == Vector3.zero)
			{
				vector = Vector3Ex.Direction2D(base.transform.position, info.PointStart);
			}
			base.ClientRPC<Vector3>(null, "TreeFall", vector);
			base.Invoke(new Action(this.DelayedKill), this.fallDuration + 1f);
			return;
		}
		this.DelayedKill();
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x00003384 File Offset: 0x00001584
	public void DelayedKill()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x000A0D51 File Offset: 0x0009EF51
	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			this.globalBroadcast = ConVar.Tree.global_broadcast;
		}
	}
}

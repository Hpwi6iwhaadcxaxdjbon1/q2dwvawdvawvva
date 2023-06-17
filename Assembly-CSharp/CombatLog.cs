using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

// Token: 0x02000452 RID: 1106
public class CombatLog
{
	// Token: 0x04001D24 RID: 7460
	private const string selfname = "you";

	// Token: 0x04001D25 RID: 7461
	private const string noname = "N/A";

	// Token: 0x04001D26 RID: 7462
	private BasePlayer player;

	// Token: 0x04001D27 RID: 7463
	private Queue<CombatLog.Event> storage;

	// Token: 0x04001D29 RID: 7465
	private static Dictionary<ulong, Queue<CombatLog.Event>> players = new Dictionary<ulong, Queue<CombatLog.Event>>();

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x060024B7 RID: 9399 RVA: 0x000E8AFA File Offset: 0x000E6CFA
	// (set) Token: 0x060024B8 RID: 9400 RVA: 0x000E8B02 File Offset: 0x000E6D02
	public float LastActive { get; private set; }

	// Token: 0x060024B9 RID: 9401 RVA: 0x000E8B0B File Offset: 0x000E6D0B
	public CombatLog(BasePlayer player)
	{
		this.player = player;
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x000E8B1A File Offset: 0x000E6D1A
	public void Init()
	{
		this.storage = CombatLog.Get(this.player.userID);
		this.LastActive = this.storage.LastOrDefault<CombatLog.Event>().time;
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x000063A5 File Offset: 0x000045A5
	public void Save()
	{
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x000E8B48 File Offset: 0x000E6D48
	public void LogInvalid(BasePlayer player, AttackEntity weapon, string description)
	{
		this.Log(player, weapon, null, description, null, -1, -1f, null);
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x000E8B68 File Offset: 0x000E6D68
	public void LogInvalid(HitInfo info, string description)
	{
		this.Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, -1f, info);
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000E8BA8 File Offset: 0x000E6DA8
	public void LogAttack(HitInfo info, string description, float oldHealth = -1f)
	{
		this.Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, oldHealth, info);
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x000E8BE4 File Offset: 0x000E6DE4
	public void Log(BaseEntity attacker, AttackEntity weapon, BaseCombatEntity hitEntity, string description, Projectile projectilePrefab = null, int projectileId = -1, float healthOld = -1f, HitInfo hitInfo = null)
	{
		CombatLog.Event val = default(CombatLog.Event);
		float distance = 0f;
		if (hitInfo != null)
		{
			distance = (hitInfo.IsProjectile() ? hitInfo.ProjectileDistance : Vector3.Distance(hitInfo.PointStart, hitInfo.HitPositionWorld));
			BasePlayer basePlayer;
			if ((basePlayer = (hitInfo.Initiator as BasePlayer)) != null && hitInfo.HitEntity != hitInfo.Initiator)
			{
				val.attacker_dead = (basePlayer.IsDead() || basePlayer.IsWounded());
			}
		}
		float health_new = (hitEntity != null) ? hitEntity.Health() : 0f;
		val.time = UnityEngine.Time.realtimeSinceStartup;
		val.attacker_id = ((attacker != null && attacker.net != null) ? attacker.net.ID : default(NetworkableId)).Value;
		val.target_id = ((hitEntity != null && hitEntity.net != null) ? hitEntity.net.ID : default(NetworkableId)).Value;
		val.attacker = ((this.player == attacker) ? "you" : (((attacker != null) ? attacker.ShortPrefabName : null) ?? "N/A"));
		val.target = ((this.player == hitEntity) ? "you" : (((hitEntity != null) ? hitEntity.ShortPrefabName : null) ?? "N/A"));
		val.weapon = ((weapon != null) ? weapon.name : "N/A");
		val.ammo = ((projectilePrefab != null) ? ((projectilePrefab != null) ? projectilePrefab.name : null) : "N/A");
		val.bone = (((hitInfo != null) ? hitInfo.boneName : null) ?? "N/A");
		val.area = ((hitInfo != null) ? hitInfo.boneArea : ((HitArea)0));
		val.distance = distance;
		val.health_old = ((healthOld == -1f) ? 0f : healthOld);
		val.health_new = health_new;
		val.info = (description ?? string.Empty);
		val.proj_hits = ((hitInfo != null) ? hitInfo.ProjectileHits : 0);
		val.proj_integrity = ((hitInfo != null) ? hitInfo.ProjectileIntegrity : 0f);
		val.proj_travel = ((hitInfo != null) ? hitInfo.ProjectileTravelTime : 0f);
		val.proj_mismatch = ((hitInfo != null) ? hitInfo.ProjectileTrajectoryMismatch : 0f);
		BasePlayer basePlayer2 = attacker as BasePlayer;
		BasePlayer.FiredProjectile firedProjectile;
		if (basePlayer2 != null && projectilePrefab != null && basePlayer2.firedProjectiles.TryGetValue(projectileId, out firedProjectile))
		{
			val.desync = (int)(firedProjectile.desyncLifeTime * 1000f);
		}
		this.Log(val);
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000E8EB0 File Offset: 0x000E70B0
	private void Log(CombatLog.Event val)
	{
		this.LastActive = UnityEngine.Time.realtimeSinceStartup;
		if (this.storage == null)
		{
			return;
		}
		this.storage.Enqueue(val);
		int num = Mathf.Max(0, Server.combatlogsize);
		while (this.storage.Count > num)
		{
			this.storage.Dequeue();
		}
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000E8F08 File Offset: 0x000E7108
	public string Get(int count, NetworkableId filterByAttacker = default(NetworkableId), bool json = false, bool isAdmin = false, ulong requestingUser = 0UL)
	{
		if (this.storage == null)
		{
			return string.Empty;
		}
		if (this.storage.Count == 0 && !json)
		{
			return "Combat log empty.";
		}
		TextTable textTable = new TextTable();
		textTable.AddColumn("time");
		textTable.AddColumn("attacker");
		textTable.AddColumn("id");
		textTable.AddColumn("target");
		textTable.AddColumn("id");
		textTable.AddColumn("weapon");
		textTable.AddColumn("ammo");
		textTable.AddColumn("area");
		textTable.AddColumn("distance");
		textTable.AddColumn("old_hp");
		textTable.AddColumn("new_hp");
		textTable.AddColumn("info");
		textTable.AddColumn("hits");
		textTable.AddColumn("integrity");
		textTable.AddColumn("travel");
		textTable.AddColumn("mismatch");
		textTable.AddColumn("desync");
		int num = this.storage.Count - count;
		int combatlogdelay = Server.combatlogdelay;
		int num2 = 0;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		foreach (CombatLog.Event @event in this.storage)
		{
			if (num > 0)
			{
				num--;
			}
			else if ((!filterByAttacker.IsValid || @event.attacker_id == filterByAttacker.Value) && (!(activeGameMode != null) || activeGameMode.returnValidCombatlog || isAdmin || @event.proj_hits <= 0))
			{
				float num3 = UnityEngine.Time.realtimeSinceStartup - @event.time;
				if (num3 >= (float)combatlogdelay)
				{
					string text = num3.ToString("0.00s");
					string attacker = @event.attacker;
					ulong num4 = @event.attacker_id;
					string text2 = num4.ToString();
					string target = @event.target;
					num4 = @event.target_id;
					string text3 = num4.ToString();
					string weapon = @event.weapon;
					string ammo = @event.ammo;
					string text4 = HitAreaUtil.Format(@event.area).ToLower();
					float num5 = @event.distance;
					string text5 = num5.ToString("0.0m");
					num5 = @event.health_old;
					string text6 = num5.ToString("0.0");
					num5 = @event.health_new;
					string text7 = num5.ToString("0.0");
					string text8 = @event.info;
					if (!this.player.IsDestroyed && this.player.userID == requestingUser && @event.attacker_dead)
					{
						text8 = "you died first (" + text8 + ")";
					}
					int num6 = @event.proj_hits;
					string text9 = num6.ToString();
					num5 = @event.proj_integrity;
					string text10 = num5.ToString("0.00");
					num5 = @event.proj_travel;
					string text11 = num5.ToString("0.00s");
					num5 = @event.proj_mismatch;
					string text12 = num5.ToString("0.00m");
					num6 = @event.desync;
					string text13 = num6.ToString();
					textTable.AddRow(new string[]
					{
						text,
						attacker,
						text2,
						target,
						text3,
						weapon,
						ammo,
						text4,
						text5,
						text6,
						text7,
						text8,
						text9,
						text10,
						text11,
						text12,
						text13
					});
				}
				else
				{
					num2++;
				}
			}
		}
		string text14;
		if (json)
		{
			text14 = textTable.ToJson();
		}
		else
		{
			text14 = textTable.ToString();
			if (num2 > 0)
			{
				text14 = string.Concat(new object[]
				{
					text14,
					"+ ",
					num2,
					" ",
					(num2 > 1) ? "events" : "event"
				});
				text14 = string.Concat(new object[]
				{
					text14,
					" in the last ",
					combatlogdelay,
					" ",
					(combatlogdelay > 1) ? "seconds" : "second"
				});
			}
		}
		return text14;
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x000E9334 File Offset: 0x000E7534
	public static Queue<CombatLog.Event> Get(ulong id)
	{
		Queue<CombatLog.Event> queue;
		if (CombatLog.players.TryGetValue(id, out queue))
		{
			return queue;
		}
		queue = new Queue<CombatLog.Event>();
		CombatLog.players.Add(id, queue);
		return queue;
	}

	// Token: 0x02000CE8 RID: 3304
	public struct Event
	{
		// Token: 0x04004586 RID: 17798
		public float time;

		// Token: 0x04004587 RID: 17799
		public ulong attacker_id;

		// Token: 0x04004588 RID: 17800
		public ulong target_id;

		// Token: 0x04004589 RID: 17801
		public string attacker;

		// Token: 0x0400458A RID: 17802
		public string target;

		// Token: 0x0400458B RID: 17803
		public string weapon;

		// Token: 0x0400458C RID: 17804
		public string ammo;

		// Token: 0x0400458D RID: 17805
		public string bone;

		// Token: 0x0400458E RID: 17806
		public HitArea area;

		// Token: 0x0400458F RID: 17807
		public float distance;

		// Token: 0x04004590 RID: 17808
		public float health_old;

		// Token: 0x04004591 RID: 17809
		public float health_new;

		// Token: 0x04004592 RID: 17810
		public string info;

		// Token: 0x04004593 RID: 17811
		public int proj_hits;

		// Token: 0x04004594 RID: 17812
		public float proj_integrity;

		// Token: 0x04004595 RID: 17813
		public float proj_travel;

		// Token: 0x04004596 RID: 17814
		public float proj_mismatch;

		// Token: 0x04004597 RID: 17815
		public int desync;

		// Token: 0x04004598 RID: 17816
		public bool attacker_dead;
	}
}

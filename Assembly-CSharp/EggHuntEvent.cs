using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class EggHuntEvent : BaseHuntEvent
{
	// Token: 0x04000FD6 RID: 4054
	public float warmupTime = 10f;

	// Token: 0x04000FD7 RID: 4055
	public float cooldownTime = 10f;

	// Token: 0x04000FD8 RID: 4056
	public float warnTime = 20f;

	// Token: 0x04000FD9 RID: 4057
	public float timeAlive;

	// Token: 0x04000FDA RID: 4058
	public static EggHuntEvent serverEvent = null;

	// Token: 0x04000FDB RID: 4059
	public static EggHuntEvent clientEvent = null;

	// Token: 0x04000FDC RID: 4060
	[NonSerialized]
	public static float durationSeconds = 180f;

	// Token: 0x04000FDD RID: 4061
	private Dictionary<ulong, EggHuntEvent.EggHunter> _eggHunters = new Dictionary<ulong, EggHuntEvent.EggHunter>();

	// Token: 0x04000FDE RID: 4062
	public List<CollectableEasterEgg> _spawnedEggs = new List<CollectableEasterEgg>();

	// Token: 0x04000FDF RID: 4063
	public ItemAmount[] placementAwards;

	// Token: 0x06001721 RID: 5921 RVA: 0x000B032F File Offset: 0x000AE52F
	public bool IsEventActive()
	{
		return this.timeAlive > this.warmupTime && this.timeAlive - this.warmupTime < EggHuntEvent.durationSeconds;
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x000B0358 File Offset: 0x000AE558
	public override void ServerInit()
	{
		base.ServerInit();
		if (EggHuntEvent.serverEvent && base.isServer)
		{
			EggHuntEvent.serverEvent.Kill(global::BaseNetworkable.DestroyMode.None);
			EggHuntEvent.serverEvent = null;
		}
		EggHuntEvent.serverEvent = this;
		base.Invoke(new Action(this.StartEvent), this.warmupTime);
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x000B03AE File Offset: 0x000AE5AE
	public void StartEvent()
	{
		this.SpawnEggs();
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x000B03B8 File Offset: 0x000AE5B8
	public void SpawnEggsAtPoint(int numEggs, Vector3 pos, Vector3 aimDir, float minDist = 1f, float maxDist = 2f)
	{
		for (int i = 0; i < numEggs; i++)
		{
			Vector3 vector = pos;
			if (aimDir == Vector3.zero)
			{
				aimDir = UnityEngine.Random.onUnitSphere;
			}
			else
			{
				aimDir = AimConeUtil.GetModifiedAimConeDirection(90f, aimDir, true);
			}
			vector = pos + Vector3Ex.Direction2D(pos + aimDir * 10f, pos) * UnityEngine.Random.Range(minDist, maxDist);
			vector.y = TerrainMeta.HeightMap.GetHeight(vector);
			CollectableEasterEgg collectableEasterEgg = GameManager.server.CreateEntity(this.HuntablePrefab[UnityEngine.Random.Range(0, this.HuntablePrefab.Length)].resourcePath, vector, default(Quaternion), true) as CollectableEasterEgg;
			collectableEasterEgg.Spawn();
			this._spawnedEggs.Add(collectableEasterEgg);
		}
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x000B0484 File Offset: 0x000AE684
	[ContextMenu("SpawnDebug")]
	public void SpawnEggs()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
			this.SpawnEggsAtPoint(UnityEngine.Random.Range(4, 6) + Mathf.RoundToInt(basePlayer.eggVision), basePlayer.transform.position, basePlayer.eyes.BodyForward(), 15f, 25f);
		}
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x000B0508 File Offset: 0x000AE708
	public void RandPickup()
	{
		foreach (global::BasePlayer basePlayer in global::BasePlayer.activePlayerList)
		{
		}
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x000B0554 File Offset: 0x000AE754
	public void EggCollected(global::BasePlayer player)
	{
		EggHuntEvent.EggHunter eggHunter;
		if (this._eggHunters.ContainsKey(player.userID))
		{
			eggHunter = this._eggHunters[player.userID];
		}
		else
		{
			eggHunter = new EggHuntEvent.EggHunter();
			eggHunter.displayName = player.displayName;
			eggHunter.userid = player.userID;
			this._eggHunters.Add(player.userID, eggHunter);
		}
		if (eggHunter == null)
		{
			Debug.LogWarning("Easter error");
			return;
		}
		eggHunter.numEggs++;
		this.QueueUpdate();
		int num = ((float)Mathf.RoundToInt(player.eggVision) * 0.5f < 1f) ? UnityEngine.Random.Range(0, 2) : 1;
		this.SpawnEggsAtPoint(UnityEngine.Random.Range(1 + num, 2 + num), player.transform.position, player.eyes.BodyForward(), 15f, 25f);
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x000B0631 File Offset: 0x000AE831
	public void QueueUpdate()
	{
		if (base.IsInvoking(new Action(this.DoNetworkUpdate)))
		{
			return;
		}
		base.Invoke(new Action(this.DoNetworkUpdate), 2f);
	}

	// Token: 0x06001729 RID: 5929 RVA: 0x00007D00 File Offset: 0x00005F00
	public void DoNetworkUpdate()
	{
		base.SendNetworkUpdate(global::BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600172A RID: 5930 RVA: 0x000B065F File Offset: 0x000AE85F
	public static void Sort(List<EggHuntEvent.EggHunter> hunterList)
	{
		hunterList.Sort((EggHuntEvent.EggHunter a, EggHuntEvent.EggHunter b) => b.numEggs.CompareTo(a.numEggs));
	}

	// Token: 0x0600172B RID: 5931 RVA: 0x000B0688 File Offset: 0x000AE888
	public List<EggHuntEvent.EggHunter> GetTopHunters()
	{
		List<EggHuntEvent.EggHunter> list = Facepunch.Pool.GetList<EggHuntEvent.EggHunter>();
		foreach (KeyValuePair<ulong, EggHuntEvent.EggHunter> keyValuePair in this._eggHunters)
		{
			list.Add(keyValuePair.Value);
		}
		EggHuntEvent.Sort(list);
		return list;
	}

	// Token: 0x0600172C RID: 5932 RVA: 0x000B06F0 File Offset: 0x000AE8F0
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.eggHunt = Facepunch.Pool.Get<EggHunt>();
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		info.msg.eggHunt.hunters = Facepunch.Pool.GetList<EggHunt.EggHunter>();
		for (int i = 0; i < Mathf.Min(10, topHunters.Count); i++)
		{
			EggHunt.EggHunter eggHunter = Facepunch.Pool.Get<EggHunt.EggHunter>();
			eggHunter.displayName = topHunters[i].displayName;
			eggHunter.numEggs = topHunters[i].numEggs;
			eggHunter.playerID = topHunters[i].userid;
			info.msg.eggHunt.hunters.Add(eggHunter);
		}
	}

	// Token: 0x0600172D RID: 5933 RVA: 0x000B079C File Offset: 0x000AE99C
	public void CleanupEggs()
	{
		foreach (CollectableEasterEgg collectableEasterEgg in this._spawnedEggs)
		{
			if (collectableEasterEgg != null)
			{
				collectableEasterEgg.Kill(global::BaseNetworkable.DestroyMode.None);
			}
		}
	}

	// Token: 0x0600172E RID: 5934 RVA: 0x000B07F8 File Offset: 0x000AE9F8
	public void Cooldown()
	{
		base.CancelInvoke(new Action(this.Cooldown));
		base.Kill(global::BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x0600172F RID: 5935 RVA: 0x000B0814 File Offset: 0x000AEA14
	public virtual void PrintWinnersAndAward()
	{
		List<EggHuntEvent.EggHunter> topHunters = this.GetTopHunters();
		if (topHunters.Count > 0)
		{
			EggHuntEvent.EggHunter eggHunter = topHunters[0];
			Chat.Broadcast(string.Concat(new object[]
			{
				eggHunter.displayName,
				" is the top bunny with ",
				eggHunter.numEggs,
				" eggs collected."
			}), "", "#eee", 0UL);
			for (int i = 0; i < topHunters.Count; i++)
			{
				EggHuntEvent.EggHunter eggHunter2 = topHunters[i];
				global::BasePlayer basePlayer = global::BasePlayer.FindByID(eggHunter2.userid);
				if (basePlayer)
				{
					basePlayer.ChatMessage(string.Concat(new object[]
					{
						"You placed ",
						i + 1,
						" of ",
						topHunters.Count,
						" with ",
						topHunters[i].numEggs,
						" eggs collected."
					}));
				}
				else
				{
					Debug.LogWarning("EggHuntEvent Printwinners could not find player with id :" + eggHunter2.userid);
				}
			}
			int num = 0;
			while (num < this.placementAwards.Length && num < topHunters.Count)
			{
				global::BasePlayer basePlayer2 = global::BasePlayer.FindByID(topHunters[num].userid);
				if (basePlayer2)
				{
					basePlayer2.inventory.GiveItem(ItemManager.Create(this.placementAwards[num].itemDef, (int)this.placementAwards[num].amount, 0UL), basePlayer2.inventory.containerMain, false);
					basePlayer2.ChatMessage(string.Concat(new object[]
					{
						"You received ",
						(int)this.placementAwards[num].amount,
						"x ",
						this.placementAwards[num].itemDef.displayName.english,
						" as an award!"
					}));
				}
				num++;
			}
			return;
		}
		Chat.Broadcast("Wow, no one played so no one won.", "", "#eee", 0UL);
	}

	// Token: 0x06001730 RID: 5936 RVA: 0x000B0A29 File Offset: 0x000AEC29
	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			EggHuntEvent.serverEvent = null;
			return;
		}
		EggHuntEvent.clientEvent = null;
	}

	// Token: 0x06001731 RID: 5937 RVA: 0x000B0A48 File Offset: 0x000AEC48
	public void Update()
	{
		this.timeAlive += UnityEngine.Time.deltaTime;
		if (base.isServer && !base.IsDestroyed)
		{
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds - this.warnTime)
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved1, true, false, true);
			}
			if (this.timeAlive - this.warmupTime > EggHuntEvent.durationSeconds && !base.IsInvoking(new Action(this.Cooldown)))
			{
				base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
				this.CleanupEggs();
				this.PrintWinnersAndAward();
				base.Invoke(new Action(this.Cooldown), 10f);
			}
		}
	}

	// Token: 0x06001732 RID: 5938 RVA: 0x000B0B00 File Offset: 0x000AED00
	public float GetTimeRemaining()
	{
		float num = EggHuntEvent.durationSeconds - this.timeAlive;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	// Token: 0x02000C29 RID: 3113
	public class EggHunter
	{
		// Token: 0x04004256 RID: 16982
		public ulong userid;

		// Token: 0x04004257 RID: 16983
		public string displayName;

		// Token: 0x04004258 RID: 16984
		public int numEggs;
	}
}

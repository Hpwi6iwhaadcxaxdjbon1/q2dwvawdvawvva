using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Rust;
using UnityEngine;

// Token: 0x0200060C RID: 1548
[CreateAssetMenu(menuName = "Rust/Missions/BaseMission")]
public class BaseMission : BaseScriptableObject
{
	// Token: 0x04002566 RID: 9574
	[ServerVar]
	public static bool missionsenabled = true;

	// Token: 0x04002567 RID: 9575
	public string shortname;

	// Token: 0x04002568 RID: 9576
	public Translate.Phrase missionName;

	// Token: 0x04002569 RID: 9577
	public Translate.Phrase missionDesc;

	// Token: 0x0400256A RID: 9578
	public BaseMission.MissionObjectiveEntry[] objectives;

	// Token: 0x0400256B RID: 9579
	public static List<Vector3> blockedPoints = new List<Vector3>();

	// Token: 0x0400256C RID: 9580
	public const string MISSION_COMPLETE_STAT = "missions_completed";

	// Token: 0x0400256D RID: 9581
	public GameObjectRef acceptEffect;

	// Token: 0x0400256E RID: 9582
	public GameObjectRef failedEffect;

	// Token: 0x0400256F RID: 9583
	public GameObjectRef victoryEffect;

	// Token: 0x04002570 RID: 9584
	public int repeatDelaySecondsSuccess = -1;

	// Token: 0x04002571 RID: 9585
	public int repeatDelaySecondsFailed = -1;

	// Token: 0x04002572 RID: 9586
	public float timeLimitSeconds;

	// Token: 0x04002573 RID: 9587
	public Sprite icon;

	// Token: 0x04002574 RID: 9588
	public Sprite providerIcon;

	// Token: 0x04002575 RID: 9589
	public BaseMission.MissionDependancy[] acceptDependancies;

	// Token: 0x04002576 RID: 9590
	public BaseMission.MissionDependancy[] completionDependancies;

	// Token: 0x04002577 RID: 9591
	public BaseMission.MissionEntityEntry[] missionEntities;

	// Token: 0x04002578 RID: 9592
	public BaseMission.PositionGenerator[] positionGenerators;

	// Token: 0x04002579 RID: 9593
	public ItemAmount[] baseRewards;

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x06002DC5 RID: 11717 RVA: 0x001132FE File Offset: 0x001114FE
	public uint id
	{
		get
		{
			return this.shortname.ManifestHash();
		}
	}

	// Token: 0x06002DC6 RID: 11718 RVA: 0x0011330C File Offset: 0x0011150C
	public static void PlayerDisconnected(BasePlayer player)
	{
		if (player.IsNpc)
		{
			return;
		}
		int activeMission = player.GetActiveMission();
		if (activeMission != -1 && activeMission < player.missions.Count)
		{
			BaseMission.MissionInstance missionInstance = player.missions[activeMission];
			BaseMission mission = missionInstance.GetMission();
			if (mission.missionEntities.Length != 0)
			{
				mission.MissionFailed(missionInstance, player, BaseMission.MissionFailReason.Disconnect);
			}
		}
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x000063A5 File Offset: 0x000045A5
	public static void PlayerKilled(BasePlayer player)
	{
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x06002DC8 RID: 11720 RVA: 0x00113361 File Offset: 0x00111561
	public bool isRepeatable
	{
		get
		{
			return this.repeatDelaySecondsSuccess != -1 || this.repeatDelaySecondsFailed != -1;
		}
	}

	// Token: 0x06002DC9 RID: 11721 RVA: 0x0011337A File Offset: 0x0011157A
	public virtual Sprite GetIcon(BaseMission.MissionInstance instance)
	{
		return this.icon;
	}

	// Token: 0x06002DCA RID: 11722 RVA: 0x00113384 File Offset: 0x00111584
	public virtual void SetupPositions(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		foreach (BaseMission.PositionGenerator positionGenerator in this.positionGenerators)
		{
			if (!positionGenerator.IsDependant())
			{
				instance.missionPoints.Add(positionGenerator.GetIdentifier(), positionGenerator.GetPosition(assignee));
			}
		}
		foreach (BaseMission.PositionGenerator positionGenerator2 in this.positionGenerators)
		{
			if (positionGenerator2.IsDependant())
			{
				instance.missionPoints.Add(positionGenerator2.GetIdentifier(), positionGenerator2.GetPosition(assignee));
			}
		}
	}

	// Token: 0x06002DCB RID: 11723 RVA: 0x00113404 File Offset: 0x00111604
	public void AddBlockers(BaseMission.MissionInstance instance)
	{
		foreach (KeyValuePair<string, Vector3> keyValuePair in instance.missionPoints)
		{
			if (!BaseMission.blockedPoints.Contains(keyValuePair.Value))
			{
				BaseMission.blockedPoints.Add(keyValuePair.Value);
			}
		}
	}

	// Token: 0x06002DCC RID: 11724 RVA: 0x00113474 File Offset: 0x00111674
	public void RemoveBlockers(BaseMission.MissionInstance instance)
	{
		foreach (KeyValuePair<string, Vector3> keyValuePair in instance.missionPoints)
		{
			if (BaseMission.blockedPoints.Contains(keyValuePair.Value))
			{
				BaseMission.blockedPoints.Remove(keyValuePair.Value);
			}
		}
	}

	// Token: 0x06002DCD RID: 11725 RVA: 0x001134E8 File Offset: 0x001116E8
	public virtual void SetupRewards(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		if (this.baseRewards.Length == 0)
		{
			return;
		}
		instance.rewards = new ItemAmount[this.baseRewards.Length];
		for (int i = 0; i < this.baseRewards.Length; i++)
		{
			instance.rewards[i] = new ItemAmount(this.baseRewards[i].itemDef, this.baseRewards[i].amount);
		}
	}

	// Token: 0x06002DCE RID: 11726 RVA: 0x0011354C File Offset: 0x0011174C
	public static void DoMissionEffect(string effectString, BasePlayer assignee)
	{
		Effect effect = new Effect();
		effect.Init(Effect.Type.Generic, assignee, StringPool.Get("head"), Vector3.zero, Vector3.forward, null);
		effect.pooledString = effectString;
		EffectNetwork.Send(effect, assignee.net.connection);
	}

	// Token: 0x06002DCF RID: 11727 RVA: 0x00113588 File Offset: 0x00111788
	public virtual void MissionStart(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		this.SetupRewards(instance, assignee);
		this.SetupPositions(instance, assignee);
		this.AddBlockers(instance);
		for (int i = 0; i < this.objectives.Length; i++)
		{
			this.objectives[i].Get().MissionStarted(i, instance);
		}
		if (this.acceptEffect.isValid)
		{
			BaseMission.DoMissionEffect(this.acceptEffect.resourcePath, assignee);
		}
		foreach (BaseMission.MissionEntityEntry missionEntityEntry in this.missionEntities)
		{
			if (missionEntityEntry.entityRef.isValid)
			{
				Vector3 missionPoint = instance.GetMissionPoint(missionEntityEntry.spawnPositionToUse, assignee);
				BaseEntity baseEntity = GameManager.server.CreateEntity(missionEntityEntry.entityRef.resourcePath, missionPoint, Quaternion.identity, true);
				MissionEntity missionEntity = baseEntity.gameObject.AddComponent<MissionEntity>();
				missionEntity.Setup(assignee, instance, missionEntityEntry.cleanupOnMissionSuccess, missionEntityEntry.cleanupOnMissionFailed);
				instance.createdEntities.Add(missionEntity);
				baseEntity.Spawn();
			}
		}
		foreach (MissionEntity missionEntity2 in instance.createdEntities)
		{
			missionEntity2.MissionStarted(assignee, instance);
		}
	}

	// Token: 0x06002DD0 RID: 11728 RVA: 0x001136C0 File Offset: 0x001118C0
	public void CheckObjectives(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		bool flag = true;
		for (int i = 0; i < this.objectives.Length; i++)
		{
			if (!instance.objectiveStatuses[i].completed || instance.objectiveStatuses[i].failed)
			{
				flag = false;
			}
		}
		if (flag && instance.status == BaseMission.MissionStatus.Active)
		{
			this.MissionSuccess(instance, assignee);
		}
	}

	// Token: 0x06002DD1 RID: 11729 RVA: 0x00113718 File Offset: 0x00111918
	public virtual void Think(BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		for (int i = 0; i < this.objectives.Length; i++)
		{
			this.objectives[i].Get().Think(i, instance, assignee, delta);
		}
		this.CheckObjectives(instance, assignee);
	}

	// Token: 0x06002DD2 RID: 11730 RVA: 0x00113758 File Offset: 0x00111958
	public virtual void MissionComplete(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		BaseMission.DoMissionEffect(this.victoryEffect.resourcePath, assignee);
		assignee.ChatMessage("You have completed the mission : " + this.missionName.english);
		if (instance.rewards != null && instance.rewards.Length != 0)
		{
			foreach (ItemAmount itemAmount in instance.rewards)
			{
				if (itemAmount.itemDef == null || itemAmount.amount == 0f)
				{
					Debug.LogError("BIG REWARD SCREWUP, NULL ITEM DEF");
				}
				Item item = ItemManager.Create(itemAmount.itemDef, Mathf.CeilToInt(itemAmount.amount), 0UL);
				if (item != null)
				{
					assignee.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
				}
			}
		}
		Analytics.Server.MissionComplete(this);
		Analytics.Azure.OnMissionComplete(assignee, this, null);
		instance.status = BaseMission.MissionStatus.Completed;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty(true);
		if (GameInfo.HasAchievements)
		{
			assignee.stats.Add("missions_completed", 1, Stats.All);
			assignee.stats.Save(true);
		}
	}

	// Token: 0x06002DD3 RID: 11731 RVA: 0x00113855 File Offset: 0x00111A55
	public virtual void MissionSuccess(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		instance.status = BaseMission.MissionStatus.Accomplished;
		this.MissionEnded(instance, assignee);
		this.MissionComplete(instance, assignee);
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x00113870 File Offset: 0x00111A70
	public virtual void MissionFailed(BaseMission.MissionInstance instance, BasePlayer assignee, BaseMission.MissionFailReason failReason)
	{
		assignee.ChatMessage("You have failed the mission : " + this.missionName.english);
		BaseMission.DoMissionEffect(this.failedEffect.resourcePath, assignee);
		Analytics.Server.MissionFailed(this, failReason);
		Analytics.Azure.OnMissionComplete(assignee, this, new BaseMission.MissionFailReason?(failReason));
		instance.status = BaseMission.MissionStatus.Failed;
		this.MissionEnded(instance, assignee);
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x001138CC File Offset: 0x00111ACC
	public virtual void MissionEnded(BaseMission.MissionInstance instance, BasePlayer assignee)
	{
		if (instance.createdEntities != null)
		{
			for (int i = instance.createdEntities.Count - 1; i >= 0; i--)
			{
				MissionEntity missionEntity = instance.createdEntities[i];
				if (!(missionEntity == null))
				{
					missionEntity.MissionEnded(assignee, instance);
				}
			}
		}
		this.RemoveBlockers(instance);
		instance.endTime = Time.time;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty(true);
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x00113938 File Offset: 0x00111B38
	public void OnObjectiveCompleted(int objectiveIndex, BaseMission.MissionInstance instance, BasePlayer playerFor)
	{
		BaseMission.MissionObjectiveEntry missionObjectiveEntry = this.objectives[objectiveIndex];
		if (missionObjectiveEntry.autoCompleteOtherObjectives.Length != 0)
		{
			foreach (int num in missionObjectiveEntry.autoCompleteOtherObjectives)
			{
				BaseMission.MissionObjectiveEntry missionObjectiveEntry2 = this.objectives[num];
				if (!instance.objectiveStatuses[num].completed)
				{
					missionObjectiveEntry2.objective.CompleteObjective(num, instance, playerFor);
				}
			}
		}
		this.CheckObjectives(instance, playerFor);
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x001139A0 File Offset: 0x00111BA0
	public static bool AssignMission(BasePlayer assignee, IMissionProvider provider, BaseMission mission)
	{
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		if (!mission.IsEligableForMission(assignee, provider))
		{
			return false;
		}
		BaseMission.MissionInstance missionInstance = Pool.Get<BaseMission.MissionInstance>();
		missionInstance.missionID = mission.id;
		missionInstance.startTime = Time.time;
		missionInstance.providerID = provider.ProviderID();
		missionInstance.status = BaseMission.MissionStatus.Active;
		missionInstance.createdEntities = Pool.GetList<MissionEntity>();
		missionInstance.objectiveStatuses = new BaseMission.MissionInstance.ObjectiveStatus[mission.objectives.Length];
		for (int i = 0; i < mission.objectives.Length; i++)
		{
			missionInstance.objectiveStatuses[i] = new BaseMission.MissionInstance.ObjectiveStatus();
		}
		assignee.AddMission(missionInstance);
		mission.MissionStart(missionInstance, assignee);
		assignee.SetActiveMission(assignee.missions.Count - 1);
		assignee.MissionDirty(true);
		return true;
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x00113A5C File Offset: 0x00111C5C
	public bool IsEligableForMission(BasePlayer player, IMissionProvider provider)
	{
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		foreach (BaseMission.MissionInstance missionInstance in player.missions)
		{
			if (missionInstance.status == BaseMission.MissionStatus.Accomplished || missionInstance.status == BaseMission.MissionStatus.Active)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x02000D89 RID: 3465
	[Serializable]
	public class MissionDependancy
	{
		// Token: 0x040047D0 RID: 18384
		public string targetMissionShortname;

		// Token: 0x040047D1 RID: 18385
		public BaseMission.MissionStatus targetMissionDesiredStatus;

		// Token: 0x040047D2 RID: 18386
		public bool everAttempted;

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x060050F9 RID: 20729 RVA: 0x001AA882 File Offset: 0x001A8A82
		public uint targetMissionID
		{
			get
			{
				return this.targetMissionShortname.ManifestHash();
			}
		}
	}

	// Token: 0x02000D8A RID: 3466
	public enum MissionStatus
	{
		// Token: 0x040047D4 RID: 18388
		Default,
		// Token: 0x040047D5 RID: 18389
		Active,
		// Token: 0x040047D6 RID: 18390
		Accomplished,
		// Token: 0x040047D7 RID: 18391
		Failed,
		// Token: 0x040047D8 RID: 18392
		Completed
	}

	// Token: 0x02000D8B RID: 3467
	public enum MissionEventType
	{
		// Token: 0x040047DA RID: 18394
		CUSTOM,
		// Token: 0x040047DB RID: 18395
		HARVEST,
		// Token: 0x040047DC RID: 18396
		CONVERSATION,
		// Token: 0x040047DD RID: 18397
		KILL_ENTITY,
		// Token: 0x040047DE RID: 18398
		ACQUIRE_ITEM,
		// Token: 0x040047DF RID: 18399
		FREE_CRATE
	}

	// Token: 0x02000D8C RID: 3468
	[Serializable]
	public class MissionObjectiveEntry
	{
		// Token: 0x040047E0 RID: 18400
		public Translate.Phrase description;

		// Token: 0x040047E1 RID: 18401
		public int[] startAfterCompletedObjectives;

		// Token: 0x040047E2 RID: 18402
		public int[] autoCompleteOtherObjectives;

		// Token: 0x040047E3 RID: 18403
		public bool onlyProgressIfStarted = true;

		// Token: 0x040047E4 RID: 18404
		public MissionObjective objective;

		// Token: 0x060050FB RID: 20731 RVA: 0x001AA88F File Offset: 0x001A8A8F
		public MissionObjective Get()
		{
			return this.objective;
		}
	}

	// Token: 0x02000D8D RID: 3469
	public class MissionInstance : Pool.IPooled
	{
		// Token: 0x040047E5 RID: 18405
		private BaseEntity _cachedProviderEntity;

		// Token: 0x040047E6 RID: 18406
		private BaseMission _cachedMission;

		// Token: 0x040047E7 RID: 18407
		public NetworkableId providerID;

		// Token: 0x040047E8 RID: 18408
		public uint missionID;

		// Token: 0x040047E9 RID: 18409
		public BaseMission.MissionStatus status;

		// Token: 0x040047EA RID: 18410
		public float completionScale;

		// Token: 0x040047EB RID: 18411
		public float startTime;

		// Token: 0x040047EC RID: 18412
		public float endTime;

		// Token: 0x040047ED RID: 18413
		public Vector3 missionLocation;

		// Token: 0x040047EE RID: 18414
		public float timePassed;

		// Token: 0x040047EF RID: 18415
		public Dictionary<string, Vector3> missionPoints = new Dictionary<string, Vector3>();

		// Token: 0x040047F0 RID: 18416
		public BaseMission.MissionInstance.ObjectiveStatus[] objectiveStatuses;

		// Token: 0x040047F1 RID: 18417
		public List<MissionEntity> createdEntities;

		// Token: 0x040047F2 RID: 18418
		public ItemAmount[] rewards;

		// Token: 0x060050FD RID: 20733 RVA: 0x001AA8A6 File Offset: 0x001A8AA6
		public BaseEntity ProviderEntity()
		{
			if (this._cachedProviderEntity == null)
			{
				this._cachedProviderEntity = (BaseNetworkable.serverEntities.Find(this.providerID) as BaseEntity);
			}
			return this._cachedProviderEntity;
		}

		// Token: 0x060050FE RID: 20734 RVA: 0x001AA8D7 File Offset: 0x001A8AD7
		public BaseMission GetMission()
		{
			if (this._cachedMission == null)
			{
				this._cachedMission = MissionManifest.GetFromID(this.missionID);
			}
			return this._cachedMission;
		}

		// Token: 0x060050FF RID: 20735 RVA: 0x001AA8FE File Offset: 0x001A8AFE
		public bool ShouldShowOnMap()
		{
			return (this.status == BaseMission.MissionStatus.Active || this.status == BaseMission.MissionStatus.Accomplished) && this.missionLocation != Vector3.zero;
		}

		// Token: 0x06005100 RID: 20736 RVA: 0x001AA924 File Offset: 0x001A8B24
		public bool ShouldShowOnCompass()
		{
			return this.ShouldShowOnMap();
		}

		// Token: 0x06005101 RID: 20737 RVA: 0x001AA92C File Offset: 0x001A8B2C
		public virtual void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionEventType type, string identifier, float amount)
		{
			if (this.status != BaseMission.MissionStatus.Active)
			{
				return;
			}
			BaseMission mission = this.GetMission();
			for (int i = 0; i < mission.objectives.Length; i++)
			{
				mission.objectives[i].objective.ProcessMissionEvent(playerFor, this, i, type, identifier, amount);
			}
		}

		// Token: 0x06005102 RID: 20738 RVA: 0x001AA978 File Offset: 0x001A8B78
		public void Think(BasePlayer assignee, float delta)
		{
			if (this.status == BaseMission.MissionStatus.Failed || this.status == BaseMission.MissionStatus.Completed)
			{
				return;
			}
			BaseMission mission = this.GetMission();
			this.timePassed += delta;
			mission.Think(this, assignee, delta);
			if (mission.timeLimitSeconds > 0f && this.timePassed >= mission.timeLimitSeconds)
			{
				mission.MissionFailed(this, assignee, BaseMission.MissionFailReason.TimeOut);
			}
		}

		// Token: 0x06005103 RID: 20739 RVA: 0x001AA9DC File Offset: 0x001A8BDC
		public Vector3 GetMissionPoint(string identifier, BasePlayer playerFor)
		{
			if (this.missionPoints.ContainsKey(identifier))
			{
				return this.missionPoints[identifier];
			}
			if (!playerFor)
			{
				Debug.Log("Massive mission failure to get point, correct mission definition of : " + this.GetMission().shortname);
				return Vector3.zero;
			}
			this.GetMission().SetupPositions(this, playerFor);
			Debug.Log("Mission point not found, regenerating");
			if (this.missionPoints.ContainsKey(identifier))
			{
				return this.missionPoints[identifier];
			}
			return Vector3.zero;
		}

		// Token: 0x06005104 RID: 20740 RVA: 0x001AAA64 File Offset: 0x001A8C64
		public void EnterPool()
		{
			this.providerID = default(NetworkableId);
			this.missionID = 0U;
			this.status = BaseMission.MissionStatus.Default;
			this.completionScale = 0f;
			this.startTime = -1f;
			this.endTime = -1f;
			this.missionLocation = Vector3.zero;
			this._cachedMission = null;
			this.timePassed = 0f;
			this.rewards = null;
			this.missionPoints.Clear();
			if (this.createdEntities != null)
			{
				Pool.FreeList<MissionEntity>(ref this.createdEntities);
			}
		}

		// Token: 0x06005105 RID: 20741 RVA: 0x001AAAEE File Offset: 0x001A8CEE
		public void LeavePool()
		{
			this.createdEntities = Pool.GetList<MissionEntity>();
		}

		// Token: 0x02000FCE RID: 4046
		[Serializable]
		public class ObjectiveStatus
		{
			// Token: 0x040050E7 RID: 20711
			public bool started;

			// Token: 0x040050E8 RID: 20712
			public bool completed;

			// Token: 0x040050E9 RID: 20713
			public bool failed;

			// Token: 0x040050EA RID: 20714
			public int genericInt1;

			// Token: 0x040050EB RID: 20715
			public float genericFloat1;
		}

		// Token: 0x02000FCF RID: 4047
		public enum ObjectiveType
		{
			// Token: 0x040050ED RID: 20717
			MOVE,
			// Token: 0x040050EE RID: 20718
			KILL
		}
	}

	// Token: 0x02000D8E RID: 3470
	[Serializable]
	public class PositionGenerator
	{
		// Token: 0x040047F3 RID: 18419
		public string identifier;

		// Token: 0x040047F4 RID: 18420
		public float minDistForMovePoint;

		// Token: 0x040047F5 RID: 18421
		public float maxDistForMovePoint = 25f;

		// Token: 0x040047F6 RID: 18422
		public bool centerOnProvider;

		// Token: 0x040047F7 RID: 18423
		public bool centerOnPlayer;

		// Token: 0x040047F8 RID: 18424
		public string centerOnPositionIdentifier = "";

		// Token: 0x040047F9 RID: 18425
		public BaseMission.PositionGenerator.PositionType positionType;

		// Token: 0x040047FA RID: 18426
		[Header("MissionPoint")]
		[global::InspectorFlags]
		public MissionPoint.MissionPointEnum Flags = (MissionPoint.MissionPointEnum)(-1);

		// Token: 0x040047FB RID: 18427
		[global::InspectorFlags]
		public MissionPoint.MissionPointEnum ExclusionFlags;

		// Token: 0x040047FC RID: 18428
		[Header("WorldPositionGenerator")]
		public WorldPositionGenerator worldPositionGenerator;

		// Token: 0x06005107 RID: 20743 RVA: 0x001AAB0E File Offset: 0x001A8D0E
		public bool IsDependant()
		{
			return !string.IsNullOrEmpty(this.centerOnPositionIdentifier);
		}

		// Token: 0x06005108 RID: 20744 RVA: 0x001AAB1E File Offset: 0x001A8D1E
		public string GetIdentifier()
		{
			return this.identifier;
		}

		// Token: 0x06005109 RID: 20745 RVA: 0x001AAB28 File Offset: 0x001A8D28
		public bool Validate(BasePlayer assignee, BaseMission missionDef)
		{
			Vector3 vector;
			if (this.positionType == BaseMission.PositionGenerator.PositionType.MissionPoint)
			{
				List<MissionPoint> list = Pool.GetList<MissionPoint>();
				bool missionPoints = MissionPoint.GetMissionPoints(ref list, assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, (int)this.Flags, (int)this.ExclusionFlags);
				Pool.FreeList<MissionPoint>(ref list);
				if (!missionPoints)
				{
					Debug.Log("FAILED TO FIND MISSION POINTS");
					return false;
				}
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.WorldPositionGenerator && this.worldPositionGenerator != null && !this.worldPositionGenerator.TrySample(assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, out vector, BaseMission.blockedPoints))
			{
				Debug.Log("FAILED TO GENERATE WORLD POSITION!!!!!");
				return false;
			}
			return true;
		}

		// Token: 0x0600510A RID: 20746 RVA: 0x001AABD4 File Offset: 0x001A8DD4
		public Vector3 GetPosition(BasePlayer assignee)
		{
			Vector3 vector;
			if (this.positionType == BaseMission.PositionGenerator.PositionType.MissionPoint)
			{
				List<MissionPoint> list = Pool.GetList<MissionPoint>();
				if (MissionPoint.GetMissionPoints(ref list, assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, (int)this.Flags, (int)this.ExclusionFlags))
				{
					vector = list[UnityEngine.Random.Range(0, list.Count)].GetPosition();
				}
				else
				{
					Debug.LogError("UNABLE TO FIND MISSIONPOINT FOR MISSION!");
					vector = assignee.transform.position;
				}
				Pool.FreeList<MissionPoint>(ref list);
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.WorldPositionGenerator && this.worldPositionGenerator != null)
			{
				if (!this.worldPositionGenerator.TrySample(assignee.transform.position, this.minDistForMovePoint, this.maxDistForMovePoint, out vector, BaseMission.blockedPoints))
				{
					Debug.LogError("UNABLE TO FIND WORLD POINT FOR MISSION!");
					vector = assignee.transform.position;
				}
			}
			else if (this.positionType == BaseMission.PositionGenerator.PositionType.DungeonPoint)
			{
				vector = DynamicDungeon.GetNextDungeonPoint();
			}
			else
			{
				Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
				onUnitSphere.y = 0f;
				onUnitSphere.Normalize();
				vector = (this.centerOnPlayer ? assignee.transform.position : assignee.transform.position) + onUnitSphere * UnityEngine.Random.Range(this.minDistForMovePoint, this.maxDistForMovePoint);
				float b = vector.y;
				float a = vector.y;
				if (TerrainMeta.WaterMap != null)
				{
					a = TerrainMeta.WaterMap.GetHeight(vector);
				}
				if (TerrainMeta.HeightMap != null)
				{
					b = TerrainMeta.HeightMap.GetHeight(vector);
				}
				vector.y = Mathf.Max(a, b);
			}
			return vector;
		}

		// Token: 0x02000FD0 RID: 4048
		public enum PositionType
		{
			// Token: 0x040050F0 RID: 20720
			MissionPoint,
			// Token: 0x040050F1 RID: 20721
			WorldPositionGenerator,
			// Token: 0x040050F2 RID: 20722
			DungeonPoint
		}
	}

	// Token: 0x02000D8F RID: 3471
	[Serializable]
	public class MissionEntityEntry
	{
		// Token: 0x040047FD RID: 18429
		public GameObjectRef entityRef;

		// Token: 0x040047FE RID: 18430
		public string spawnPositionToUse;

		// Token: 0x040047FF RID: 18431
		public bool cleanupOnMissionFailed;

		// Token: 0x04004800 RID: 18432
		public bool cleanupOnMissionSuccess;

		// Token: 0x04004801 RID: 18433
		public string entityIdentifier;
	}

	// Token: 0x02000D90 RID: 3472
	public enum MissionFailReason
	{
		// Token: 0x04004803 RID: 18435
		TimeOut,
		// Token: 0x04004804 RID: 18436
		Disconnect,
		// Token: 0x04004805 RID: 18437
		ResetPlayerState,
		// Token: 0x04004806 RID: 18438
		Abandon
	}
}

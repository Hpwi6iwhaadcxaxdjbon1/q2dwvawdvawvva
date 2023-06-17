using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020004DC RID: 1244
public class PuzzleReset : FacepunchBehaviour
{
	// Token: 0x0400207B RID: 8315
	public SpawnGroup[] respawnGroups;

	// Token: 0x0400207C RID: 8316
	public IOEntity[] resetEnts;

	// Token: 0x0400207D RID: 8317
	public GameObject[] resetObjects;

	// Token: 0x0400207E RID: 8318
	public bool playersBlockReset;

	// Token: 0x0400207F RID: 8319
	public bool CheckSleepingAIZForPlayers;

	// Token: 0x04002080 RID: 8320
	public float playerDetectionRadius;

	// Token: 0x04002081 RID: 8321
	public float playerHeightDetectionMinMax = -1f;

	// Token: 0x04002082 RID: 8322
	public Transform playerDetectionOrigin;

	// Token: 0x04002083 RID: 8323
	public float timeBetweenResets = 30f;

	// Token: 0x04002084 RID: 8324
	public bool scaleWithServerPopulation;

	// Token: 0x04002085 RID: 8325
	[HideInInspector]
	public Vector3[] resetPositions;

	// Token: 0x04002086 RID: 8326
	public bool broadcastResetMessage;

	// Token: 0x04002087 RID: 8327
	public Translate.Phrase resetPhrase;

	// Token: 0x04002088 RID: 8328
	private AIInformationZone zone;

	// Token: 0x04002089 RID: 8329
	private float resetTimeElapsed;

	// Token: 0x0400208A RID: 8330
	private float resetTickTime = 10f;

	// Token: 0x06002848 RID: 10312 RVA: 0x000F8F62 File Offset: 0x000F7162
	public float GetResetSpacing()
	{
		return this.timeBetweenResets * (this.scaleWithServerPopulation ? (1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate)) : 1f);
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000F8F8F File Offset: 0x000F718F
	public void Start()
	{
		if (this.timeBetweenResets != float.PositiveInfinity)
		{
			this.ResetTimer();
		}
	}

	// Token: 0x0600284A RID: 10314 RVA: 0x000F8FA4 File Offset: 0x000F71A4
	public void ResetTimer()
	{
		this.resetTimeElapsed = 0f;
		base.CancelInvoke(new Action(this.ResetTick));
		base.InvokeRandomized(new Action(this.ResetTick), UnityEngine.Random.Range(0f, 1f), this.resetTickTime, 0.5f);
	}

	// Token: 0x0600284B RID: 10315 RVA: 0x000F8FFA File Offset: 0x000F71FA
	public bool PassesResetCheck()
	{
		if (!this.playersBlockReset)
		{
			return true;
		}
		if (this.CheckSleepingAIZForPlayers)
		{
			return this.AIZSleeping();
		}
		return !this.PlayersWithinDistance();
	}

	// Token: 0x0600284C RID: 10316 RVA: 0x000F9020 File Offset: 0x000F7220
	private bool AIZSleeping()
	{
		if (this.zone != null)
		{
			if (!this.zone.PointInside(base.transform.position))
			{
				this.zone = AIInformationZone.GetForPoint(base.transform.position, true);
			}
		}
		else
		{
			this.zone = AIInformationZone.GetForPoint(base.transform.position, true);
		}
		return !(this.zone == null) && this.zone.Sleeping;
	}

	// Token: 0x0600284D RID: 10317 RVA: 0x000F909E File Offset: 0x000F729E
	private bool PlayersWithinDistance()
	{
		return PuzzleReset.AnyPlayersWithinDistance(this.playerDetectionOrigin, this.playerDetectionRadius);
	}

	// Token: 0x0600284E RID: 10318 RVA: 0x000F90B4 File Offset: 0x000F72B4
	public static bool AnyPlayersWithinDistance(Transform origin, float radius)
	{
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && Vector3.Distance(basePlayer.transform.position, origin.position) < radius)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600284F RID: 10319 RVA: 0x000F9130 File Offset: 0x000F7330
	public void ResetTick()
	{
		if (this.PassesResetCheck())
		{
			this.resetTimeElapsed += this.resetTickTime;
		}
		if (this.resetTimeElapsed > this.GetResetSpacing())
		{
			this.resetTimeElapsed = 0f;
			this.DoReset();
		}
	}

	// Token: 0x06002850 RID: 10320 RVA: 0x000F916C File Offset: 0x000F736C
	public void CleanupSleepers()
	{
		if (this.playerDetectionOrigin == null || BasePlayer.sleepingPlayerList == null)
		{
			return;
		}
		for (int i = BasePlayer.sleepingPlayerList.Count - 1; i >= 0; i--)
		{
			BasePlayer basePlayer = BasePlayer.sleepingPlayerList[i];
			if (!(basePlayer == null) && basePlayer.IsSleeping() && Vector3.Distance(basePlayer.transform.position, this.playerDetectionOrigin.position) <= this.playerDetectionRadius)
			{
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
			}
		}
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000F91F4 File Offset: 0x000F73F4
	public void DoReset()
	{
		this.CleanupSleepers();
		IOEntity component = base.GetComponent<IOEntity>();
		if (component != null)
		{
			PuzzleReset.ResetIOEntRecursive(component, UnityEngine.Time.frameCount);
			component.MarkDirty();
		}
		else if (this.resetPositions != null)
		{
			foreach (Vector3 position in this.resetPositions)
			{
				Vector3 position2 = base.transform.TransformPoint(position);
				List<IOEntity> list = Facepunch.Pool.GetList<IOEntity>();
				global::Vis.Entities<IOEntity>(position2, 0.5f, list, 1235288065, QueryTriggerInteraction.Ignore);
				foreach (IOEntity ioentity in list)
				{
					if (ioentity.IsRootEntity() && ioentity.isServer)
					{
						PuzzleReset.ResetIOEntRecursive(ioentity, UnityEngine.Time.frameCount);
						ioentity.MarkDirty();
					}
				}
				Facepunch.Pool.FreeList<IOEntity>(ref list);
			}
		}
		List<SpawnGroup> list2 = Facepunch.Pool.GetList<SpawnGroup>();
		global::Vis.Components<SpawnGroup>(base.transform.position, 1f, list2, 262144, QueryTriggerInteraction.Collide);
		foreach (SpawnGroup spawnGroup in list2)
		{
			if (!(spawnGroup == null))
			{
				spawnGroup.Clear();
				spawnGroup.DelayedSpawn();
			}
		}
		Facepunch.Pool.FreeList<SpawnGroup>(ref list2);
		foreach (GameObject gameObject in this.resetObjects)
		{
			if (gameObject != null)
			{
				gameObject.SendMessage("OnPuzzleReset", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (this.broadcastResetMessage)
		{
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				if (!basePlayer.IsNpc && basePlayer.IsConnected)
				{
					basePlayer.ShowToast(GameTip.Styles.Server_Event, this.resetPhrase, Array.Empty<string>());
				}
			}
		}
	}

	// Token: 0x06002852 RID: 10322 RVA: 0x000F9404 File Offset: 0x000F7604
	public static void ResetIOEntRecursive(IOEntity target, int resetIndex)
	{
		if (target.lastResetIndex == resetIndex)
		{
			return;
		}
		target.lastResetIndex = resetIndex;
		target.ResetIOState();
		foreach (IOEntity.IOSlot ioslot in target.outputs)
		{
			if (ioslot.connectedTo.Get(true) != null && ioslot.connectedTo.Get(true) != target)
			{
				PuzzleReset.ResetIOEntRecursive(ioslot.connectedTo.Get(true), resetIndex);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000316 RID: 790
public class DevBotSpawner : FacepunchBehaviour
{
	// Token: 0x040017D5 RID: 6101
	public GameObjectRef bot;

	// Token: 0x040017D6 RID: 6102
	public Transform waypointParent;

	// Token: 0x040017D7 RID: 6103
	public bool autoSelectLatestSpawnedGameObject = true;

	// Token: 0x040017D8 RID: 6104
	public float spawnRate = 1f;

	// Token: 0x040017D9 RID: 6105
	public int maxPopulation = 1;

	// Token: 0x040017DA RID: 6106
	private Transform[] waypoints;

	// Token: 0x040017DB RID: 6107
	private List<BaseEntity> _spawned = new List<BaseEntity>();

	// Token: 0x06001EC4 RID: 7876 RVA: 0x000D1A58 File Offset: 0x000CFC58
	public bool HasFreePopulation()
	{
		for (int i = this._spawned.Count - 1; i >= 0; i--)
		{
			BaseEntity baseEntity = this._spawned[i];
			if (baseEntity == null || baseEntity.Health() <= 0f)
			{
				this._spawned.Remove(baseEntity);
			}
		}
		return this._spawned.Count < this.maxPopulation;
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000D1AC4 File Offset: 0x000CFCC4
	public void SpawnBot()
	{
		while (this.HasFreePopulation())
		{
			Vector3 position = this.waypoints[0].position;
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.bot.resourcePath, position, default(Quaternion), true);
			if (baseEntity == null)
			{
				return;
			}
			this._spawned.Add(baseEntity);
			baseEntity.SendMessage("SetWaypoints", this.waypoints, SendMessageOptions.DontRequireReceiver);
			baseEntity.Spawn();
		}
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000D1B38 File Offset: 0x000CFD38
	public void Start()
	{
		this.waypoints = this.waypointParent.GetComponentsInChildren<Transform>();
		base.InvokeRepeating(new Action(this.SpawnBot), 5f, this.spawnRate);
	}
}

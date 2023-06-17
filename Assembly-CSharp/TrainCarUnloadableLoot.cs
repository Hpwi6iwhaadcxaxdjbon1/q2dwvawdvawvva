using System;
using Rust;
using UnityEngine;

// Token: 0x020004B2 RID: 1202
public class TrainCarUnloadableLoot : TrainCarUnloadable
{
	// Token: 0x04001FC3 RID: 8131
	[SerializeField]
	private TrainCarUnloadableLoot.LootCrateSet[] lootLayouts;

	// Token: 0x04001FC4 RID: 8132
	[SerializeField]
	private Transform[] lootPositions;

	// Token: 0x06002740 RID: 10048 RVA: 0x000F544C File Offset: 0x000F364C
	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			int num = UnityEngine.Random.Range(0, this.lootLayouts.Length);
			for (int i = 0; i < this.lootLayouts[num].crates.Length; i++)
			{
				GameObjectRef gameObjectRef = this.lootLayouts[num].crates[i];
				BaseEntity baseEntity = GameManager.server.CreateEntity(gameObjectRef.resourcePath, this.lootPositions[i].localPosition, this.lootPositions[i].localRotation, true);
				if (baseEntity != null)
				{
					baseEntity.Spawn();
					baseEntity.SetParent(this, false, false);
				}
			}
		}
	}

	// Token: 0x02000D0D RID: 3341
	[Serializable]
	public class LootCrateSet
	{
		// Token: 0x04004602 RID: 17922
		public GameObjectRef[] crates;
	}
}

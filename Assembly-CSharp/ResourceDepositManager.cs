using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000403 RID: 1027
public class ResourceDepositManager : BaseEntity
{
	// Token: 0x04001B05 RID: 6917
	public static ResourceDepositManager _manager;

	// Token: 0x04001B06 RID: 6918
	private const int resolution = 20;

	// Token: 0x04001B07 RID: 6919
	public Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit> _deposits;

	// Token: 0x060022FD RID: 8957 RVA: 0x000E02B8 File Offset: 0x000DE4B8
	public static Vector2i GetIndexFrom(Vector3 pos)
	{
		return new Vector2i((int)pos.x / 20, (int)pos.z / 20);
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x000E02D3 File Offset: 0x000DE4D3
	public static ResourceDepositManager Get()
	{
		return ResourceDepositManager._manager;
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000E02DA File Offset: 0x000DE4DA
	public ResourceDepositManager()
	{
		ResourceDepositManager._manager = this;
		this._deposits = new Dictionary<Vector2i, ResourceDepositManager.ResourceDeposit>();
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000E02F4 File Offset: 0x000DE4F4
	public ResourceDepositManager.ResourceDeposit CreateFromPosition(Vector3 pos)
	{
		Vector2i indexFrom = ResourceDepositManager.GetIndexFrom(pos);
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)new Vector2((float)indexFrom.x, (float)indexFrom.y).Seed(World.Seed + World.Salt));
		ResourceDepositManager.ResourceDeposit resourceDeposit = new ResourceDepositManager.ResourceDeposit();
		resourceDeposit.origin = new Vector3((float)(indexFrom.x * 20), 0f, (float)(indexFrom.y * 20));
		if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 100, 1f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
		}
		else if (!false)
		{
			resourceDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, UnityEngine.Random.Range(30000, 100000), UnityEngine.Random.Range(0.3f, 0.5f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			float num;
			if (World.Procedural)
			{
				num = ((TerrainMeta.BiomeMap.GetBiome(pos, 2) > 0.5f) ? 1f : 0f) * 0.25f;
			}
			else
			{
				num = 0.1f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(2f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float num2;
			if (World.Procedural)
			{
				num2 = ((TerrainMeta.BiomeMap.GetBiome(pos, 1) > 0.5f) ? 1f : 0f) * (0.25f + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 8) ? 1f : 0f) + 0.25f * (TerrainMeta.TopologyMap.GetTopology(pos, 1) ? 1f : 0f));
			}
			else
			{
				num2 = 0.1f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num2)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, UnityEngine.Random.Range(10000, 100000), UnityEngine.Random.Range(4f, 4f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
			float num3 = 0f;
			if (World.Procedural)
			{
				if (TerrainMeta.BiomeMap.GetBiome(pos, 8) > 0.5f || TerrainMeta.BiomeMap.GetBiome(pos, 4) > 0.5f)
				{
					num3 += 0.25f;
				}
			}
			else
			{
				num3 += 0.15f;
			}
			if (UnityEngine.Random.Range(0f, 1f) >= 1f - num3)
			{
				resourceDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, UnityEngine.Random.Range(5000, 10000), UnityEngine.Random.Range(30f, 50f), ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, false);
			}
		}
		this._deposits.Add(indexFrom, resourceDeposit);
		UnityEngine.Random.state = state;
		return resourceDeposit;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000E05D0 File Offset: 0x000DE7D0
	public ResourceDepositManager.ResourceDeposit GetFromPosition(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit result = null;
		if (this._deposits.TryGetValue(ResourceDepositManager.GetIndexFrom(pos), out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000E05F8 File Offset: 0x000DE7F8
	public static ResourceDepositManager.ResourceDeposit GetOrCreate(Vector3 pos)
	{
		ResourceDepositManager.ResourceDeposit fromPosition = ResourceDepositManager.Get().GetFromPosition(pos);
		if (fromPosition != null)
		{
			return fromPosition;
		}
		return ResourceDepositManager.Get().CreateFromPosition(pos);
	}

	// Token: 0x02000CD5 RID: 3285
	[Serializable]
	public class ResourceDeposit
	{
		// Token: 0x04004507 RID: 17671
		public float lastSurveyTime = float.NegativeInfinity;

		// Token: 0x04004508 RID: 17672
		public Vector3 origin;

		// Token: 0x04004509 RID: 17673
		public List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry> _resources;

		// Token: 0x06004FBF RID: 20415 RVA: 0x001A7040 File Offset: 0x001A5240
		public ResourceDeposit()
		{
			this._resources = new List<ResourceDepositManager.ResourceDeposit.ResourceDepositEntry>();
		}

		// Token: 0x06004FC0 RID: 20416 RVA: 0x001A7060 File Offset: 0x001A5260
		public void Add(ItemDefinition type, float efficiency, int amount, float workNeeded, ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType, bool liquid = false)
		{
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry = new ResourceDepositManager.ResourceDeposit.ResourceDepositEntry();
			resourceDepositEntry.type = type;
			resourceDepositEntry.efficiency = efficiency;
			ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resourceDepositEntry2 = resourceDepositEntry;
			resourceDepositEntry.amount = amount;
			resourceDepositEntry2.startAmount = amount;
			resourceDepositEntry.spawnType = spawnType;
			resourceDepositEntry.workNeeded = workNeeded;
			resourceDepositEntry.isLiquid = liquid;
			this._resources.Add(resourceDepositEntry);
		}

		// Token: 0x02000FCB RID: 4043
		[Serializable]
		public enum surveySpawnType
		{
			// Token: 0x040050DA RID: 20698
			ITEM,
			// Token: 0x040050DB RID: 20699
			OIL,
			// Token: 0x040050DC RID: 20700
			WATER
		}

		// Token: 0x02000FCC RID: 4044
		[Serializable]
		public class ResourceDepositEntry
		{
			// Token: 0x040050DD RID: 20701
			public ItemDefinition type;

			// Token: 0x040050DE RID: 20702
			public float efficiency = 1f;

			// Token: 0x040050DF RID: 20703
			public int amount;

			// Token: 0x040050E0 RID: 20704
			public int startAmount;

			// Token: 0x040050E1 RID: 20705
			public float workNeeded = 1f;

			// Token: 0x040050E2 RID: 20706
			public float workDone;

			// Token: 0x040050E3 RID: 20707
			public ResourceDepositManager.ResourceDeposit.surveySpawnType spawnType;

			// Token: 0x040050E4 RID: 20708
			public bool isLiquid;

			// Token: 0x060055A0 RID: 21920 RVA: 0x001BA8AE File Offset: 0x001B8AAE
			public void Subtract(int subamount)
			{
				if (subamount <= 0)
				{
					return;
				}
				this.amount -= subamount;
				if (this.amount < 0)
				{
					this.amount = 0;
				}
			}
		}
	}
}

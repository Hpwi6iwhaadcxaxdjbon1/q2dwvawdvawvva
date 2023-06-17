using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020003F5 RID: 1013
public static class GrowableGenetics
{
	// Token: 0x04001AA2 RID: 6818
	public const int GeneSlotCount = 6;

	// Token: 0x04001AA3 RID: 6819
	public const float CrossBreedingRadius = 1.5f;

	// Token: 0x04001AA4 RID: 6820
	private static GrowableGenetics.GeneWeighting[] neighbourWeights = new GrowableGenetics.GeneWeighting[Enum.GetValues(typeof(GrowableGenetics.GeneType)).Length];

	// Token: 0x04001AA5 RID: 6821
	private static GrowableGenetics.GeneWeighting dominant = default(GrowableGenetics.GeneWeighting);

	// Token: 0x060022BB RID: 8891 RVA: 0x000DF100 File Offset: 0x000DD300
	public static void CrossBreed(GrowableEntity growable)
	{
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities<GrowableEntity>(growable.transform.position, 1.5f, list, 512, QueryTriggerInteraction.Collide);
		bool flag = false;
		for (int i = 0; i < 6; i++)
		{
			GrowableGene growableGene = growable.Genes.Genes[i];
			GrowableGenetics.GeneWeighting dominantGeneWeighting = GrowableGenetics.GetDominantGeneWeighting(growable, list, i);
			if (dominantGeneWeighting.Weighting > growable.Properties.Genes.Weights[(int)growableGene.Type].CrossBreedingWeight)
			{
				flag = true;
				growableGene.Set(dominantGeneWeighting.GeneType, false);
			}
		}
		if (flag)
		{
			growable.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000DF198 File Offset: 0x000DD398
	private static GrowableGenetics.GeneWeighting GetDominantGeneWeighting(GrowableEntity crossBreedingGrowable, List<GrowableEntity> neighbours, int slot)
	{
		PlanterBox planter = crossBreedingGrowable.GetPlanter();
		if (planter == null)
		{
			GrowableGenetics.dominant.Weighting = -1f;
			return GrowableGenetics.dominant;
		}
		for (int i = 0; i < GrowableGenetics.neighbourWeights.Length; i++)
		{
			GrowableGenetics.neighbourWeights[i].Weighting = 0f;
			GrowableGenetics.neighbourWeights[i].GeneType = (GrowableGenetics.GeneType)i;
		}
		GrowableGenetics.dominant.Weighting = 0f;
		foreach (GrowableEntity growableEntity in neighbours)
		{
			if (growableEntity.isServer)
			{
				PlanterBox planter2 = growableEntity.GetPlanter();
				if (!(planter2 == null) && !(planter2 != planter) && !(growableEntity == crossBreedingGrowable) && growableEntity.prefabID == crossBreedingGrowable.prefabID && !growableEntity.IsDead())
				{
					GrowableGenetics.GeneType type = growableEntity.Genes.Genes[slot].Type;
					float crossBreedingWeight = growableEntity.Properties.Genes.Weights[(int)type].CrossBreedingWeight;
					GrowableGenetics.GeneWeighting[] array = GrowableGenetics.neighbourWeights;
					GrowableGenetics.GeneType geneType = type;
					float num = array[(int)geneType].Weighting = array[(int)geneType].Weighting + crossBreedingWeight;
					if (num > GrowableGenetics.dominant.Weighting)
					{
						GrowableGenetics.dominant.Weighting = num;
						GrowableGenetics.dominant.GeneType = type;
					}
				}
			}
		}
		return GrowableGenetics.dominant;
	}

	// Token: 0x02000CC7 RID: 3271
	public enum GeneType
	{
		// Token: 0x040044CA RID: 17610
		Empty,
		// Token: 0x040044CB RID: 17611
		WaterRequirement,
		// Token: 0x040044CC RID: 17612
		GrowthSpeed,
		// Token: 0x040044CD RID: 17613
		Yield,
		// Token: 0x040044CE RID: 17614
		Hardiness
	}

	// Token: 0x02000CC8 RID: 3272
	public struct GeneWeighting
	{
		// Token: 0x040044CF RID: 17615
		public float Weighting;

		// Token: 0x040044D0 RID: 17616
		public GrowableGenetics.GeneType GeneType;
	}
}

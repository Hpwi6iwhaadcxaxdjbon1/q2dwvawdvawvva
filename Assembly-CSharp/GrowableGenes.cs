using System;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x020003F4 RID: 1012
public class GrowableGenes
{
	// Token: 0x04001A9F RID: 6815
	public GrowableGene[] Genes;

	// Token: 0x04001AA0 RID: 6816
	private static GrowableGenetics.GeneWeighting[] baseWeights = new GrowableGenetics.GeneWeighting[6];

	// Token: 0x04001AA1 RID: 6817
	private static GrowableGenetics.GeneWeighting[] slotWeights = new GrowableGenetics.GeneWeighting[6];

	// Token: 0x060022AD RID: 8877 RVA: 0x000DED21 File Offset: 0x000DCF21
	public GrowableGenes()
	{
		this.Clear();
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000DED30 File Offset: 0x000DCF30
	private void Clear()
	{
		this.Genes = new GrowableGene[6];
		for (int i = 0; i < 6; i++)
		{
			this.Genes[i] = new GrowableGene();
		}
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000DED64 File Offset: 0x000DCF64
	public void GenerateRandom(GrowableEntity growable)
	{
		if (growable == null)
		{
			return;
		}
		if (growable.Properties.Genes == null)
		{
			return;
		}
		this.CalculateBaseWeights(growable.Properties.Genes);
		for (int i = 0; i < 6; i++)
		{
			this.CalculateSlotWeights(growable.Properties.Genes, i);
			this.Genes[i].Set(this.PickWeightedGeneType(), true);
		}
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000DEDD4 File Offset: 0x000DCFD4
	private void CalculateBaseWeights(GrowableGeneProperties properties)
	{
		int num = 0;
		foreach (GrowableGeneProperties.GeneWeight geneWeight in properties.Weights)
		{
			GrowableGenes.baseWeights[num].GeneType = (GrowableGenes.slotWeights[num].GeneType = (GrowableGenetics.GeneType)num);
			GrowableGenes.baseWeights[num].Weighting = geneWeight.BaseWeight;
			num++;
		}
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000DEE40 File Offset: 0x000DD040
	private void CalculateSlotWeights(GrowableGeneProperties properties, int slot)
	{
		int num = 0;
		foreach (GrowableGeneProperties.GeneWeight geneWeight in properties.Weights)
		{
			GrowableGenes.slotWeights[num].Weighting = GrowableGenes.baseWeights[num].Weighting + geneWeight.SlotWeights[slot];
			num++;
		}
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000DEE9C File Offset: 0x000DD09C
	private GrowableGenetics.GeneType PickWeightedGeneType()
	{
		IOrderedEnumerable<GrowableGenetics.GeneWeighting> orderedEnumerable = from w in GrowableGenes.slotWeights
		orderby w.Weighting
		select w;
		float num = 0f;
		foreach (GrowableGenetics.GeneWeighting geneWeighting in orderedEnumerable)
		{
			num += geneWeighting.Weighting;
		}
		GrowableGenetics.GeneType result = GrowableGenetics.GeneType.Empty;
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		foreach (GrowableGenetics.GeneWeighting geneWeighting2 in orderedEnumerable)
		{
			num3 += geneWeighting2.Weighting;
			if (num2 < num3)
			{
				result = geneWeighting2.GeneType;
				break;
			}
		}
		return result;
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000DEF84 File Offset: 0x000DD184
	public int GetGeneTypeCount(GrowableGenetics.GeneType geneType)
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (genes[i].Type == geneType)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000DEFB8 File Offset: 0x000DD1B8
	public int GetPositiveGeneCount()
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (genes[i].IsPositive())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x000DEFEC File Offset: 0x000DD1EC
	public int GetNegativeGeneCount()
	{
		int num = 0;
		GrowableGene[] genes = this.Genes;
		for (int i = 0; i < genes.Length; i++)
		{
			if (!genes[i].IsPositive())
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000DF01F File Offset: 0x000DD21F
	public void Save(BaseNetworkable.SaveInfo info)
	{
		info.msg.growableEntity.genes = GrowableGeneEncoding.EncodeGenesToInt(this);
		info.msg.growableEntity.previousGenes = GrowableGeneEncoding.EncodePreviousGenesToInt(this);
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000DF04D File Offset: 0x000DD24D
	public void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.growableEntity == null)
		{
			return;
		}
		GrowableGeneEncoding.DecodeIntToGenes(info.msg.growableEntity.genes, this);
		GrowableGeneEncoding.DecodeIntToPreviousGenes(info.msg.growableEntity.previousGenes, this);
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000DF089 File Offset: 0x000DD289
	public void DebugPrint()
	{
		Debug.Log(this.GetDisplayString(false));
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000DF098 File Offset: 0x000DD298
	private string GetDisplayString(bool previousGenes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 6; i++)
		{
			stringBuilder.Append(GrowableGene.GetDisplayCharacter(previousGenes ? this.Genes[i].PreviousType : this.Genes[i].Type));
		}
		return stringBuilder.ToString();
	}
}

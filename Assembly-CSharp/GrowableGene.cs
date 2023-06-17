using System;

// Token: 0x020003F1 RID: 1009
public class GrowableGene
{
	// Token: 0x170002E5 RID: 741
	// (get) Token: 0x06002296 RID: 8854 RVA: 0x000DEA65 File Offset: 0x000DCC65
	// (set) Token: 0x06002297 RID: 8855 RVA: 0x000DEA6D File Offset: 0x000DCC6D
	public GrowableGenetics.GeneType Type { get; private set; }

	// Token: 0x170002E6 RID: 742
	// (get) Token: 0x06002298 RID: 8856 RVA: 0x000DEA76 File Offset: 0x000DCC76
	// (set) Token: 0x06002299 RID: 8857 RVA: 0x000DEA7E File Offset: 0x000DCC7E
	public GrowableGenetics.GeneType PreviousType { get; private set; }

	// Token: 0x0600229A RID: 8858 RVA: 0x000DEA87 File Offset: 0x000DCC87
	public void Set(GrowableGenetics.GeneType geneType, bool firstSet = false)
	{
		if (firstSet)
		{
			this.SetPrevious(geneType);
		}
		else
		{
			this.SetPrevious(this.Type);
		}
		this.Type = geneType;
	}

	// Token: 0x0600229B RID: 8859 RVA: 0x000DEAA8 File Offset: 0x000DCCA8
	public void SetPrevious(GrowableGenetics.GeneType type)
	{
		this.PreviousType = type;
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000DEAB1 File Offset: 0x000DCCB1
	public string GetDisplayCharacter()
	{
		return GrowableGene.GetDisplayCharacter(this.Type);
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000DEAC0 File Offset: 0x000DCCC0
	public static string GetDisplayCharacter(GrowableGenetics.GeneType type)
	{
		switch (type)
		{
		case GrowableGenetics.GeneType.Empty:
			return "X";
		case GrowableGenetics.GeneType.WaterRequirement:
			return "W";
		case GrowableGenetics.GeneType.GrowthSpeed:
			return "G";
		case GrowableGenetics.GeneType.Yield:
			return "Y";
		case GrowableGenetics.GeneType.Hardiness:
			return "H";
		default:
			return "U";
		}
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000DEB0C File Offset: 0x000DCD0C
	public string GetColourCodedDisplayCharacter()
	{
		return GrowableGene.GetColourCodedDisplayCharacter(this.Type);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000DEB19 File Offset: 0x000DCD19
	public static string GetColourCodedDisplayCharacter(GrowableGenetics.GeneType type)
	{
		return "<color=" + (GrowableGene.IsPositive(type) ? "#60891B>" : "#AA4734>") + GrowableGene.GetDisplayCharacter(type) + "</color>";
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000DEB44 File Offset: 0x000DCD44
	public static bool IsPositive(GrowableGenetics.GeneType type)
	{
		switch (type)
		{
		case GrowableGenetics.GeneType.Empty:
			return false;
		case GrowableGenetics.GeneType.WaterRequirement:
			return false;
		case GrowableGenetics.GeneType.GrowthSpeed:
			return true;
		case GrowableGenetics.GeneType.Yield:
			return true;
		case GrowableGenetics.GeneType.Hardiness:
			return true;
		default:
			return false;
		}
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000DEB6D File Offset: 0x000DCD6D
	public bool IsPositive()
	{
		return GrowableGene.IsPositive(this.Type);
	}
}

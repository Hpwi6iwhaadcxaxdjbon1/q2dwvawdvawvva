using System;
using System.Text;
using ProtoBuf;

// Token: 0x020003F2 RID: 1010
public static class GrowableGeneEncoding
{
	// Token: 0x060022A3 RID: 8867 RVA: 0x000DEB7A File Offset: 0x000DCD7A
	public static void EncodeGenesToItem(global::GrowableEntity sourceGrowable, global::Item targetItem)
	{
		if (sourceGrowable == null || sourceGrowable.Genes == null)
		{
			return;
		}
		GrowableGeneEncoding.EncodeGenesToItem(GrowableGeneEncoding.EncodeGenesToInt(sourceGrowable.Genes), targetItem);
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000DEB9F File Offset: 0x000DCD9F
	public static void EncodeGenesToItem(int genes, global::Item targetItem)
	{
		if (targetItem == null)
		{
			return;
		}
		targetItem.instanceData = new ProtoBuf.Item.InstanceData
		{
			ShouldPool = false,
			dataInt = genes
		};
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x000DEBC0 File Offset: 0x000DCDC0
	public static int EncodeGenesToInt(GrowableGenes genes)
	{
		int num = 0;
		for (int i = 0; i < genes.Genes.Length; i++)
		{
			num = GrowableGeneEncoding.Set(num, i, (int)genes.Genes[i].Type);
		}
		return num;
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x000DEBF8 File Offset: 0x000DCDF8
	public static int EncodePreviousGenesToInt(GrowableGenes genes)
	{
		int num = 0;
		for (int i = 0; i < genes.Genes.Length; i++)
		{
			num = GrowableGeneEncoding.Set(num, i, (int)genes.Genes[i].PreviousType);
		}
		return num;
	}

	// Token: 0x060022A7 RID: 8871 RVA: 0x000DEC30 File Offset: 0x000DCE30
	public static void DecodeIntToGenes(int data, GrowableGenes genes)
	{
		for (int i = 0; i < 6; i++)
		{
			genes.Genes[i].Set((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i), false);
		}
	}

	// Token: 0x060022A8 RID: 8872 RVA: 0x000DEC60 File Offset: 0x000DCE60
	public static void DecodeIntToPreviousGenes(int data, GrowableGenes genes)
	{
		for (int i = 0; i < 6; i++)
		{
			genes.Genes[i].SetPrevious((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i));
		}
	}

	// Token: 0x060022A9 RID: 8873 RVA: 0x000DEC90 File Offset: 0x000DCE90
	public static string DecodeIntToGeneString(int data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 6; i++)
		{
			stringBuilder.Append(GrowableGene.GetColourCodedDisplayCharacter((GrowableGenetics.GeneType)GrowableGeneEncoding.Get(data, i)));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060022AA RID: 8874 RVA: 0x000DECC8 File Offset: 0x000DCEC8
	private static int Set(int storage, int slot, int value)
	{
		int num = slot * 5;
		int num2 = 31 << num;
		return (storage & ~num2) | value << num;
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x000DECEC File Offset: 0x000DCEEC
	private static int Get(int storage, int slot)
	{
		int num = slot * 5;
		int num2 = 31 << num;
		return (storage & num2) >> num;
	}
}

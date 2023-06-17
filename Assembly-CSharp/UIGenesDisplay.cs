using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008CC RID: 2252
public class UIGenesDisplay : MonoBehaviour
{
	// Token: 0x04003274 RID: 12916
	public UIGene[] GeneUI;

	// Token: 0x04003275 RID: 12917
	public Text[] TextLinks;

	// Token: 0x04003276 RID: 12918
	public Text[] TextDiagLinks;

	// Token: 0x06003763 RID: 14179 RVA: 0x0014D38C File Offset: 0x0014B58C
	public void Init(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene gene in genes.Genes)
		{
			this.GeneUI[num].Init(gene);
			num++;
			if (num < genes.Genes.Length)
			{
				this.TextLinks[num - 1].color = (genes.Genes[num].IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
		}
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x0014D40F File Offset: 0x0014B60F
	public void InitDualRow(GrowableGenes genes, bool firstRow)
	{
		if (firstRow)
		{
			this.InitFirstRow(genes);
			return;
		}
		this.InitSecondRow(genes);
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x0014D424 File Offset: 0x0014B624
	private void InitFirstRow(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene growableGene in genes.Genes)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				this.GeneUI[num].InitPrevious(growableGene);
			}
			else
			{
				this.GeneUI[num].Init(growableGene);
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				return;
			}
			if (growableGene.Type != growableGene.PreviousType || genes.Genes[num].Type != genes.Genes[num].PreviousType)
			{
				this.TextLinks[num - 1].enabled = false;
			}
			else
			{
				this.TextLinks[num - 1].enabled = true;
				this.TextLinks[num - 1].color = (genes.Genes[num].IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
		}
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x0014D518 File Offset: 0x0014B718
	private void InitSecondRow(GrowableGenes genes)
	{
		int num = 0;
		foreach (GrowableGene growableGene in genes.Genes)
		{
			if (growableGene.Type != growableGene.PreviousType)
			{
				this.GeneUI[num].Init(growableGene);
			}
			else
			{
				this.GeneUI[num].Hide();
			}
			num++;
			if (num >= genes.Genes.Length)
			{
				return;
			}
			this.TextLinks[num - 1].enabled = false;
			GrowableGene growableGene2 = genes.Genes[num];
			this.TextDiagLinks[num - 1].enabled = false;
			if (growableGene.Type != growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				this.TextLinks[num - 1].enabled = true;
				this.TextLinks[num - 1].color = (growableGene2.IsPositive() ? this.GeneUI[num - 1].PositiveColour : this.GeneUI[num - 1].NegativeColour);
			}
			else if (growableGene.Type == growableGene.PreviousType && growableGene2.Type != growableGene2.PreviousType)
			{
				this.ShowDiagLink(num - 1, -43f, growableGene2);
			}
			else if (growableGene.Type != growableGene.PreviousType && growableGene2.Type == growableGene2.PreviousType)
			{
				this.ShowDiagLink(num - 1, 43f, growableGene2);
			}
		}
	}

	// Token: 0x06003767 RID: 14183 RVA: 0x0014D674 File Offset: 0x0014B874
	private void ShowDiagLink(int index, float rotation, GrowableGene nextGene)
	{
		Vector3 localEulerAngles = this.TextDiagLinks[index].transform.localEulerAngles;
		localEulerAngles.z = rotation;
		this.TextDiagLinks[index].transform.localEulerAngles = localEulerAngles;
		this.TextDiagLinks[index].enabled = true;
		this.TextDiagLinks[index].color = (nextGene.IsPositive() ? this.GeneUI[index].PositiveColour : this.GeneUI[index].NegativeColour);
	}
}

using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008CB RID: 2251
public class UIGene : MonoBehaviour
{
	// Token: 0x0400326D RID: 12909
	public GameObject Child;

	// Token: 0x0400326E RID: 12910
	public Color PositiveColour;

	// Token: 0x0400326F RID: 12911
	public Color NegativeColour;

	// Token: 0x04003270 RID: 12912
	public Color PositiveTextColour;

	// Token: 0x04003271 RID: 12913
	public Color NegativeTextColour;

	// Token: 0x04003272 RID: 12914
	public Image ImageBG;

	// Token: 0x04003273 RID: 12915
	public Text TextGene;

	// Token: 0x0600375E RID: 14174 RVA: 0x0014D2C4 File Offset: 0x0014B4C4
	public void Init(GrowableGene gene)
	{
		bool flag = gene.IsPositive();
		this.ImageBG.color = (flag ? this.PositiveColour : this.NegativeColour);
		this.TextGene.color = (flag ? this.PositiveTextColour : this.NegativeTextColour);
		this.TextGene.text = gene.GetDisplayCharacter();
		this.Show();
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x0014D327 File Offset: 0x0014B527
	public void InitPrevious(GrowableGene gene)
	{
		this.ImageBG.color = Color.black;
		this.TextGene.color = Color.grey;
		this.TextGene.text = GrowableGene.GetDisplayCharacter(gene.PreviousType);
		this.Show();
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x0014D365 File Offset: 0x0014B565
	public void Hide()
	{
		this.Child.gameObject.SetActive(false);
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x0014D378 File Offset: 0x0014B578
	public void Show()
	{
		this.Child.gameObject.SetActive(true);
	}
}

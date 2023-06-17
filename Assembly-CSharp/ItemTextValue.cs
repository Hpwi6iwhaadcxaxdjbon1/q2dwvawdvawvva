using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200082E RID: 2094
public class ItemTextValue : MonoBehaviour
{
	// Token: 0x04002EF0 RID: 12016
	public Text text;

	// Token: 0x04002EF1 RID: 12017
	public Color bad;

	// Token: 0x04002EF2 RID: 12018
	public Color good;

	// Token: 0x04002EF3 RID: 12019
	public bool negativestat;

	// Token: 0x04002EF4 RID: 12020
	public bool asPercentage;

	// Token: 0x04002EF5 RID: 12021
	public bool useColors = true;

	// Token: 0x04002EF6 RID: 12022
	public bool signed = true;

	// Token: 0x04002EF7 RID: 12023
	public string suffix;

	// Token: 0x04002EF8 RID: 12024
	public float multiplier = 1f;

	// Token: 0x060035F2 RID: 13810 RVA: 0x0014847C File Offset: 0x0014667C
	public void SetValue(float val, int numDecimals = 0, string overrideText = "")
	{
		val *= this.multiplier;
		this.text.text = ((overrideText == "") ? string.Format("{0}{1:n" + numDecimals + "}", (val > 0f && this.signed) ? "+" : "", val) : overrideText);
		if (this.asPercentage)
		{
			Text text = this.text;
			text.text += " %";
		}
		if (this.suffix != "" && !float.IsPositiveInfinity(val))
		{
			Text text2 = this.text;
			text2.text += this.suffix;
		}
		bool flag = val > 0f;
		if (this.negativestat)
		{
			flag = !flag;
		}
		if (this.useColors)
		{
			this.text.color = (flag ? this.good : this.bad);
		}
	}
}

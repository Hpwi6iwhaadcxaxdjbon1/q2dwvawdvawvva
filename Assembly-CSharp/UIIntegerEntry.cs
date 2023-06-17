using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000854 RID: 2132
public class UIIntegerEntry : MonoBehaviour
{
	// Token: 0x04002FD0 RID: 12240
	public InputField textEntry;

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06003625 RID: 13861 RVA: 0x00148F88 File Offset: 0x00147188
	// (remove) Token: 0x06003626 RID: 13862 RVA: 0x00148FC0 File Offset: 0x001471C0
	public event Action textChanged;

	// Token: 0x06003627 RID: 13863 RVA: 0x00148FF5 File Offset: 0x001471F5
	public void OnAmountTextChanged()
	{
		this.textChanged();
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x00149002 File Offset: 0x00147202
	public void SetAmount(int amount)
	{
		if (amount == this.GetIntAmount())
		{
			return;
		}
		this.textEntry.text = amount.ToString();
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x00149020 File Offset: 0x00147220
	public int GetIntAmount()
	{
		int result = 0;
		int.TryParse(this.textEntry.text, out result);
		return result;
	}

	// Token: 0x0600362A RID: 13866 RVA: 0x00149043 File Offset: 0x00147243
	public void PlusMinus(int delta)
	{
		this.SetAmount(this.GetIntAmount() + delta);
	}
}

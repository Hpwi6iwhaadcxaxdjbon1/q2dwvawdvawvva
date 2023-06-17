using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000778 RID: 1912
public class EnumListItemUI : MonoBehaviour
{
	// Token: 0x04002AFB RID: 11003
	public object Value;

	// Token: 0x04002AFC RID: 11004
	public RustText TextValue;

	// Token: 0x04002AFD RID: 11005
	private EnumListUI list;

	// Token: 0x060034CF RID: 13519 RVA: 0x00146191 File Offset: 0x00144391
	public void Init(object value, string valueText, EnumListUI list)
	{
		this.Value = value;
		this.list = list;
		this.TextValue.text = valueText;
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x001461AD File Offset: 0x001443AD
	public void Clicked()
	{
		this.list.ItemClicked(this.Value);
	}
}

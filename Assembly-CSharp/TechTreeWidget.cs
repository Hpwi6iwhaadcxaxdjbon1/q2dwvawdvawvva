using System;
using UnityEngine;

// Token: 0x020007DA RID: 2010
public class TechTreeWidget : BaseMonoBehaviour
{
	// Token: 0x04002D00 RID: 11520
	public int id;

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x06003555 RID: 13653 RVA: 0x00146787 File Offset: 0x00144987
	public RectTransform rectTransform
	{
		get
		{
			return base.GetComponent<RectTransform>();
		}
	}
}

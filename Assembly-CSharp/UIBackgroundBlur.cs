using System;
using UnityEngine;

// Token: 0x020008C4 RID: 2244
public class UIBackgroundBlur : ListComponent<UIBackgroundBlur>, IClientComponent
{
	// Token: 0x0400324C RID: 12876
	public float amount = 1f;

	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x06003751 RID: 14161 RVA: 0x0014D11C File Offset: 0x0014B31C
	public static float currentMax
	{
		get
		{
			if (ListComponent<UIBackgroundBlur>.InstanceList.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < ListComponent<UIBackgroundBlur>.InstanceList.Count; i++)
			{
				num = Mathf.Max(ListComponent<UIBackgroundBlur>.InstanceList[i].amount, num);
			}
			return num;
		}
	}
}

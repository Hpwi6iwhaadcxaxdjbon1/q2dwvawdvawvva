using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class MusicClip : ScriptableObject
{
	// Token: 0x0400142C RID: 5164
	public AudioClip audioClip;

	// Token: 0x0400142D RID: 5165
	public int lengthInBars = 1;

	// Token: 0x0400142E RID: 5166
	public int lengthInBarsWithTail;

	// Token: 0x0400142F RID: 5167
	public List<float> fadeInPoints = new List<float>();

	// Token: 0x06001BDB RID: 7131 RVA: 0x000C3688 File Offset: 0x000C1888
	public float GetNextFadeInPoint(float currentClipTimeBars)
	{
		if (this.fadeInPoints.Count == 0)
		{
			return currentClipTimeBars + 0.125f;
		}
		float result = -1f;
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.fadeInPoints.Count; i++)
		{
			float num2 = this.fadeInPoints[i];
			float num3 = num2 - currentClipTimeBars;
			if (num2 > 0.01f && num3 > 0f && num3 < num)
			{
				num = num3;
				result = num2;
			}
		}
		return result;
	}
}

using System;
using ConVar;
using UnityEngine;

// Token: 0x020007E5 RID: 2021
public class FPSGraph : Graph
{
	// Token: 0x0600356C RID: 13676 RVA: 0x00146D08 File Offset: 0x00144F08
	public void Refresh()
	{
		base.enabled = (FPS.graph > 0);
		this.Area.width = (float)(this.Resolution = Mathf.Clamp(FPS.graph, 0, Screen.width));
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x00146D48 File Offset: 0x00144F48
	protected void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x00146D50 File Offset: 0x00144F50
	protected override float GetValue()
	{
		return 1f / UnityEngine.Time.deltaTime;
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x00146D5D File Offset: 0x00144F5D
	protected override Color GetColor(float value)
	{
		if (value < 10f)
		{
			return Color.red;
		}
		if (value >= 30f)
		{
			return Color.green;
		}
		return Color.yellow;
	}
}

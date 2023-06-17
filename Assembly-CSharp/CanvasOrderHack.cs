using System;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class CanvasOrderHack : MonoBehaviour
{
	// Token: 0x06001CF6 RID: 7414 RVA: 0x000C87C8 File Offset: 0x000C69C8
	private void OnEnable()
	{
		foreach (Canvas canvas in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas.overrideSorting)
			{
				Canvas canvas2 = canvas;
				int sortingOrder = canvas2.sortingOrder;
				canvas2.sortingOrder = sortingOrder + 1;
			}
		}
		foreach (Canvas canvas3 in base.GetComponentsInChildren<Canvas>(true))
		{
			if (canvas3.overrideSorting)
			{
				Canvas canvas4 = canvas3;
				int sortingOrder = canvas4.sortingOrder;
				canvas4.sortingOrder = sortingOrder - 1;
			}
		}
	}
}

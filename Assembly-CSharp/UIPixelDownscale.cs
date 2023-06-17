using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008CE RID: 2254
public class UIPixelDownscale : MonoBehaviour
{
	// Token: 0x04003277 RID: 12919
	public CanvasScaler CanvasScaler;

	// Token: 0x0600376A RID: 14186 RVA: 0x0014D6F8 File Offset: 0x0014B8F8
	private void Awake()
	{
		if (this.CanvasScaler == null)
		{
			this.CanvasScaler = base.GetComponent<CanvasScaler>();
			if (this.CanvasScaler == null)
			{
				Debug.LogError(base.GetType().Name + " is attached to a gameobject that is missing a canvas scaler");
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x0014D754 File Offset: 0x0014B954
	private void Update()
	{
		if ((float)Screen.width < this.CanvasScaler.referenceResolution.x || (float)Screen.height < this.CanvasScaler.referenceResolution.y)
		{
			this.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
			return;
		}
		this.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
	}
}

using System;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class HolosightReticlePositioning : MonoBehaviour
{
	// Token: 0x040011D7 RID: 4567
	public IronsightAimPoint aimPoint;

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x0600190B RID: 6411 RVA: 0x000B8990 File Offset: 0x000B6B90
	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x000B899D File Offset: 0x000B6B9D
	private void Update()
	{
		if (MainCamera.isValid)
		{
			this.UpdatePosition(MainCamera.mainCamera);
		}
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x000B89B4 File Offset: 0x000B6BB4
	private void UpdatePosition(Camera cam)
	{
		Vector3 position = this.aimPoint.targetPoint.transform.position;
		Vector2 vector = RectTransformUtility.WorldToScreenPoint(cam, position);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform.parent as RectTransform, vector, cam, out vector);
		vector.x /= (this.rectTransform.parent as RectTransform).rect.width * 0.5f;
		vector.y /= (this.rectTransform.parent as RectTransform).rect.height * 0.5f;
		this.rectTransform.anchoredPosition = vector;
	}
}

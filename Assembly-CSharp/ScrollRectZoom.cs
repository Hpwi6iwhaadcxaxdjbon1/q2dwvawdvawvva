using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008B5 RID: 2229
public class ScrollRectZoom : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x040031F9 RID: 12793
	public ScrollRectEx scrollRect;

	// Token: 0x040031FA RID: 12794
	public float zoom = 1f;

	// Token: 0x040031FB RID: 12795
	public float max = 1.5f;

	// Token: 0x040031FC RID: 12796
	public float min = 0.5f;

	// Token: 0x040031FD RID: 12797
	public bool mouseWheelZoom = true;

	// Token: 0x040031FE RID: 12798
	public float scrollAmount = 0.2f;

	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x06003721 RID: 14113 RVA: 0x0014C893 File Offset: 0x0014AA93
	public RectTransform rectTransform
	{
		get
		{
			return this.scrollRect.transform as RectTransform;
		}
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x0014C8A5 File Offset: 0x0014AAA5
	private void OnEnable()
	{
		this.SetZoom(this.zoom, true);
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x0014C8B4 File Offset: 0x0014AAB4
	public void OnScroll(PointerEventData data)
	{
		if (this.mouseWheelZoom)
		{
			this.SetZoom(this.zoom + this.scrollAmount * data.scrollDelta.y, true);
		}
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x0014C8E0 File Offset: 0x0014AAE0
	public void SetZoom(float z, bool expZoom = true)
	{
		z = Mathf.Clamp(z, this.min, this.max);
		this.zoom = z;
		Vector2 normalizedPosition = this.scrollRect.normalizedPosition;
		if (expZoom)
		{
			this.scrollRect.content.localScale = Vector3.one * Mathf.Exp(this.zoom);
		}
		else
		{
			this.scrollRect.content.localScale = Vector3.one * this.zoom;
		}
		this.scrollRect.normalizedPosition = normalizedPosition;
	}
}

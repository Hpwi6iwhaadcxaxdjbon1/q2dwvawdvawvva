using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020007DB RID: 2011
public class ZoomImage : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	// Token: 0x04002D01 RID: 11521
	[SerializeField]
	private float _minimumScale = 0.5f;

	// Token: 0x04002D02 RID: 11522
	[SerializeField]
	private float _initialScale = 1f;

	// Token: 0x04002D03 RID: 11523
	[SerializeField]
	private float _maximumScale = 3f;

	// Token: 0x04002D04 RID: 11524
	[SerializeField]
	private float _scaleIncrement = 0.5f;

	// Token: 0x04002D05 RID: 11525
	[HideInInspector]
	private Vector3 _scale;

	// Token: 0x04002D06 RID: 11526
	private RectTransform _thisTransform;

	// Token: 0x06003557 RID: 13655 RVA: 0x0014678F File Offset: 0x0014498F
	private void Awake()
	{
		this._thisTransform = (base.transform as RectTransform);
		this._scale.Set(this._initialScale, this._initialScale, 1f);
		this._thisTransform.localScale = this._scale;
	}

	// Token: 0x06003558 RID: 13656 RVA: 0x001467D0 File Offset: 0x001449D0
	public void OnScroll(PointerEventData eventData)
	{
		Vector2 a;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._thisTransform, Input.mousePosition, null, out a);
		float y = eventData.scrollDelta.y;
		if (y > 0f && this._scale.x < this._maximumScale)
		{
			this._scale.Set(this._scale.x + this._scaleIncrement, this._scale.y + this._scaleIncrement, 1f);
			this._thisTransform.localScale = this._scale;
			this._thisTransform.anchoredPosition -= a * this._scaleIncrement;
			return;
		}
		if (y < 0f && this._scale.x > this._minimumScale)
		{
			this._scale.Set(this._scale.x - this._scaleIncrement, this._scale.y - this._scaleIncrement, 1f);
			this._thisTransform.localScale = this._scale;
			this._thisTransform.anchoredPosition += a * this._scaleIncrement;
		}
	}
}

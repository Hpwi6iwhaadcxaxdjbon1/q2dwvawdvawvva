using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020002E9 RID: 745
public class TextureColorPicker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
	// Token: 0x0400175A RID: 5978
	public Texture2D texture;

	// Token: 0x0400175B RID: 5979
	public TextureColorPicker.onColorSelectedEvent onColorSelected = new TextureColorPicker.onColorSelectedEvent();

	// Token: 0x06001DFC RID: 7676 RVA: 0x000CCA98 File Offset: 0x000CAC98
	public virtual void OnPointerDown(PointerEventData eventData)
	{
		this.OnDrag(eventData);
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x000CCAA4 File Offset: 0x000CACA4
	public virtual void OnDrag(PointerEventData eventData)
	{
		RectTransform rectTransform = base.transform as RectTransform;
		Vector2 vector = default(Vector2);
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out vector))
		{
			vector.x += rectTransform.rect.width * 0.5f;
			vector.y += rectTransform.rect.height * 0.5f;
			vector.x /= rectTransform.rect.width;
			vector.y /= rectTransform.rect.height;
			Color pixel = this.texture.GetPixel((int)(vector.x * (float)this.texture.width), (int)(vector.y * (float)this.texture.height));
			this.onColorSelected.Invoke(pixel);
		}
	}

	// Token: 0x02000C9A RID: 3226
	[Serializable]
	public class onColorSelectedEvent : UnityEvent<Color>
	{
	}
}

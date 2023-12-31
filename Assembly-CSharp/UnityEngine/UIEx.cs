﻿using System;
using UnityEngine.UI;

namespace UnityEngine
{
	// Token: 0x02000A2A RID: 2602
	public static class UIEx
	{
		// Token: 0x06003DC3 RID: 15811 RVA: 0x0016A438 File Offset: 0x00168638
		public static Vector2 Unpivot(this RectTransform rect, Vector2 localPos)
		{
			localPos.x += rect.pivot.x * rect.rect.width;
			localPos.y += rect.pivot.y * rect.rect.height;
			return localPos;
		}

		// Token: 0x06003DC4 RID: 15812 RVA: 0x0016A490 File Offset: 0x00168690
		public static void CenterOnPosition(this ScrollRect scrollrect, Vector2 pos)
		{
			RectTransform rectTransform = scrollrect.transform as RectTransform;
			Vector2 vector = new Vector2(scrollrect.content.localScale.x, scrollrect.content.localScale.y);
			pos.x *= vector.x;
			pos.y *= vector.y;
			Vector2 vector2 = new Vector2(scrollrect.content.rect.width * vector.x - rectTransform.rect.width, scrollrect.content.rect.height * vector.y - rectTransform.rect.height);
			pos.x = pos.x / vector2.x + scrollrect.content.pivot.x;
			pos.y = pos.y / vector2.y + scrollrect.content.pivot.y;
			if (scrollrect.movementType != ScrollRect.MovementType.Unrestricted)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			scrollrect.normalizedPosition = pos;
		}

		// Token: 0x06003DC5 RID: 15813 RVA: 0x0016A5DC File Offset: 0x001687DC
		public static void RebuildHackUnity2019(this Image image)
		{
			Sprite sprite = image.sprite;
			image.sprite = null;
			image.sprite = sprite;
		}
	}
}

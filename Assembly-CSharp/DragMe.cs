using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020008DC RID: 2268
public class DragMe : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x0400328C RID: 12940
	public static DragMe dragging;

	// Token: 0x0400328D RID: 12941
	public static GameObject dragIcon;

	// Token: 0x0400328E RID: 12942
	public static object data;

	// Token: 0x0400328F RID: 12943
	[NonSerialized]
	public string dragType = "generic";

	// Token: 0x06003787 RID: 14215 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
	}

	// Token: 0x1700046B RID: 1131
	// (get) Token: 0x06003788 RID: 14216 RVA: 0x0014DA02 File Offset: 0x0014BC02
	protected virtual Canvas TopCanvas
	{
		get
		{
			return UIRootScaled.DragOverlayCanvas;
		}
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDrag(PointerEventData eventData)
	{
	}

	// Token: 0x0600378A RID: 14218 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnEndDrag(PointerEventData eventData)
	{
	}

	// Token: 0x0600378B RID: 14219 RVA: 0x0014DA09 File Offset: 0x0014BC09
	public void CancelDrag()
	{
		this.OnEndDrag(new PointerEventData(EventSystem.current));
	}
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020008DF RID: 2271
public class DropMe : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	// Token: 0x04003292 RID: 12946
	public string[] droppableTypes;

	// Token: 0x0600378E RID: 14222 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDrop(PointerEventData eventData)
	{
	}
}

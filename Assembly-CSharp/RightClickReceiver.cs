using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x020008E3 RID: 2275
public class RightClickReceiver : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x04003297 RID: 12951
	public UnityEvent ClickReceiver;

	// Token: 0x0600379D RID: 14237 RVA: 0x0014DB3A File Offset: 0x0014BD3A
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			UnityEvent clickReceiver = this.ClickReceiver;
			if (clickReceiver == null)
			{
				return;
			}
			clickReceiver.Invoke();
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Token: 0x0200081C RID: 2076
public class DragReceiver : MonoBehaviour
{
	// Token: 0x04002E95 RID: 11925
	public DragReceiver.TriggerEvent onEndDrag;

	// Token: 0x02000E84 RID: 3716
	[Serializable]
	public class TriggerEvent : UnityEvent<BaseEventData>
	{
	}
}

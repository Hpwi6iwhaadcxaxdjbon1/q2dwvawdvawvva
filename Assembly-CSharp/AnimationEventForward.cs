using System;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class AnimationEventForward : MonoBehaviour
{
	// Token: 0x040015DC RID: 5596
	public GameObject targetObject;

	// Token: 0x06001D0C RID: 7436 RVA: 0x000C8BF0 File Offset: 0x000C6DF0
	public void Event(string type)
	{
		this.targetObject.SendMessage(type);
	}
}

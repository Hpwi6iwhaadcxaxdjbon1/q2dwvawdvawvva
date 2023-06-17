using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005C3 RID: 1475
public class ItemEventFlag : MonoBehaviour, IItemUpdate
{
	// Token: 0x0400240A RID: 9226
	public Item.Flag flag;

	// Token: 0x0400240B RID: 9227
	public UnityEvent onEnabled = new UnityEvent();

	// Token: 0x0400240C RID: 9228
	public UnityEvent onDisable = new UnityEvent();

	// Token: 0x0400240D RID: 9229
	internal bool firstRun = true;

	// Token: 0x0400240E RID: 9230
	internal bool lastState;

	// Token: 0x06002C3C RID: 11324 RVA: 0x0010C2F8 File Offset: 0x0010A4F8
	public virtual void OnItemUpdate(Item item)
	{
		bool flag = item.HasFlag(this.flag);
		if (!this.firstRun && flag == this.lastState)
		{
			return;
		}
		if (flag)
		{
			this.onEnabled.Invoke();
		}
		else
		{
			this.onDisable.Invoke();
		}
		this.lastState = flag;
		this.firstRun = false;
	}
}

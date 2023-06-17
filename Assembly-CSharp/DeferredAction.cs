using System;
using UnityEngine;

// Token: 0x02000902 RID: 2306
public class DeferredAction
{
	// Token: 0x040032E7 RID: 13031
	private UnityEngine.Object sender;

	// Token: 0x040032E8 RID: 13032
	private Action action;

	// Token: 0x040032E9 RID: 13033
	private ActionPriority priority = ActionPriority.Medium;

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x060037E9 RID: 14313 RVA: 0x0014E97A File Offset: 0x0014CB7A
	// (set) Token: 0x060037EA RID: 14314 RVA: 0x0014E982 File Offset: 0x0014CB82
	public bool Idle { get; private set; }

	// Token: 0x17000475 RID: 1141
	// (get) Token: 0x060037EB RID: 14315 RVA: 0x0014E98B File Offset: 0x0014CB8B
	public int Index
	{
		get
		{
			return (int)this.priority;
		}
	}

	// Token: 0x060037EC RID: 14316 RVA: 0x0014E993 File Offset: 0x0014CB93
	public DeferredAction(UnityEngine.Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		this.sender = sender;
		this.action = action;
		this.priority = priority;
		this.Idle = true;
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x0014E9BE File Offset: 0x0014CBBE
	public void Action()
	{
		if (this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		this.Idle = true;
		if (this.sender)
		{
			this.action();
		}
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x0014E9F2 File Offset: 0x0014CBF2
	public void Invoke()
	{
		if (!this.Idle)
		{
			throw new Exception("Double invocation of a deferred action.");
		}
		LoadBalancer.Enqueue(this);
		this.Idle = false;
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x0014EA14 File Offset: 0x0014CC14
	public static implicit operator bool(DeferredAction obj)
	{
		return obj != null;
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x0014EA1A File Offset: 0x0014CC1A
	public static void Invoke(UnityEngine.Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
	{
		new DeferredAction(sender, action, priority).Invoke();
	}
}

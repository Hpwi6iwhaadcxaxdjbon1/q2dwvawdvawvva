using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003C6 RID: 966
public class EntityFlag_Toggle : EntityComponent<BaseEntity>, IOnPostNetworkUpdate, IOnSendNetworkUpdate, IPrefabPreProcess
{
	// Token: 0x04001A15 RID: 6677
	public bool runClientside = true;

	// Token: 0x04001A16 RID: 6678
	public bool runServerside = true;

	// Token: 0x04001A17 RID: 6679
	public BaseEntity.Flags flag;

	// Token: 0x04001A18 RID: 6680
	[SerializeField]
	[Tooltip("If multiple flags are defined in 'flag', should they all be set, or any?")]
	private EntityFlag_Toggle.FlagCheck flagCheck;

	// Token: 0x04001A19 RID: 6681
	[SerializeField]
	[Tooltip("Specify any flags that must NOT be on for this toggle to be on")]
	private BaseEntity.Flags notFlag;

	// Token: 0x04001A1A RID: 6682
	[SerializeField]
	private UnityEvent onFlagEnabled = new UnityEvent();

	// Token: 0x04001A1B RID: 6683
	[SerializeField]
	private UnityEvent onFlagDisabled = new UnityEvent();

	// Token: 0x04001A1C RID: 6684
	internal bool hasRunOnce;

	// Token: 0x04001A1D RID: 6685
	internal bool lastToggleOn;

	// Token: 0x0600218F RID: 8591 RVA: 0x000DAC73 File Offset: 0x000D8E73
	protected void OnDisable()
	{
		this.hasRunOnce = false;
		this.lastToggleOn = false;
	}

	// Token: 0x06002190 RID: 8592 RVA: 0x000DAC84 File Offset: 0x000D8E84
	public void DoUpdate(BaseEntity entity)
	{
		bool flag = (this.flagCheck == EntityFlag_Toggle.FlagCheck.All) ? entity.HasFlag(this.flag) : entity.HasAny(this.flag);
		if (entity.HasAny(this.notFlag))
		{
			flag = false;
		}
		if (this.hasRunOnce && flag == this.lastToggleOn)
		{
			return;
		}
		this.hasRunOnce = true;
		this.lastToggleOn = flag;
		if (flag)
		{
			this.onFlagEnabled.Invoke();
		}
		else
		{
			this.onFlagDisabled.Invoke();
		}
		this.OnStateToggled(flag);
	}

	// Token: 0x06002191 RID: 8593 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnStateToggled(bool state)
	{
	}

	// Token: 0x06002192 RID: 8594 RVA: 0x000DAD06 File Offset: 0x000D8F06
	public void OnPostNetworkUpdate(BaseEntity entity)
	{
		if (base.baseEntity != entity)
		{
			return;
		}
		if (!this.runClientside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x000DAD27 File Offset: 0x000D8F27
	public void OnSendNetworkUpdate(BaseEntity entity)
	{
		if (!this.runServerside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x000DAD39 File Offset: 0x000D8F39
	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !this.runClientside) && (!serverside || !this.runServerside))
		{
			process.RemoveComponent(this);
		}
	}

	// Token: 0x02000CBD RID: 3261
	private enum FlagCheck
	{
		// Token: 0x040044A5 RID: 17573
		All,
		// Token: 0x040044A6 RID: 17574
		Any
	}
}

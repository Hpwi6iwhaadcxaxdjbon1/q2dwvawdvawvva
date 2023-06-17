using System;
using Rust;
using UnityEngine;

// Token: 0x020003BB RID: 955
public class AnimatedBuildingBlock : StabilityEntity
{
	// Token: 0x04001A06 RID: 6662
	private bool animatorNeedsInitializing = true;

	// Token: 0x04001A07 RID: 6663
	private bool animatorIsOpen = true;

	// Token: 0x04001A08 RID: 6664
	private bool isAnimating;

	// Token: 0x06002170 RID: 8560 RVA: 0x000DA7C0 File Offset: 0x000D89C0
	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateAnimationParameters(true);
		}
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000DA7D6 File Offset: 0x000D89D6
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.UpdateAnimationParameters(true);
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x000DA7E5 File Offset: 0x000D89E5
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		this.UpdateAnimationParameters(false);
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000DA7F8 File Offset: 0x000D89F8
	protected void UpdateAnimationParameters(bool init)
	{
		if (!this.model)
		{
			return;
		}
		if (!this.model.animator)
		{
			return;
		}
		if (!this.model.animator.isInitialized)
		{
			return;
		}
		bool flag = this.animatorNeedsInitializing || this.animatorIsOpen != base.IsOpen() || (init && this.isAnimating);
		bool flag2 = this.animatorNeedsInitializing || init;
		if (flag)
		{
			this.isAnimating = true;
			this.model.animator.enabled = true;
			this.model.animator.SetBool("open", this.animatorIsOpen = base.IsOpen());
			if (flag2)
			{
				this.model.animator.fireEvents = false;
				if (this.model.animator.isActiveAndEnabled)
				{
					this.model.animator.Update(0f);
					this.model.animator.Update(20f);
				}
				this.PutAnimatorToSleep();
			}
			else
			{
				this.model.animator.fireEvents = base.isClient;
				if (base.isServer)
				{
					base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
				}
			}
		}
		else if (flag2)
		{
			this.PutAnimatorToSleep();
		}
		this.animatorNeedsInitializing = false;
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000DA93E File Offset: 0x000D8B3E
	protected void OnAnimatorFinished()
	{
		if (!this.isAnimating)
		{
			this.PutAnimatorToSleep();
		}
		this.isAnimating = false;
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000DA958 File Offset: 0x000D8B58
	private void PutAnimatorToSleep()
	{
		if (!this.model || !this.model.animator)
		{
			Debug.LogWarning(base.transform.GetRecursiveName("") + " has missing model/animator", base.gameObject);
			return;
		}
		this.model.animator.enabled = false;
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
		}
		this.OnAnimatorDisabled();
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void OnAnimatorDisabled()
	{
	}
}

using System;
using Rust;
using UnityEngine;

// Token: 0x0200032E RID: 814
public class DoorAnimEvents : MonoBehaviour, IClientComponent
{
	// Token: 0x04001802 RID: 6146
	public GameObjectRef openStart;

	// Token: 0x04001803 RID: 6147
	public GameObjectRef openEnd;

	// Token: 0x04001804 RID: 6148
	public GameObjectRef closeStart;

	// Token: 0x04001805 RID: 6149
	public GameObjectRef closeEnd;

	// Token: 0x04001806 RID: 6150
	public GameObject soundTarget;

	// Token: 0x04001807 RID: 6151
	public bool checkAnimSpeed;

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x06001EF0 RID: 7920 RVA: 0x000D26EC File Offset: 0x000D08EC
	public Animator animator
	{
		get
		{
			return base.GetComponent<Animator>();
		}
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x000D26F4 File Offset: 0x000D08F4
	private void DoorOpenStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed < 0f)
		{
			return;
		}
		Effect.client.Run(this.openStart.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x000D2794 File Offset: 0x000D0994
	private void DoorOpenEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.openEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed < 0f)
		{
			return;
		}
		Effect.client.Run(this.openEnd.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x000D2834 File Offset: 0x000D0A34
	private void DoorCloseStart()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeStart.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed > 0f)
		{
			return;
		}
		Effect.client.Run(this.closeStart.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}

	// Token: 0x06001EF4 RID: 7924 RVA: 0x000D28D4 File Offset: 0x000D0AD4
	private void DoorCloseEnd()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (!this.closeEnd.isValid)
		{
			return;
		}
		if (this.animator.IsInTransition(0))
		{
			return;
		}
		if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
		{
			return;
		}
		if (this.checkAnimSpeed && this.animator.GetCurrentAnimatorStateInfo(0).speed > 0f)
		{
			return;
		}
		Effect.client.Run(this.closeEnd.resourcePath, (this.soundTarget == null) ? base.gameObject : this.soundTarget);
	}
}

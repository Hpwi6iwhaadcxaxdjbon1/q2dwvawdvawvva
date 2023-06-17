using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000708 RID: 1800
public class WaterVisibilityTrigger : EnvironmentVolumeTrigger
{
	// Token: 0x0400294C RID: 10572
	public bool togglePhysics = true;

	// Token: 0x0400294D RID: 10573
	public bool toggleVisuals = true;

	// Token: 0x0400294E RID: 10574
	private long enteredTick;

	// Token: 0x0400294F RID: 10575
	private static long ticks = 1L;

	// Token: 0x04002950 RID: 10576
	private static SortedList<long, WaterVisibilityTrigger> tracker = new SortedList<long, WaterVisibilityTrigger>();

	// Token: 0x060032E0 RID: 13024 RVA: 0x00139814 File Offset: 0x00137A14
	public static void Reset()
	{
		WaterVisibilityTrigger.ticks = 1L;
		WaterVisibilityTrigger.tracker.Clear();
	}

	// Token: 0x060032E1 RID: 13025 RVA: 0x00139827 File Offset: 0x00137A27
	protected void OnDestroy()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
	}

	// Token: 0x060032E2 RID: 13026 RVA: 0x00007A3C File Offset: 0x00005C3C
	private int GetVisibilityMask()
	{
		return 0;
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ToggleVisibility()
	{
	}

	// Token: 0x060032E4 RID: 13028 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ResetVisibility()
	{
	}

	// Token: 0x060032E5 RID: 13029 RVA: 0x00139842 File Offset: 0x00137A42
	private void ToggleCollision(Collider other)
	{
		if (this.togglePhysics && WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, true);
		}
	}

	// Token: 0x060032E6 RID: 13030 RVA: 0x00139870 File Offset: 0x00137A70
	private void ResetCollision(Collider other)
	{
		if (this.togglePhysics && WaterSystem.Collision != null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, false);
		}
	}

	// Token: 0x060032E7 RID: 13031 RVA: 0x001398A0 File Offset: 0x00137AA0
	protected void OnTriggerEnter(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && !WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			long num = WaterVisibilityTrigger.ticks;
			WaterVisibilityTrigger.ticks = num + 1L;
			this.enteredTick = num;
			WaterVisibilityTrigger.tracker.Add(this.enteredTick, this);
			this.ToggleVisibility();
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ToggleCollision(other);
		}
	}

	// Token: 0x060032E8 RID: 13032 RVA: 0x00139920 File Offset: 0x00137B20
	protected void OnTriggerExit(Collider other)
	{
		bool flag = other.gameObject.GetComponent<PlayerWalkMovement>() != null;
		bool flag2 = other.gameObject.CompareTag("MainCamera");
		if ((flag || flag2) && WaterVisibilityTrigger.tracker.ContainsValue(this))
		{
			WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
			if (WaterVisibilityTrigger.tracker.Count > 0)
			{
				WaterVisibilityTrigger.tracker.Values[WaterVisibilityTrigger.tracker.Count - 1].ToggleVisibility();
			}
			else
			{
				this.ResetVisibility();
			}
		}
		if (!flag2 && !other.isTrigger)
		{
			this.ResetCollision(other);
		}
	}
}

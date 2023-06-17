using System;
using UnityEngine;

// Token: 0x02000427 RID: 1063
public class TorpedoServerProjectile : ServerProjectile
{
	// Token: 0x04001C0B RID: 7179
	[Tooltip("Make sure to leave some allowance for waves, which affect the true depth.")]
	[SerializeField]
	private float minWaterDepth = 0.5f;

	// Token: 0x04001C0C RID: 7180
	[SerializeField]
	private float shallowWaterInaccuracy;

	// Token: 0x04001C0D RID: 7181
	[SerializeField]
	private float deepWaterInaccuracy;

	// Token: 0x04001C0E RID: 7182
	[SerializeField]
	private float shallowWaterCutoff = 2f;

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x060023EA RID: 9194 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool HasRangeLimit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x060023EB RID: 9195 RVA: 0x000E55E4 File Offset: 0x000E37E4
	protected override int mask
	{
		get
		{
			return 1236478721;
		}
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000E55EC File Offset: 0x000E37EC
	public override bool DoMovement()
	{
		if (!base.DoMovement())
		{
			return false;
		}
		float num = WaterLevel.GetWaterInfo(base.transform.position, true, null, false).surfaceLevel - base.transform.position.y;
		if (num < -1f)
		{
			this.gravityModifier = 1f;
		}
		else if (num <= this.minWaterDepth)
		{
			Vector3 currentVelocity = base.CurrentVelocity;
			currentVelocity.y = 0f;
			base.CurrentVelocity = currentVelocity;
			this.gravityModifier = 0.1f;
		}
		else if (num > this.minWaterDepth + 0.3f && num <= this.minWaterDepth + 0.7f)
		{
			this.gravityModifier = -0.1f;
		}
		else
		{
			this.gravityModifier = Mathf.Clamp(base.CurrentVelocity.y, -0.1f, 0.1f);
		}
		return true;
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000E56C0 File Offset: 0x000E38C0
	public override void InitializeVelocity(Vector3 overrideVel)
	{
		base.InitializeVelocity(overrideVel);
		float value = WaterLevel.GetWaterInfo(base.transform.position, true, null, false).surfaceLevel - base.transform.position.y;
		float t = Mathf.InverseLerp(this.shallowWaterCutoff, this.shallowWaterCutoff + 2f, value);
		float maxAngle = Mathf.Lerp(this.shallowWaterInaccuracy, this.deepWaterInaccuracy, t);
		this.initialVelocity = this.initialVelocity.GetWithInaccuracy(maxAngle);
		base.CurrentVelocity = this.initialVelocity;
	}
}

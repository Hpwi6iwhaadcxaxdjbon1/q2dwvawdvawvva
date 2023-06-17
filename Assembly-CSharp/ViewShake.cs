using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002F2 RID: 754
public class ViewShake
{
	// Token: 0x0400176D RID: 5997
	protected List<ViewShake.ShakeParameters> Entries = new List<ViewShake.ShakeParameters>();

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06001E0F RID: 7695 RVA: 0x000CD26A File Offset: 0x000CB46A
	// (set) Token: 0x06001E10 RID: 7696 RVA: 0x000CD272 File Offset: 0x000CB472
	public Vector3 PositionOffset { get; protected set; }

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06001E11 RID: 7697 RVA: 0x000CD27B File Offset: 0x000CB47B
	// (set) Token: 0x06001E12 RID: 7698 RVA: 0x000CD283 File Offset: 0x000CB483
	public Vector3 AnglesOffset { get; protected set; }

	// Token: 0x06001E13 RID: 7699 RVA: 0x000CD28C File Offset: 0x000CB48C
	public void AddShake(float amplitude, float frequency, float duration)
	{
		this.Entries.Add(new ViewShake.ShakeParameters
		{
			amplitude = amplitude,
			frequency = Mathf.Max(frequency, 0.01f),
			duration = duration,
			endTime = Time.time + duration,
			nextShake = 0f,
			angle = 0f,
			infinite = (duration <= 0f)
		});
	}

	// Token: 0x06001E14 RID: 7700 RVA: 0x000CD2FC File Offset: 0x000CB4FC
	public void Update()
	{
		Vector3 a = Vector3.zero;
		Vector3 zero = Vector3.zero;
		this.Entries.RemoveAll((ViewShake.ShakeParameters i) => !i.infinite && Time.time > i.endTime);
		foreach (ViewShake.ShakeParameters shakeParameters in this.Entries)
		{
			if (Time.time > shakeParameters.nextShake)
			{
				shakeParameters.nextShake = Time.time + 1f / shakeParameters.frequency;
				shakeParameters.offset = new Vector3(UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude), UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude), UnityEngine.Random.Range(-shakeParameters.amplitude, shakeParameters.amplitude));
				shakeParameters.angle = UnityEngine.Random.Range(-shakeParameters.amplitude * 0.25f, shakeParameters.amplitude * 0.25f);
			}
			float num = 0f;
			float num2 = shakeParameters.infinite ? 1f : ((shakeParameters.endTime - Time.time) / shakeParameters.duration);
			if (num2 != 0f)
			{
				num = shakeParameters.frequency / num2;
			}
			num2 *= num2;
			float f = Time.time * num;
			num2 *= Mathf.Sin(f);
			a += shakeParameters.offset * num2;
			zero.z += shakeParameters.angle * num2;
			if (!shakeParameters.infinite)
			{
				shakeParameters.amplitude -= shakeParameters.amplitude * Time.deltaTime / (shakeParameters.duration * shakeParameters.frequency);
			}
		}
		this.PositionOffset = a * 0.01f;
		this.AnglesOffset = zero;
	}

	// Token: 0x06001E15 RID: 7701 RVA: 0x000CD4E8 File Offset: 0x000CB6E8
	public void Stop()
	{
		this.Entries.Clear();
		this.PositionOffset = Vector3.zero;
		this.AnglesOffset = Vector3.zero;
	}

	// Token: 0x02000C9B RID: 3227
	protected class ShakeParameters
	{
		// Token: 0x04004409 RID: 17417
		public float endTime;

		// Token: 0x0400440A RID: 17418
		public float duration;

		// Token: 0x0400440B RID: 17419
		public float amplitude;

		// Token: 0x0400440C RID: 17420
		public float frequency;

		// Token: 0x0400440D RID: 17421
		public float nextShake;

		// Token: 0x0400440E RID: 17422
		public float angle;

		// Token: 0x0400440F RID: 17423
		public Vector3 offset;

		// Token: 0x04004410 RID: 17424
		public bool infinite;
	}
}

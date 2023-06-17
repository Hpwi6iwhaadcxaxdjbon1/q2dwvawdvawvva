using System;
using ConVar;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class PhysicsEffects : MonoBehaviour
{
	// Token: 0x04001875 RID: 6261
	public BaseEntity entity;

	// Token: 0x04001876 RID: 6262
	public SoundDefinition physImpactSoundDef;

	// Token: 0x04001877 RID: 6263
	public float minTimeBetweenEffects = 0.25f;

	// Token: 0x04001878 RID: 6264
	public float minDistBetweenEffects = 0.1f;

	// Token: 0x04001879 RID: 6265
	public float hardnessScale = 1f;

	// Token: 0x0400187A RID: 6266
	public float lowMedThreshold = 0.4f;

	// Token: 0x0400187B RID: 6267
	public float medHardThreshold = 0.7f;

	// Token: 0x0400187C RID: 6268
	public float enableDelay = 0.1f;

	// Token: 0x0400187D RID: 6269
	public LayerMask ignoreLayers;

	// Token: 0x0400187E RID: 6270
	private float lastEffectPlayed;

	// Token: 0x0400187F RID: 6271
	private float enabledAt = float.PositiveInfinity;

	// Token: 0x04001880 RID: 6272
	private float ignoreImpactThreshold = 0.02f;

	// Token: 0x04001881 RID: 6273
	private Vector3 lastCollisionPos;

	// Token: 0x06001F1A RID: 7962 RVA: 0x000D3082 File Offset: 0x000D1282
	public void OnEnable()
	{
		this.enabledAt = UnityEngine.Time.time;
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000D3090 File Offset: 0x000D1290
	public void OnCollisionEnter(Collision collision)
	{
		if (!ConVar.Physics.sendeffects)
		{
			return;
		}
		if (UnityEngine.Time.time < this.enabledAt + this.enableDelay)
		{
			return;
		}
		if (UnityEngine.Time.time < this.lastEffectPlayed + this.minTimeBetweenEffects)
		{
			return;
		}
		if ((1 << collision.gameObject.layer & this.ignoreLayers) != 0)
		{
			return;
		}
		float num = collision.relativeVelocity.magnitude;
		num = num * 0.055f * this.hardnessScale;
		if (num <= this.ignoreImpactThreshold)
		{
			return;
		}
		if (Vector3.Distance(base.transform.position, this.lastCollisionPos) < this.minDistBetweenEffects && this.lastEffectPlayed != 0f)
		{
			return;
		}
		if (this.entity != null)
		{
			this.entity.SignalBroadcast(BaseEntity.Signal.PhysImpact, num.ToString(), null);
		}
		this.lastEffectPlayed = UnityEngine.Time.time;
		this.lastCollisionPos = base.transform.position;
	}
}

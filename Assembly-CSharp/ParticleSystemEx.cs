using System;
using UnityEngine;

// Token: 0x02000923 RID: 2339
public static class ParticleSystemEx
{
	// Token: 0x0600384B RID: 14411 RVA: 0x0015004A File Offset: 0x0014E24A
	public static void SetPlayingState(this ParticleSystem ps, bool play)
	{
		if (ps == null)
		{
			return;
		}
		if (play && !ps.isPlaying)
		{
			ps.Play();
			return;
		}
		if (!play && ps.isPlaying)
		{
			ps.Stop();
		}
	}

	// Token: 0x0600384C RID: 14412 RVA: 0x0015007C File Offset: 0x0014E27C
	public static void SetEmitterState(this ParticleSystem ps, bool enable)
	{
		if (enable != ps.emission.enabled)
		{
			ps.emission.enabled = enable;
		}
	}
}

using System;
using UnityEngine;

// Token: 0x02000346 RID: 838
public class MuzzleFlash_Flamelet : MonoBehaviour
{
	// Token: 0x04001863 RID: 6243
	public ParticleSystem flameletParticle;

	// Token: 0x06001F11 RID: 7953 RVA: 0x000D2E4C File Offset: 0x000D104C
	private void OnEnable()
	{
		this.flameletParticle.shape.angle = (float)UnityEngine.Random.Range(6, 13);
		float num = UnityEngine.Random.Range(7f, 9f);
		this.flameletParticle.startSpeed = UnityEngine.Random.Range(2.5f, num);
		this.flameletParticle.startSize = UnityEngine.Random.Range(0.05f, num * 0.015f);
	}
}

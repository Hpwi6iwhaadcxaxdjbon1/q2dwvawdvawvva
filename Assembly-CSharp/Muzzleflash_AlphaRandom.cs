using System;
using UnityEngine;

// Token: 0x02000347 RID: 839
public class Muzzleflash_AlphaRandom : MonoBehaviour
{
	// Token: 0x04001864 RID: 6244
	public ParticleSystem[] muzzleflashParticles;

	// Token: 0x04001865 RID: 6245
	private Gradient grad = new Gradient();

	// Token: 0x04001866 RID: 6246
	private GradientColorKey[] gck = new GradientColorKey[3];

	// Token: 0x04001867 RID: 6247
	private GradientAlphaKey[] gak = new GradientAlphaKey[3];

	// Token: 0x06001F13 RID: 7955 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000D2EB8 File Offset: 0x000D10B8
	private void OnEnable()
	{
		this.gck[0].color = Color.white;
		this.gck[0].time = 0f;
		this.gck[1].color = Color.white;
		this.gck[1].time = 0.6f;
		this.gck[2].color = Color.black;
		this.gck[2].time = 0.75f;
		float alpha = UnityEngine.Random.Range(0.2f, 0.85f);
		this.gak[0].alpha = alpha;
		this.gak[0].time = 0f;
		this.gak[1].alpha = alpha;
		this.gak[1].time = 0.45f;
		this.gak[2].alpha = 0f;
		this.gak[2].time = 0.5f;
		this.grad.SetKeys(this.gck, this.gak);
		foreach (ParticleSystem particleSystem in this.muzzleflashParticles)
		{
			if (particleSystem == null)
			{
				Debug.LogWarning("Muzzleflash_AlphaRandom : null particle system in " + base.gameObject.name);
			}
			else
			{
				particleSystem.colorOverLifetime.color = this.grad;
			}
		}
	}
}

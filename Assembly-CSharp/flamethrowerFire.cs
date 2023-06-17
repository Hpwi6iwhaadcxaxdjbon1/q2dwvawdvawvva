using System;
using UnityEngine;

// Token: 0x02000982 RID: 2434
public class flamethrowerFire : MonoBehaviour
{
	// Token: 0x0400342E RID: 13358
	public ParticleSystem pilotLightFX;

	// Token: 0x0400342F RID: 13359
	public ParticleSystem[] flameFX;

	// Token: 0x04003430 RID: 13360
	public FlameJet jet;

	// Token: 0x04003431 RID: 13361
	public AudioSource oneShotSound;

	// Token: 0x04003432 RID: 13362
	public AudioSource loopSound;

	// Token: 0x04003433 RID: 13363
	public AudioClip pilotlightIdle;

	// Token: 0x04003434 RID: 13364
	public AudioClip flameLoop;

	// Token: 0x04003435 RID: 13365
	public AudioClip flameStart;

	// Token: 0x04003436 RID: 13366
	public flamethrowerState flameState;

	// Token: 0x04003437 RID: 13367
	private flamethrowerState previousflameState;

	// Token: 0x060039F8 RID: 14840 RVA: 0x00157810 File Offset: 0x00155A10
	public void PilotLightOn()
	{
		this.pilotLightFX.enableEmission = true;
		this.SetFlameStatus(false);
	}

	// Token: 0x060039F9 RID: 14841 RVA: 0x00157828 File Offset: 0x00155A28
	public void SetFlameStatus(bool status)
	{
		ParticleSystem[] array = this.flameFX;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = status;
		}
	}

	// Token: 0x060039FA RID: 14842 RVA: 0x00157853 File Offset: 0x00155A53
	public void ShutOff()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(false);
	}

	// Token: 0x060039FB RID: 14843 RVA: 0x00157868 File Offset: 0x00155A68
	public void FlameOn()
	{
		this.pilotLightFX.enableEmission = false;
		this.SetFlameStatus(true);
	}

	// Token: 0x060039FC RID: 14844 RVA: 0x00157880 File Offset: 0x00155A80
	private void Start()
	{
		this.previousflameState = (this.flameState = flamethrowerState.OFF);
	}

	// Token: 0x060039FD RID: 14845 RVA: 0x001578A0 File Offset: 0x00155AA0
	private void Update()
	{
		if (this.previousflameState != this.flameState)
		{
			switch (this.flameState)
			{
			case flamethrowerState.OFF:
				this.ShutOff();
				break;
			case flamethrowerState.PILOT_LIGHT:
				this.PilotLightOn();
				break;
			case flamethrowerState.FLAME_ON:
				this.FlameOn();
				break;
			}
			this.previousflameState = this.flameState;
			this.jet.SetOn(this.flameState == flamethrowerState.FLAME_ON);
		}
	}
}

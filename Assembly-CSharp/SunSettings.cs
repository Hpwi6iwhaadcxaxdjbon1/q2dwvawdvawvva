using System;
using ConVar;
using UnityEngine;

// Token: 0x020002E8 RID: 744
public class SunSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x04001759 RID: 5977
	private Light light;

	// Token: 0x06001DF9 RID: 7673 RVA: 0x000CCA53 File Offset: 0x000CAC53
	private void OnEnable()
	{
		this.light = base.GetComponent<Light>();
	}

	// Token: 0x06001DFA RID: 7674 RVA: 0x000CCA64 File Offset: 0x000CAC64
	private void Update()
	{
		LightShadows lightShadows = (LightShadows)Mathf.Clamp(ConVar.Graphics.shadowmode, 1, 2);
		if (this.light.shadows != lightShadows)
		{
			this.light.shadows = lightShadows;
		}
	}
}

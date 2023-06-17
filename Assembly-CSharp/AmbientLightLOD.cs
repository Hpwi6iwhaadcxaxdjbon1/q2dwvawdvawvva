using System;

// Token: 0x020008F1 RID: 2289
public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	// Token: 0x040032B3 RID: 12979
	public bool isDynamic;

	// Token: 0x040032B4 RID: 12980
	public float enabledRadius = 20f;

	// Token: 0x040032B5 RID: 12981
	public bool toggleFade;

	// Token: 0x040032B6 RID: 12982
	public float toggleFadeDuration = 0.5f;

	// Token: 0x060037BE RID: 14270 RVA: 0x000CAC2E File Offset: 0x000C8E2E
	protected void OnValidate()
	{
		LightEx.CheckConflict(base.gameObject);
	}
}

using System;

// Token: 0x020008F9 RID: 2297
public class DistanceFlareLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	// Token: 0x040032CB RID: 13003
	public bool isDynamic;

	// Token: 0x040032CC RID: 13004
	public float minEnabledDistance = 100f;

	// Token: 0x040032CD RID: 13005
	public float maxEnabledDistance = 600f;

	// Token: 0x040032CE RID: 13006
	public bool toggleFade;

	// Token: 0x040032CF RID: 13007
	public float toggleFadeDuration = 0.5f;
}

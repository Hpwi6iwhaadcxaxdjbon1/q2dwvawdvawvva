using System;
using UnityEngine;

// Token: 0x02000494 RID: 1172
public class GlassPane : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04001EDC RID: 7900
	public Renderer glassRendereer;

	// Token: 0x04001EDD RID: 7901
	[SerializeField]
	private BaseVehicleModule module;

	// Token: 0x04001EDE RID: 7902
	[SerializeField]
	private float showFullDamageAt = 0.75f;
}

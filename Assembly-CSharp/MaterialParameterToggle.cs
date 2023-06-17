using System;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class MaterialParameterToggle : MonoBehaviour
{
	// Token: 0x04001689 RID: 5769
	[InspectorFlags]
	public MaterialParameterToggle.ToggleMode Toggle;

	// Token: 0x0400168A RID: 5770
	public Renderer[] TargetRenderers = new Renderer[0];

	// Token: 0x0400168B RID: 5771
	[ColorUsage(true, true)]
	public Color EmissionColor;

	// Token: 0x02000C94 RID: 3220
	[Flags]
	public enum ToggleMode
	{
		// Token: 0x040043F6 RID: 17398
		Detail = 0,
		// Token: 0x040043F7 RID: 17399
		Emission = 1
	}
}

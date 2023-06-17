using System;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class EmissionToggle : MonoBehaviour, IClientComponent
{
	// Token: 0x04001838 RID: 6200
	private Color emissionColor;

	// Token: 0x04001839 RID: 6201
	public Renderer[] targetRenderers;

	// Token: 0x0400183A RID: 6202
	public int materialIndex = -1;
}

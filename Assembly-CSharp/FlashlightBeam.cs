using System;
using UnityEngine;

// Token: 0x020001C8 RID: 456
public class FlashlightBeam : MonoBehaviour, IClientComponent
{
	// Token: 0x040011D4 RID: 4564
	public Vector2 scrollDir;

	// Token: 0x040011D5 RID: 4565
	public Vector3 localEndPoint = new Vector3(0f, 0f, 2f);

	// Token: 0x040011D6 RID: 4566
	public LineRenderer beamRenderer;
}

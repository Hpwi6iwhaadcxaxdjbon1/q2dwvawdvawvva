using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001C4 RID: 452
public class ClientIOLine : BaseMonoBehaviour
{
	// Token: 0x040011C3 RID: 4547
	public RendererLOD _lod;

	// Token: 0x040011C4 RID: 4548
	public LineRenderer _line;

	// Token: 0x040011C5 RID: 4549
	public Material directionalMaterial;

	// Token: 0x040011C6 RID: 4550
	public Material defaultMaterial;

	// Token: 0x040011C7 RID: 4551
	public IOEntity.IOType lineType;

	// Token: 0x040011C8 RID: 4552
	public static List<ClientIOLine> _allLines = new List<ClientIOLine>();

	// Token: 0x040011C9 RID: 4553
	public WireTool.WireColour colour;

	// Token: 0x040011CA RID: 4554
	public IOEntity ownerIOEnt;
}

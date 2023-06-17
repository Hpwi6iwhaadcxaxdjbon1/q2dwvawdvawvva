using System;
using UnityEngine;

// Token: 0x02000532 RID: 1330
public class HLODBounds : MonoBehaviour, IEditorComponent
{
	// Token: 0x040021EB RID: 8683
	[Tooltip("The bounds that this HLOD will cover. This should not overlap with any other HLODs")]
	public Bounds MeshBounds = new Bounds(Vector3.zero, new Vector3(50f, 25f, 50f));

	// Token: 0x040021EC RID: 8684
	[Tooltip("Assets created will use this prefix. Make sure multiple HLODS in a scene have different prefixes")]
	public string MeshPrefix = "root";

	// Token: 0x040021ED RID: 8685
	[Tooltip("The point from which to calculate the HLOD. Any RendererLODs that are visible at this distance will baked into the HLOD mesh")]
	public float CullDistance = 100f;

	// Token: 0x040021EE RID: 8686
	[Tooltip("If set, the lod will take over at this distance instead of the CullDistance (eg. we make a model based on what this area looks like at 200m but we actually want it take over rendering at 300m)")]
	public float OverrideLodDistance;

	// Token: 0x040021EF RID: 8687
	[Tooltip("Any renderers below this height will considered culled even if they are visible from a distance. Good for underground areas")]
	public float CullBelowHeight;

	// Token: 0x040021F0 RID: 8688
	[Tooltip("Optimises the mesh produced by removing non-visible and small faces. Can turn it off during dev but should be on for final builds")]
	public bool ApplyMeshTrimming = true;

	// Token: 0x040021F1 RID: 8689
	public MeshTrimSettings Settings = MeshTrimSettings.Default;

	// Token: 0x040021F2 RID: 8690
	public RendererLOD DebugComponent;

	// Token: 0x040021F3 RID: 8691
	public bool ShowTrimSettings;
}

using System;
using UnityEngine;

// Token: 0x02000563 RID: 1379
public class SceneToPrefabTag : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002276 RID: 8822
	public SceneToPrefabTag.TagType Type;

	// Token: 0x04002277 RID: 8823
	public int SpecificLOD;

	// Token: 0x02000D50 RID: 3408
	public enum TagType
	{
		// Token: 0x040046F3 RID: 18163
		ForceInclude,
		// Token: 0x040046F4 RID: 18164
		ForceExclude,
		// Token: 0x040046F5 RID: 18165
		SingleMaterial,
		// Token: 0x040046F6 RID: 18166
		UseSpecificLOD
	}
}

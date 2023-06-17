using System;
using UnityEngine;

// Token: 0x02000562 RID: 1378
public class SceneToPrefab : MonoBehaviour, IEditorComponent
{
	// Token: 0x04002273 RID: 8819
	public bool flattenHierarchy;

	// Token: 0x04002274 RID: 8820
	public GameObject outputPrefab;

	// Token: 0x04002275 RID: 8821
	[Tooltip("If true the HLOD generation will be skipped and the previous results will be used, good to use if non-visual changes were made (eg.triggers)")]
	public bool skipAllHlod;
}

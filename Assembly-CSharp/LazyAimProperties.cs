using System;
using UnityEngine;

// Token: 0x0200055B RID: 1371
[CreateAssetMenu(menuName = "Rust/LazyAim Properties")]
public class LazyAimProperties : ScriptableObject
{
	// Token: 0x04002262 RID: 8802
	[Range(0f, 10f)]
	public float snapStrength = 6f;

	// Token: 0x04002263 RID: 8803
	[Range(0f, 45f)]
	public float deadzoneAngle = 1f;
}

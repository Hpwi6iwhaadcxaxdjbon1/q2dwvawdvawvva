using System;
using UnityEngine;

// Token: 0x02000677 RID: 1655
public class RandomDynamicPrefab : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x04002712 RID: 10002
	public uint Seed;

	// Token: 0x04002713 RID: 10003
	public float Distance = 100f;

	// Token: 0x04002714 RID: 10004
	public float Probability = 0.5f;

	// Token: 0x04002715 RID: 10005
	public string ResourceFolder = string.Empty;
}

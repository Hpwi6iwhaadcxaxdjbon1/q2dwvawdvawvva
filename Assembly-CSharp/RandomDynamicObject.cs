using System;
using UnityEngine;

// Token: 0x02000676 RID: 1654
public class RandomDynamicObject : MonoBehaviour, IClientComponent, ILOD
{
	// Token: 0x0400270E RID: 9998
	public uint Seed;

	// Token: 0x0400270F RID: 9999
	public float Distance = 100f;

	// Token: 0x04002710 RID: 10000
	public float Probability = 0.5f;

	// Token: 0x04002711 RID: 10001
	public GameObject[] Candidates;
}

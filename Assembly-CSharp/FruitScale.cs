using System;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class FruitScale : MonoBehaviour, IClientComponent
{
	// Token: 0x06002465 RID: 9317 RVA: 0x000E78FE File Offset: 0x000E5AFE
	public void SetProgress(float progress)
	{
		base.transform.localScale = Vector3.one * progress;
	}
}

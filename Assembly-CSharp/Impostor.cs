using System;
using UnityEngine;

// Token: 0x02000724 RID: 1828
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Impostor : MonoBehaviour, IClientComponent
{
	// Token: 0x04002994 RID: 10644
	public ImpostorAsset asset;

	// Token: 0x04002995 RID: 10645
	[Header("Baking")]
	public GameObject reference;

	// Token: 0x04002996 RID: 10646
	public float angle;

	// Token: 0x04002997 RID: 10647
	public int resolution = 1024;

	// Token: 0x04002998 RID: 10648
	public int padding = 32;

	// Token: 0x04002999 RID: 10649
	public bool spriteOutlineAsMesh;

	// Token: 0x06003327 RID: 13095 RVA: 0x000063A5 File Offset: 0x000045A5
	private void OnEnable()
	{
	}
}

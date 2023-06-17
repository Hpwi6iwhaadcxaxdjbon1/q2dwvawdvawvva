using System;
using UnityEngine;

// Token: 0x0200072F RID: 1839
[ExecuteInEditMode]
[RequireComponent(typeof(CommandBufferManager))]
public class PostOpaqueDepth : MonoBehaviour
{
	// Token: 0x040029B8 RID: 10680
	public RenderTexture postOpaqueDepth;

	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x06003352 RID: 13138 RVA: 0x0013AF16 File Offset: 0x00139116
	public RenderTexture PostOpaque
	{
		get
		{
			return this.postOpaqueDepth;
		}
	}
}

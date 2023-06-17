using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002E6 RID: 742
public class SkinnedMultiMesh : MonoBehaviour
{
	// Token: 0x04001741 RID: 5953
	public bool shadowOnly;

	// Token: 0x04001742 RID: 5954
	internal bool IsVisible = true;

	// Token: 0x04001743 RID: 5955
	public bool eyesView;

	// Token: 0x04001744 RID: 5956
	public Skeleton skeleton;

	// Token: 0x04001745 RID: 5957
	public SkeletonSkinLod skeletonSkinLod;

	// Token: 0x04001746 RID: 5958
	public List<SkinnedMultiMesh.Part> parts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x04001747 RID: 5959
	[NonSerialized]
	public List<SkinnedMultiMesh.Part> createdParts = new List<SkinnedMultiMesh.Part>();

	// Token: 0x04001748 RID: 5960
	[NonSerialized]
	public long lastBuildHash;

	// Token: 0x04001749 RID: 5961
	[NonSerialized]
	public MaterialPropertyBlock sharedPropertyBlock;

	// Token: 0x0400174A RID: 5962
	[NonSerialized]
	public MaterialPropertyBlock hairPropertyBlock;

	// Token: 0x0400174B RID: 5963
	public float skinNumber;

	// Token: 0x0400174C RID: 5964
	public float meshNumber;

	// Token: 0x0400174D RID: 5965
	public float hairNumber;

	// Token: 0x0400174E RID: 5966
	public int skinType;

	// Token: 0x0400174F RID: 5967
	public SkinSetCollection SkinCollection;

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06001DEF RID: 7663 RVA: 0x000CC7E1 File Offset: 0x000CA9E1
	public List<Renderer> Renderers { get; } = new List<Renderer>(32);

	// Token: 0x02000C99 RID: 3225
	public struct Part
	{
		// Token: 0x04004405 RID: 17413
		public Wearable wearable;

		// Token: 0x04004406 RID: 17414
		public GameObject gameObject;

		// Token: 0x04004407 RID: 17415
		public string name;

		// Token: 0x04004408 RID: 17416
		public Item item;
	}
}

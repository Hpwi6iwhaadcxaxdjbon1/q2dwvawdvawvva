using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000517 RID: 1303
public class Gibbable : PrefabAttribute, IClientComponent
{
	// Token: 0x04002185 RID: 8581
	public GameObject gibSource;

	// Token: 0x04002186 RID: 8582
	public Material[] customMaterials;

	// Token: 0x04002187 RID: 8583
	public GameObject materialSource;

	// Token: 0x04002188 RID: 8584
	public bool copyMaterialBlock = true;

	// Token: 0x04002189 RID: 8585
	public bool applyDamageTexture;

	// Token: 0x0400218A RID: 8586
	public PhysicMaterial physicsMaterial;

	// Token: 0x0400218B RID: 8587
	public GameObjectRef fxPrefab;

	// Token: 0x0400218C RID: 8588
	public bool spawnFxPrefab = true;

	// Token: 0x0400218D RID: 8589
	[Tooltip("If enabled, gibs will spawn even though we've hit a gib limit")]
	public bool important;

	// Token: 0x0400218E RID: 8590
	public bool useContinuousCollision;

	// Token: 0x0400218F RID: 8591
	public float explodeScale;

	// Token: 0x04002190 RID: 8592
	public float scaleOverride = 1f;

	// Token: 0x04002191 RID: 8593
	[ReadOnly]
	public int uniqueId;

	// Token: 0x04002192 RID: 8594
	public Gibbable.BoundsEffectType boundsEffectType;

	// Token: 0x04002193 RID: 8595
	public bool isConditional;

	// Token: 0x04002194 RID: 8596
	[ReadOnly]
	public Bounds effectBounds;

	// Token: 0x04002195 RID: 8597
	public List<Gibbable.OverrideMesh> MeshOverrides = new List<Gibbable.OverrideMesh>();

	// Token: 0x06002991 RID: 10641 RVA: 0x000FEBF5 File Offset: 0x000FCDF5
	protected override Type GetIndexedType()
	{
		return typeof(Gibbable);
	}

	// Token: 0x02000D38 RID: 3384
	[Serializable]
	public struct OverrideMesh
	{
		// Token: 0x04004691 RID: 18065
		public bool enabled;

		// Token: 0x04004692 RID: 18066
		public Gibbable.ColliderType ColliderType;

		// Token: 0x04004693 RID: 18067
		public Vector3 BoxSize;

		// Token: 0x04004694 RID: 18068
		public Vector3 ColliderCentre;

		// Token: 0x04004695 RID: 18069
		public float ColliderRadius;

		// Token: 0x04004696 RID: 18070
		public float CapsuleHeight;

		// Token: 0x04004697 RID: 18071
		public int CapsuleDirection;

		// Token: 0x04004698 RID: 18072
		public bool BlockMaterialCopy;
	}

	// Token: 0x02000D39 RID: 3385
	public enum ColliderType
	{
		// Token: 0x0400469A RID: 18074
		Box,
		// Token: 0x0400469B RID: 18075
		Sphere,
		// Token: 0x0400469C RID: 18076
		Capsule
	}

	// Token: 0x02000D3A RID: 3386
	public enum ParentingType
	{
		// Token: 0x0400469E RID: 18078
		None,
		// Token: 0x0400469F RID: 18079
		GibsOnly,
		// Token: 0x040046A0 RID: 18080
		FXOnly,
		// Token: 0x040046A1 RID: 18081
		All
	}

	// Token: 0x02000D3B RID: 3387
	public enum BoundsEffectType
	{
		// Token: 0x040046A3 RID: 18083
		None,
		// Token: 0x040046A4 RID: 18084
		Electrical,
		// Token: 0x040046A5 RID: 18085
		Glass,
		// Token: 0x040046A6 RID: 18086
		Scrap,
		// Token: 0x040046A7 RID: 18087
		Stone,
		// Token: 0x040046A8 RID: 18088
		Wood
	}
}

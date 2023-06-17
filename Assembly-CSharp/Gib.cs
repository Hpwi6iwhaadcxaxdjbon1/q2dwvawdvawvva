using System;
using UnityEngine;

// Token: 0x020002C2 RID: 706
public class Gib : ListComponent<Gib>
{
	// Token: 0x04001656 RID: 5718
	public static int gibCount;

	// Token: 0x04001657 RID: 5719
	public MeshFilter _meshFilter;

	// Token: 0x04001658 RID: 5720
	public MeshRenderer _meshRenderer;

	// Token: 0x04001659 RID: 5721
	public MeshCollider _meshCollider;

	// Token: 0x0400165A RID: 5722
	public BoxCollider _boxCollider;

	// Token: 0x0400165B RID: 5723
	public SphereCollider _sphereCollider;

	// Token: 0x0400165C RID: 5724
	public CapsuleCollider _capsuleCollider;

	// Token: 0x0400165D RID: 5725
	public Rigidbody _rigidbody;

	// Token: 0x06001D70 RID: 7536 RVA: 0x000CABAC File Offset: 0x000C8DAC
	public static string GetEffect(PhysicMaterial physicMaterial)
	{
		string nameLower = physicMaterial.GetNameLower();
		if (nameLower == "wood")
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		if (nameLower == "concrete")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (nameLower == "metal")
		{
			return "assets/bundled/prefabs/fx/building/metal_sheet_gib.prefab";
		}
		if (nameLower == "rock")
		{
			return "assets/bundled/prefabs/fx/building/stone_gib.prefab";
		}
		if (!(nameLower == "flesh"))
		{
			return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
		}
		return "assets/bundled/prefabs/fx/building/wood_gib.prefab";
	}
}

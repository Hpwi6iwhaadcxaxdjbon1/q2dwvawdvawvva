using System;
using UnityEngine;

// Token: 0x02000559 RID: 1369
[CreateAssetMenu(menuName = "Rust/Clothing Movement Properties")]
public class ClothingMovementProperties : ScriptableObject
{
	// Token: 0x0400225D RID: 8797
	public float speedReduction;

	// Token: 0x0400225E RID: 8798
	[Tooltip("If this piece of clothing is worn movement speed will be reduced by atleast this much")]
	public float minSpeedReduction;

	// Token: 0x0400225F RID: 8799
	public float waterSpeedBonus;
}

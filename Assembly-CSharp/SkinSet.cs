using System;
using UnityEngine;

// Token: 0x0200075D RID: 1885
[CreateAssetMenu(menuName = "Rust/Skin Set")]
public class SkinSet : ScriptableObject
{
	// Token: 0x04002ABC RID: 10940
	public string Label;

	// Token: 0x04002ABD RID: 10941
	public Gradient SkinColour;

	// Token: 0x04002ABE RID: 10942
	public HairSetCollection HairCollection;

	// Token: 0x04002ABF RID: 10943
	[Header("Models")]
	public GameObjectRef Head;

	// Token: 0x04002AC0 RID: 10944
	public GameObjectRef Torso;

	// Token: 0x04002AC1 RID: 10945
	public GameObjectRef Legs;

	// Token: 0x04002AC2 RID: 10946
	public GameObjectRef Feet;

	// Token: 0x04002AC3 RID: 10947
	public GameObjectRef Hands;

	// Token: 0x04002AC4 RID: 10948
	[Header("Censored Variants")]
	public GameObjectRef CensoredTorso;

	// Token: 0x04002AC5 RID: 10949
	public GameObjectRef CensoredLegs;

	// Token: 0x04002AC6 RID: 10950
	[Header("Materials")]
	public Material HeadMaterial;

	// Token: 0x04002AC7 RID: 10951
	public Material BodyMaterial;

	// Token: 0x04002AC8 RID: 10952
	public Material EyeMaterial;

	// Token: 0x06003498 RID: 13464 RVA: 0x00145AEC File Offset: 0x00143CEC
	internal Color GetSkinColor(float skinNumber)
	{
		return this.SkinColour.Evaluate(skinNumber);
	}
}

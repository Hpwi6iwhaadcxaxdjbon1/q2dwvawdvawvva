using System;
using UnityEngine;

// Token: 0x020002F4 RID: 756
[RequireComponent(typeof(Wearable))]
public class WearableHairCap : MonoBehaviour
{
	// Token: 0x0400176E RID: 5998
	public HairType Type;

	// Token: 0x0400176F RID: 5999
	[ColorUsage(false, true)]
	public Color BaseColor = Color.black;

	// Token: 0x04001770 RID: 6000
	public Texture Mask;

	// Token: 0x04001771 RID: 6001
	private static MaterialPropertyBlock block;

	// Token: 0x04001772 RID: 6002
	private static int _HairBaseColorUV1 = Shader.PropertyToID("_HairBaseColorUV1");

	// Token: 0x04001773 RID: 6003
	private static int _HairBaseColorUV2 = Shader.PropertyToID("_HairBaseColorUV2");

	// Token: 0x04001774 RID: 6004
	private static int _HairPackedMapUV1 = Shader.PropertyToID("_HairPackedMapUV1");

	// Token: 0x04001775 RID: 6005
	private static int _HairPackedMapUV2 = Shader.PropertyToID("_HairPackedMapUV2");

	// Token: 0x06001E18 RID: 7704 RVA: 0x000CD520 File Offset: 0x000CB720
	public void ApplyHairCap(MaterialPropertyBlock block)
	{
		if (this.Type == HairType.Head || this.Type == HairType.Armpit || this.Type == HairType.Pubic)
		{
			Texture texture = block.GetTexture(WearableHairCap._HairPackedMapUV1);
			block.SetColor(WearableHairCap._HairBaseColorUV1, this.BaseColor.gamma);
			block.SetTexture(WearableHairCap._HairPackedMapUV1, (this.Mask != null) ? this.Mask : texture);
			return;
		}
		if (this.Type == HairType.Facial)
		{
			Texture texture2 = block.GetTexture(WearableHairCap._HairPackedMapUV2);
			block.SetColor(WearableHairCap._HairBaseColorUV2, this.BaseColor.gamma);
			block.SetTexture(WearableHairCap._HairPackedMapUV2, (this.Mask != null) ? this.Mask : texture2);
		}
	}
}

using System;
using UnityEngine;

// Token: 0x020002D0 RID: 720
public class OverlayMeshPaintableSource : MeshPaintableSource
{
	// Token: 0x040016BC RID: 5820
	private static readonly Memoized<string, string> STPrefixed = new Memoized<string, string>((string s) => s + "_ST");

	// Token: 0x040016BD RID: 5821
	public string baseTextureName = "_Decal1Texture";

	// Token: 0x040016BE RID: 5822
	[NonSerialized]
	public Texture2D baseTexture;

	// Token: 0x06001D99 RID: 7577 RVA: 0x000CB2F4 File Offset: 0x000C94F4
	public override void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		base.UpdateMaterials(block, textureOverride, forEditing, isSelected);
		if (this.baseTexture != null)
		{
			float num = (float)this.baseTexture.width / (float)this.baseTexture.height;
			float num2 = (float)(this.texWidth / this.texHeight);
			float num3 = 1f;
			float z = 0f;
			float num4 = 1f;
			float w = 0f;
			if (num2 <= num)
			{
				float num5 = (float)this.texHeight * num;
				num3 = (float)this.texWidth / num5;
				z = (1f - num3) / 2f;
			}
			else
			{
				float num6 = (float)this.texWidth / num;
				num4 = (float)this.texHeight / num6;
				w = (1f - num4) / 2f;
			}
			block.SetTexture(this.baseTextureName, this.baseTexture);
			block.SetVector(OverlayMeshPaintableSource.STPrefixed.Get(this.baseTextureName), new Vector4(num3, num4, z, w));
			return;
		}
		block.SetTexture(this.baseTextureName, Texture2D.blackTexture);
	}
}

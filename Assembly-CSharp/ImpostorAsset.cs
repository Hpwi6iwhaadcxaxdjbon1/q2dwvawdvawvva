using System;
using UnityEngine;

// Token: 0x02000725 RID: 1829
public class ImpostorAsset : ScriptableObject
{
	// Token: 0x0400299A RID: 10650
	public ImpostorAsset.TextureEntry[] textures;

	// Token: 0x0400299B RID: 10651
	public Vector2 size;

	// Token: 0x0400299C RID: 10652
	public Vector2 pivot;

	// Token: 0x0400299D RID: 10653
	public Mesh mesh;

	// Token: 0x06003329 RID: 13097 RVA: 0x0013A954 File Offset: 0x00138B54
	public Texture2D FindTexture(string name)
	{
		foreach (ImpostorAsset.TextureEntry textureEntry in this.textures)
		{
			if (textureEntry.name == name)
			{
				return textureEntry.texture;
			}
		}
		return null;
	}

	// Token: 0x02000E3D RID: 3645
	[Serializable]
	public class TextureEntry
	{
		// Token: 0x04004AC6 RID: 19142
		public string name;

		// Token: 0x04004AC7 RID: 19143
		public Texture2D texture;

		// Token: 0x06005248 RID: 21064 RVA: 0x001AFC43 File Offset: 0x001ADE43
		public TextureEntry(string name, Texture2D texture)
		{
			this.name = name;
			this.texture = texture;
		}
	}
}

using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class MeshPaintableSource : MonoBehaviour, IClientComponent
{
	// Token: 0x040016A9 RID: 5801
	public Vector4 uvRange = new Vector4(0f, 0f, 1f, 1f);

	// Token: 0x040016AA RID: 5802
	public int texWidth = 256;

	// Token: 0x040016AB RID: 5803
	public int texHeight = 128;

	// Token: 0x040016AC RID: 5804
	public string replacementTextureName = "_DecalTexture";

	// Token: 0x040016AD RID: 5805
	public float cameraFOV = 60f;

	// Token: 0x040016AE RID: 5806
	public float cameraDistance = 2f;

	// Token: 0x040016AF RID: 5807
	[NonSerialized]
	public Texture2D texture;

	// Token: 0x040016B0 RID: 5808
	public GameObject sourceObject;

	// Token: 0x040016B1 RID: 5809
	public Mesh collisionMesh;

	// Token: 0x040016B2 RID: 5810
	public Vector3 localPosition;

	// Token: 0x040016B3 RID: 5811
	public Vector3 localRotation;

	// Token: 0x040016B4 RID: 5812
	public bool applyToAllRenderers = true;

	// Token: 0x040016B5 RID: 5813
	public Renderer[] extraRenderers;

	// Token: 0x040016B6 RID: 5814
	public bool paint3D;

	// Token: 0x040016B7 RID: 5815
	public bool applyToSkinRenderers = true;

	// Token: 0x040016B8 RID: 5816
	public bool applyToFirstPersonLegs = true;

	// Token: 0x040016B9 RID: 5817
	[NonSerialized]
	public bool isSelected;

	// Token: 0x040016BA RID: 5818
	[NonSerialized]
	public Renderer legRenderer;

	// Token: 0x040016BB RID: 5819
	private static MaterialPropertyBlock block;

	// Token: 0x06001D91 RID: 7569 RVA: 0x000CAFF4 File Offset: 0x000C91F4
	public void Init()
	{
		if (this.texture == null)
		{
			this.texture = new Texture2D(this.texWidth, this.texHeight, TextureFormat.ARGB32, false);
			this.texture.name = "MeshPaintableSource_" + base.gameObject.name;
			this.texture.wrapMode = TextureWrapMode.Clamp;
			this.texture.Clear(Color.clear);
		}
		if (MeshPaintableSource.block == null)
		{
			MeshPaintableSource.block = new MaterialPropertyBlock();
		}
		else
		{
			MeshPaintableSource.block.Clear();
		}
		this.UpdateMaterials(MeshPaintableSource.block, null, false, this.isSelected);
		List<Renderer> list = Pool.GetList<Renderer>();
		(this.applyToAllRenderers ? base.transform.root : base.transform).GetComponentsInChildren<Renderer>(true, list);
		foreach (Renderer renderer in list)
		{
			PlayerModelSkin playerModelSkin;
			if (this.applyToSkinRenderers || !renderer.TryGetComponent<PlayerModelSkin>(out playerModelSkin))
			{
				renderer.SetPropertyBlock(MeshPaintableSource.block);
			}
		}
		if (this.extraRenderers != null)
		{
			foreach (Renderer renderer2 in this.extraRenderers)
			{
				if (renderer2 != null)
				{
					renderer2.SetPropertyBlock(MeshPaintableSource.block);
				}
			}
		}
		if (this.applyToFirstPersonLegs && this.legRenderer != null)
		{
			this.legRenderer.SetPropertyBlock(MeshPaintableSource.block);
		}
		Pool.FreeList<Renderer>(ref list);
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x000CB188 File Offset: 0x000C9388
	public void Free()
	{
		if (this.texture)
		{
			UnityEngine.Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06001D93 RID: 7571 RVA: 0x000CB1A9 File Offset: 0x000C93A9
	public virtual void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		block.SetTexture(this.replacementTextureName, textureOverride ?? this.texture);
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x000CB1C4 File Offset: 0x000C93C4
	public virtual Color32[] UpdateFrom(Texture2D input)
	{
		this.Init();
		Color32[] pixels = input.GetPixels32();
		this.texture.SetPixels32(pixels);
		this.texture.Apply(true, false);
		return pixels;
	}

	// Token: 0x06001D95 RID: 7573 RVA: 0x000CB1F8 File Offset: 0x000C93F8
	public void Load(byte[] data)
	{
		this.Init();
		if (data != null)
		{
			this.texture.LoadImage(data);
			this.texture.Apply(true, false);
		}
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x000CB220 File Offset: 0x000C9420
	public void Clear()
	{
		if (this.texture == null)
		{
			return;
		}
		this.texture.Clear(new Color(0f, 0f, 0f, 0f));
		this.texture.Apply(true, false);
	}
}

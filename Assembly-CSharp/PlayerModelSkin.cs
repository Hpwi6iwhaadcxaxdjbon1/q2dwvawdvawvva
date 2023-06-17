using System;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class PlayerModelSkin : MonoBehaviour, IPrefabPreProcess
{
	// Token: 0x040016D7 RID: 5847
	public PlayerModelSkin.SkinMaterialType MaterialType;

	// Token: 0x040016D8 RID: 5848
	public Renderer SkinRenderer;

	// Token: 0x06001DB7 RID: 7607 RVA: 0x000CB914 File Offset: 0x000C9B14
	public void Setup(SkinSetCollection skin, float hairNum, float meshNum)
	{
		if (!this.SkinRenderer)
		{
			return;
		}
		if (!skin)
		{
			return;
		}
		switch (this.MaterialType)
		{
		case PlayerModelSkin.SkinMaterialType.HEAD:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).HeadMaterial;
			return;
		case PlayerModelSkin.SkinMaterialType.EYE:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).EyeMaterial;
			return;
		case PlayerModelSkin.SkinMaterialType.BODY:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).BodyMaterial;
			return;
		default:
			this.SkinRenderer.sharedMaterial = skin.Get(meshNum).BodyMaterial;
			return;
		}
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x000CB9B2 File Offset: 0x000C9BB2
	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!clientside)
		{
			return;
		}
		this.SkinRenderer = base.GetComponent<Renderer>();
	}

	// Token: 0x02000C97 RID: 3223
	public enum SkinMaterialType
	{
		// Token: 0x040043FD RID: 17405
		HEAD,
		// Token: 0x040043FE RID: 17406
		EYE,
		// Token: 0x040043FF RID: 17407
		BODY
	}
}

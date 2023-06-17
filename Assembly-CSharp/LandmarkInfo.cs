using System;
using UnityEngine;

// Token: 0x0200054A RID: 1354
public class LandmarkInfo : MonoBehaviour
{
	// Token: 0x0400221A RID: 8730
	[Header("LandmarkInfo")]
	public bool shouldDisplayOnMap;

	// Token: 0x0400221B RID: 8731
	public bool isLayerSpecific;

	// Token: 0x0400221C RID: 8732
	public Translate.Phrase displayPhrase;

	// Token: 0x0400221D RID: 8733
	public Sprite mapIcon;

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x060029E1 RID: 10721 RVA: 0x000445C9 File Offset: 0x000427C9
	public virtual MapLayer MapLayer
	{
		get
		{
			return MapLayer.Overworld;
		}
	}

	// Token: 0x060029E2 RID: 10722 RVA: 0x000FFD6D File Offset: 0x000FDF6D
	protected virtual void Awake()
	{
		if (TerrainMeta.Path)
		{
			TerrainMeta.Path.Landmarks.Add(this);
		}
	}
}

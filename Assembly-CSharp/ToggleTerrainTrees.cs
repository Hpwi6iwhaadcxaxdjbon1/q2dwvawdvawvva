using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000884 RID: 2180
public class ToggleTerrainTrees : MonoBehaviour
{
	// Token: 0x040030FC RID: 12540
	public Toggle toggleControl;

	// Token: 0x040030FD RID: 12541
	public Text textControl;

	// Token: 0x06003699 RID: 13977 RVA: 0x0014A472 File Offset: 0x00148672
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawTreesAndFoliage;
		}
	}

	// Token: 0x0600369A RID: 13978 RVA: 0x0014A495 File Offset: 0x00148695
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawTreesAndFoliage = this.toggleControl.isOn;
		}
	}

	// Token: 0x0600369B RID: 13979 RVA: 0x0014A4B8 File Offset: 0x001486B8
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Trees";
		}
	}
}

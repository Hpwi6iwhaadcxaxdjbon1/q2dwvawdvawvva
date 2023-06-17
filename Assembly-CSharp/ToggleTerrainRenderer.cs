using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000883 RID: 2179
public class ToggleTerrainRenderer : MonoBehaviour
{
	// Token: 0x040030FA RID: 12538
	public Toggle toggleControl;

	// Token: 0x040030FB RID: 12539
	public Text textControl;

	// Token: 0x06003695 RID: 13973 RVA: 0x0014A40D File Offset: 0x0014860D
	protected void OnEnable()
	{
		if (Terrain.activeTerrain)
		{
			this.toggleControl.isOn = Terrain.activeTerrain.drawHeightmap;
		}
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x0014A430 File Offset: 0x00148630
	public void OnToggleChanged()
	{
		if (Terrain.activeTerrain)
		{
			Terrain.activeTerrain.drawHeightmap = this.toggleControl.isOn;
		}
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x0014A453 File Offset: 0x00148653
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = "Terrain Renderer";
		}
	}
}

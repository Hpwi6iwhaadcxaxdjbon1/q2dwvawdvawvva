using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x02000737 RID: 1847
public class RgbEffects : SingletonComponent<RgbEffects>
{
	// Token: 0x040029E5 RID: 10725
	[ClientVar(Help = "Enables RGB lighting effects (supports SteelSeries and Razer)", Saved = true)]
	public static bool Enabled = true;

	// Token: 0x040029E6 RID: 10726
	[ClientVar(Help = "Controls how RGB values are mapped to LED lights on SteelSeries devices", Saved = true)]
	public static Vector3 ColorCorrection_SteelSeries = new Vector3(1.5f, 1.5f, 1.5f);

	// Token: 0x040029E7 RID: 10727
	[ClientVar(Help = "Controls how RGB values are mapped to LED lights on Razer devices", Saved = true)]
	public static Vector3 ColorCorrection_Razer = new Vector3(3f, 3f, 3f);

	// Token: 0x040029E8 RID: 10728
	[ClientVar(Help = "Brightness of colors, from 0 to 1 (note: may affect color accuracy)", Saved = true)]
	public static float Brightness = 1f;

	// Token: 0x040029E9 RID: 10729
	public Color defaultColor;

	// Token: 0x040029EA RID: 10730
	public Color buildingPrivilegeColor;

	// Token: 0x040029EB RID: 10731
	public Color coldColor;

	// Token: 0x040029EC RID: 10732
	public Color hotColor;

	// Token: 0x040029ED RID: 10733
	public Color hurtColor;

	// Token: 0x040029EE RID: 10734
	public Color healedColor;

	// Token: 0x040029EF RID: 10735
	public Color irradiatedColor;

	// Token: 0x040029F0 RID: 10736
	public Color comfortedColor;

	// Token: 0x0600336E RID: 13166 RVA: 0x000063A5 File Offset: 0x000045A5
	[ClientVar(Name = "static")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConVar_Static(ConsoleSystem.Arg args)
	{
	}

	// Token: 0x0600336F RID: 13167 RVA: 0x000063A5 File Offset: 0x000045A5
	[ClientVar(Name = "pulse")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConVar_Pulse(ConsoleSystem.Arg args)
	{
	}
}

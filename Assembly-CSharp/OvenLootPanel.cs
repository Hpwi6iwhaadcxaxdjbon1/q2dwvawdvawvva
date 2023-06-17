using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000844 RID: 2116
public class OvenLootPanel : MonoBehaviour
{
	// Token: 0x04002F5A RID: 12122
	public GameObject controlsOn;

	// Token: 0x04002F5B RID: 12123
	public GameObject controlsOff;

	// Token: 0x04002F5C RID: 12124
	public Image TitleBackground;

	// Token: 0x04002F5D RID: 12125
	public RustText TitleText;

	// Token: 0x04002F5E RID: 12126
	public Color AlertBackgroundColor;

	// Token: 0x04002F5F RID: 12127
	public Color AlertTextColor;

	// Token: 0x04002F60 RID: 12128
	public Color OffBackgroundColor;

	// Token: 0x04002F61 RID: 12129
	public Color OffTextColor;

	// Token: 0x04002F62 RID: 12130
	public Color OnBackgroundColor;

	// Token: 0x04002F63 RID: 12131
	public Color OnTextColor;

	// Token: 0x04002F64 RID: 12132
	private Translate.Phrase OffPhrase = new Translate.Phrase("off", "off");

	// Token: 0x04002F65 RID: 12133
	private Translate.Phrase OnPhrase = new Translate.Phrase("on", "on");

	// Token: 0x04002F66 RID: 12134
	private Translate.Phrase NoFuelPhrase = new Translate.Phrase("no_fuel", "No Fuel");

	// Token: 0x04002F67 RID: 12135
	public GameObject FuelRowPrefab;

	// Token: 0x04002F68 RID: 12136
	public GameObject MaterialRowPrefab;

	// Token: 0x04002F69 RID: 12137
	public GameObject ItemRowPrefab;

	// Token: 0x04002F6A RID: 12138
	public Sprite IconBackground_Wood;

	// Token: 0x04002F6B RID: 12139
	public Sprite IconBackGround_Input;

	// Token: 0x04002F6C RID: 12140
	public LootGrid LootGrid_Wood;

	// Token: 0x04002F6D RID: 12141
	public LootGrid LootGrid_Input;

	// Token: 0x04002F6E RID: 12142
	public LootGrid LootGrid_Output;

	// Token: 0x04002F6F RID: 12143
	public GameObject Contents;

	// Token: 0x04002F70 RID: 12144
	public GameObject[] ElectricDisableRoots = new GameObject[0];
}

using System;
using Rust;
using TMPro;
using UnityEngine;

// Token: 0x02000849 RID: 2121
public class ProtectionValue : MonoBehaviour, IClothingChanged
{
	// Token: 0x04002F84 RID: 12164
	public CanvasGroup group;

	// Token: 0x04002F85 RID: 12165
	public TextMeshProUGUI text;

	// Token: 0x04002F86 RID: 12166
	public DamageType damageType;

	// Token: 0x04002F87 RID: 12167
	public bool selectedItem;

	// Token: 0x04002F88 RID: 12168
	public bool displayBaseProtection;
}

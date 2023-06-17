using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200079F RID: 1951
public class CopyText : MonoBehaviour
{
	// Token: 0x04002B4E RID: 11086
	public RustText TargetText;

	// Token: 0x06003503 RID: 13571 RVA: 0x001464CB File Offset: 0x001446CB
	public void TriggerCopy()
	{
		if (this.TargetText != null)
		{
			GUIUtility.systemCopyBuffer = this.TargetText.text;
		}
	}
}

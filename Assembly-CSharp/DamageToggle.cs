using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007A4 RID: 1956
[RequireComponent(typeof(Toggle))]
public class DamageToggle : MonoBehaviour
{
	// Token: 0x04002B5C RID: 11100
	public Toggle toggle;

	// Token: 0x06003513 RID: 13587 RVA: 0x001465F4 File Offset: 0x001447F4
	private void Reset()
	{
		this.toggle = base.GetComponent<Toggle>();
	}
}

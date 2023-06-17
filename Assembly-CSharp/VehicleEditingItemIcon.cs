using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000859 RID: 2137
[RequireComponent(typeof(ItemIcon))]
public class VehicleEditingItemIcon : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04002FE1 RID: 12257
	[SerializeField]
	private Image foregroundImage;

	// Token: 0x04002FE2 RID: 12258
	[SerializeField]
	private Image linkImage;
}

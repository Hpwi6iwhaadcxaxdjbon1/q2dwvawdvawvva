using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C4 RID: 708
public class IconSkinPicker : MonoBehaviour
{
	// Token: 0x04001660 RID: 5728
	public GameObjectRef pickerIcon;

	// Token: 0x04001661 RID: 5729
	public GameObject container;

	// Token: 0x04001662 RID: 5730
	public Action skinChangedEvent;

	// Token: 0x04001663 RID: 5731
	public ScrollRect scroller;

	// Token: 0x04001664 RID: 5732
	public SearchFilterInput searchFilter;
}

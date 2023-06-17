using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007F9 RID: 2041
public class PointOfInterestMapMarker : MonoBehaviour
{
	// Token: 0x04002DC4 RID: 11716
	public Image MapIcon;

	// Token: 0x04002DC5 RID: 11717
	public Image MapIconOuter;

	// Token: 0x04002DC6 RID: 11718
	public GameObject LeaderRoot;

	// Token: 0x04002DC7 RID: 11719
	public GameObject EditPopup;

	// Token: 0x04002DC8 RID: 11720
	public Tooltip Tooltip;

	// Token: 0x04002DC9 RID: 11721
	public GameObject MarkerLabelRoot;

	// Token: 0x04002DCA RID: 11722
	public RustText MarkerLabel;

	// Token: 0x04002DCB RID: 11723
	public RustText NoMarkerLabel;

	// Token: 0x04002DCC RID: 11724
	public RustInput MarkerLabelModify;

	// Token: 0x04002DCD RID: 11725
	public MapMarkerIconSelector[] IconSelectors;

	// Token: 0x04002DCE RID: 11726
	public MapMarkerIconSelector[] ColourSelectors;

	// Token: 0x04002DCF RID: 11727
	public bool IsListWidget;

	// Token: 0x04002DD0 RID: 11728
	public GameObject DeleteButton;
}

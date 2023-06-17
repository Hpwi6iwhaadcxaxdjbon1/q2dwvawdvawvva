using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000806 RID: 2054
public class UICompass : MonoBehaviour
{
	// Token: 0x04002E0E RID: 11790
	public RawImage compassStrip;

	// Token: 0x04002E0F RID: 11791
	public CanvasGroup compassGroup;

	// Token: 0x04002E10 RID: 11792
	public List<CompassMapMarker> CompassMarkers;

	// Token: 0x04002E11 RID: 11793
	public List<CompassMapMarker> TeamCompassMarkers;

	// Token: 0x04002E12 RID: 11794
	public List<CompassMissionMarker> MissionMarkers;

	// Token: 0x04002E13 RID: 11795
	public List<CompassMapMarker> LocalPings;

	// Token: 0x04002E14 RID: 11796
	public List<CompassMapMarker> TeamPings;

	// Token: 0x04002E15 RID: 11797
	public Image LeftPingPulse;

	// Token: 0x04002E16 RID: 11798
	public Image RightPingPulse;
}

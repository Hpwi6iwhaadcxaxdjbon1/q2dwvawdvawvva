using System;
using UnityEngine;

// Token: 0x020008A9 RID: 2217
public class uiPlayerPreview : SingletonComponent<uiPlayerPreview>
{
	// Token: 0x040031CD RID: 12749
	public Camera previewCamera;

	// Token: 0x040031CE RID: 12750
	public PlayerModel playermodel;

	// Token: 0x040031CF RID: 12751
	public ReflectionProbe reflectionProbe;

	// Token: 0x040031D0 RID: 12752
	public SegmentMaskPositioning segmentMask;
}

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x020007DD RID: 2013
public class UIVideoPlayer : UIDialog
{
	// Token: 0x04002D09 RID: 11529
	public AspectRatioFitter aspectRatioFitter;

	// Token: 0x04002D0A RID: 11530
	public GameObject closeButton;

	// Token: 0x04002D0B RID: 11531
	public VideoPlayer videoPlayer;

	// Token: 0x04002D0C RID: 11532
	public RawImage videoCanvas;

	// Token: 0x04002D0D RID: 11533
	public RectTransform videoProgressBar;

	// Token: 0x04002D0E RID: 11534
	public GameObject loadingIndicator;

	// Token: 0x04002D0F RID: 11535
	public float audioDuckingAmount = 0.333f;

	// Token: 0x04002D10 RID: 11536
	public float timeoutAfter = 5f;
}

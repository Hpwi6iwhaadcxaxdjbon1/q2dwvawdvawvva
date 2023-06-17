using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200080A RID: 2058
public class UIRecordingInfo : SingletonComponent<UIRecordingInfo>
{
	// Token: 0x04002E2B RID: 11819
	public RustText CountdownText;

	// Token: 0x04002E2C RID: 11820
	public Slider TapeProgressSlider;

	// Token: 0x04002E2D RID: 11821
	public GameObject CountdownRoot;

	// Token: 0x04002E2E RID: 11822
	public GameObject RecordingRoot;

	// Token: 0x04002E2F RID: 11823
	public Transform Spinner;

	// Token: 0x04002E30 RID: 11824
	public float SpinSpeed = 180f;

	// Token: 0x04002E31 RID: 11825
	public Image CassetteImage;

	// Token: 0x060035C4 RID: 13764 RVA: 0x001462B2 File Offset: 0x001444B2
	private void Start()
	{
		base.gameObject.SetActive(false);
	}
}

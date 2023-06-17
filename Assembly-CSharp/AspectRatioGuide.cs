using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000793 RID: 1939
public class AspectRatioGuide : MonoBehaviour
{
	// Token: 0x04002B1D RID: 11037
	public AspectRatioFitter aspectRatioFitter;

	// Token: 0x04002B1E RID: 11038
	public RustText label;

	// Token: 0x04002B1F RID: 11039
	public float aspect;

	// Token: 0x04002B20 RID: 11040
	public float ratio;

	// Token: 0x060034F0 RID: 13552 RVA: 0x001462D0 File Offset: 0x001444D0
	private void Populate()
	{
		this.aspect = CameraMan.GuideAspect;
		this.ratio = Mathf.Max(CameraMan.GuideRatio, 1f);
		this.aspectRatioFitter.aspectRatio = this.aspect / this.ratio;
		this.label.text = string.Format("{0}:{1}", this.aspect, this.ratio);
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x00146340 File Offset: 0x00144540
	public void Awake()
	{
		this.Populate();
	}

	// Token: 0x060034F2 RID: 13554 RVA: 0x00146340 File Offset: 0x00144540
	public void Update()
	{
		this.Populate();
	}
}

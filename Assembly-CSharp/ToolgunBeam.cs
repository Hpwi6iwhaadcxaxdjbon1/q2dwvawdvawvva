using System;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class ToolgunBeam : MonoBehaviour
{
	// Token: 0x0400120D RID: 4621
	public LineRenderer electricalBeam;

	// Token: 0x0400120E RID: 4622
	public float scrollSpeed = -8f;

	// Token: 0x0400120F RID: 4623
	private Color fadeColor = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04001210 RID: 4624
	public float fadeSpeed = 4f;

	// Token: 0x06001924 RID: 6436 RVA: 0x000B9044 File Offset: 0x000B7244
	public void Update()
	{
		if (this.fadeColor.a <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.electricalBeam.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(Time.time * this.scrollSpeed, 0f));
		this.fadeColor.a = this.fadeColor.a - Time.deltaTime * this.fadeSpeed;
		this.electricalBeam.startColor = this.fadeColor;
		this.electricalBeam.endColor = this.fadeColor;
	}
}

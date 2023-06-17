using System;
using UnityEngine;

// Token: 0x02000990 RID: 2448
public class ExplosionsFPS : MonoBehaviour
{
	// Token: 0x0400348E RID: 13454
	private readonly GUIStyle guiStyleHeader = new GUIStyle();

	// Token: 0x0400348F RID: 13455
	private float timeleft;

	// Token: 0x04003490 RID: 13456
	private float fps;

	// Token: 0x04003491 RID: 13457
	private int frames;

	// Token: 0x06003A47 RID: 14919 RVA: 0x00158F4D File Offset: 0x0015714D
	private void Awake()
	{
		this.guiStyleHeader.fontSize = 14;
		this.guiStyleHeader.normal.textColor = new Color(1f, 1f, 1f);
	}

	// Token: 0x06003A48 RID: 14920 RVA: 0x00158F80 File Offset: 0x00157180
	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 30f, 30f), "FPS: " + (int)this.fps, this.guiStyleHeader);
	}

	// Token: 0x06003A49 RID: 14921 RVA: 0x00158FBC File Offset: 0x001571BC
	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0.0)
		{
			this.fps = (float)this.frames;
			this.timeleft = 1f;
			this.frames = 0;
		}
	}
}

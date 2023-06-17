using System;
using ConVar;
using UnityEngine;

// Token: 0x020002A3 RID: 675
public class CameraSettings : MonoBehaviour, IClientComponent
{
	// Token: 0x04001617 RID: 5655
	private Camera cam;

	// Token: 0x06001D2D RID: 7469 RVA: 0x000C9460 File Offset: 0x000C7660
	private void OnEnable()
	{
		this.cam = base.GetComponent<Camera>();
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x000C946E File Offset: 0x000C766E
	private void Update()
	{
		this.cam.farClipPlane = Mathf.Clamp(ConVar.Graphics.drawdistance, 500f, 2500f);
	}
}

using System;
using UnityEngine;

// Token: 0x02000508 RID: 1288
public class EnvironmentVolumeTrigger : MonoBehaviour
{
	// Token: 0x04002134 RID: 8500
	[HideInInspector]
	public Vector3 Center = Vector3.zero;

	// Token: 0x04002135 RID: 8501
	[HideInInspector]
	public Vector3 Size = Vector3.one;

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x0600293E RID: 10558 RVA: 0x000FD282 File Offset: 0x000FB482
	// (set) Token: 0x0600293F RID: 10559 RVA: 0x000FD28A File Offset: 0x000FB48A
	public EnvironmentVolume volume { get; private set; }

	// Token: 0x06002940 RID: 10560 RVA: 0x000FD294 File Offset: 0x000FB494
	protected void Awake()
	{
		this.volume = base.gameObject.GetComponent<EnvironmentVolume>();
		if (this.volume == null)
		{
			this.volume = base.gameObject.AddComponent<EnvironmentVolume>();
			this.volume.Center = this.Center;
			this.volume.Size = this.Size;
			this.volume.UpdateTrigger();
		}
	}
}

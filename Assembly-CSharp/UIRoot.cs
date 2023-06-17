using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008D0 RID: 2256
public abstract class UIRoot : MonoBehaviour
{
	// Token: 0x0400327A RID: 12922
	private GraphicRaycaster[] graphicRaycasters;

	// Token: 0x0400327B RID: 12923
	public Canvas overlayCanvas;

	// Token: 0x06003770 RID: 14192 RVA: 0x0014D850 File Offset: 0x0014BA50
	private void ToggleRaycasters(bool state)
	{
		for (int i = 0; i < this.graphicRaycasters.Length; i++)
		{
			GraphicRaycaster graphicRaycaster = this.graphicRaycasters[i];
			if (graphicRaycaster.enabled != state)
			{
				graphicRaycaster.enabled = state;
			}
		}
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x000063A5 File Offset: 0x000045A5
	protected virtual void Awake()
	{
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x0014D889 File Offset: 0x0014BA89
	protected virtual void Start()
	{
		this.graphicRaycasters = base.GetComponentsInChildren<GraphicRaycaster>(true);
	}

	// Token: 0x06003773 RID: 14195 RVA: 0x0014D898 File Offset: 0x0014BA98
	protected void Update()
	{
		this.Refresh();
	}

	// Token: 0x06003774 RID: 14196
	protected abstract void Refresh();
}

using System;
using ConVar;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008D2 RID: 2258
public class UIRootScaled : UIRoot
{
	// Token: 0x0400327C RID: 12924
	private static UIRootScaled Instance;

	// Token: 0x0400327D RID: 12925
	public bool OverrideReference;

	// Token: 0x0400327E RID: 12926
	public Vector2 TargetReference = new Vector2(1280f, 720f);

	// Token: 0x0400327F RID: 12927
	public CanvasScaler scaler;

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x06003778 RID: 14200 RVA: 0x0014D8A8 File Offset: 0x0014BAA8
	public static Canvas DragOverlayCanvas
	{
		get
		{
			return UIRootScaled.Instance.overlayCanvas;
		}
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x0014D8B4 File Offset: 0x0014BAB4
	protected override void Awake()
	{
		UIRootScaled.Instance = this;
		base.Awake();
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x0014D8C4 File Offset: 0x0014BAC4
	protected override void Refresh()
	{
		Vector2 vector = new Vector2(1280f / ConVar.Graphics.uiscale, 720f / ConVar.Graphics.uiscale);
		if (this.OverrideReference)
		{
			vector = new Vector2(this.TargetReference.x / ConVar.Graphics.uiscale, this.TargetReference.y / ConVar.Graphics.uiscale);
		}
		if (this.scaler.referenceResolution != vector)
		{
			this.scaler.referenceResolution = vector;
		}
	}
}

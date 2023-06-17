using System;
using Rust.UI;
using UnityEngine.EventSystems;

// Token: 0x020007AE RID: 1966
public class DemoShotButton : RustButton, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x04002B70 RID: 11120
	public bool FireEventOnClicked;

	// Token: 0x06003522 RID: 13602 RVA: 0x00146622 File Offset: 0x00144822
	public override void OnPointerDown(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			return;
		}
		base.OnPointerDown(eventData);
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x00146634 File Offset: 0x00144834
	public override void OnPointerUp(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			return;
		}
		base.OnPointerUp(eventData);
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x00146646 File Offset: 0x00144846
	public void OnPointerClick(PointerEventData eventData)
	{
		if (this.FireEventOnClicked)
		{
			base.Press();
		}
	}
}

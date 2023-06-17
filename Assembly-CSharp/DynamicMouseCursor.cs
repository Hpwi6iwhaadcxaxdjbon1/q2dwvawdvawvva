using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x020007DE RID: 2014
public class DynamicMouseCursor : MonoBehaviour
{
	// Token: 0x04002D11 RID: 11537
	public Texture2D RegularCursor;

	// Token: 0x04002D12 RID: 11538
	public Vector2 RegularCursorPos;

	// Token: 0x04002D13 RID: 11539
	public Texture2D HoverCursor;

	// Token: 0x04002D14 RID: 11540
	public Vector2 HoverCursorPos;

	// Token: 0x04002D15 RID: 11541
	private Texture2D current;

	// Token: 0x0600355C RID: 13660 RVA: 0x00146964 File Offset: 0x00144B64
	private void LateUpdate()
	{
		if (!Cursor.visible)
		{
			return;
		}
		GameObject gameObject = this.CurrentlyHoveredItem();
		if (gameObject != null)
		{
			using (TimeWarning.New("RustControl", 0))
			{
				RustControl componentInParent = gameObject.GetComponentInParent<RustControl>();
				if (componentInParent != null && componentInParent.IsDisabled)
				{
					this.UpdateCursor(this.RegularCursor, this.RegularCursorPos);
					return;
				}
			}
			using (TimeWarning.New("ISubmitHandler", 0))
			{
				if (gameObject.GetComponentInParent<ISubmitHandler>() != null)
				{
					this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
					return;
				}
			}
			using (TimeWarning.New("IPointerDownHandler", 0))
			{
				if (gameObject.GetComponentInParent<IPointerDownHandler>() != null)
				{
					this.UpdateCursor(this.HoverCursor, this.HoverCursorPos);
					return;
				}
			}
		}
		using (TimeWarning.New("UpdateCursor", 0))
		{
			this.UpdateCursor(this.RegularCursor, this.RegularCursorPos);
		}
	}

	// Token: 0x0600355D RID: 13661 RVA: 0x00146A9C File Offset: 0x00144C9C
	private void UpdateCursor(Texture2D cursor, Vector2 offs)
	{
		if (this.current == cursor)
		{
			return;
		}
		this.current = cursor;
		Cursor.SetCursor(cursor, offs, CursorMode.Auto);
	}

	// Token: 0x0600355E RID: 13662 RVA: 0x00146ABC File Offset: 0x00144CBC
	private GameObject CurrentlyHoveredItem()
	{
		FpStandaloneInputModule fpStandaloneInputModule = EventSystem.current.currentInputModule as FpStandaloneInputModule;
		if (fpStandaloneInputModule == null)
		{
			return null;
		}
		return fpStandaloneInputModule.CurrentData.pointerCurrentRaycast.gameObject;
	}
}

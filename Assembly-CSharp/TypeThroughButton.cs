using System;
using System.Collections;
using Rust;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008C2 RID: 2242
public class TypeThroughButton : Button, IUpdateSelectedHandler, IEventSystemHandler
{
	// Token: 0x04003249 RID: 12873
	public InputField typingTarget;

	// Token: 0x0400324A RID: 12874
	private Event _processingEvent = new Event();

	// Token: 0x0600374D RID: 14157 RVA: 0x0014D088 File Offset: 0x0014B288
	public void OnUpdateSelected(BaseEventData eventData)
	{
		if (this.typingTarget == null)
		{
			return;
		}
		while (Event.PopEvent(this._processingEvent))
		{
			if (this._processingEvent.rawType == EventType.KeyDown && this._processingEvent.character != '\0')
			{
				Event e = new Event(this._processingEvent);
				Global.Runner.StartCoroutine(this.DelayedActivateTextField(e));
				break;
			}
		}
		eventData.Use();
	}

	// Token: 0x0600374E RID: 14158 RVA: 0x0014D0F2 File Offset: 0x0014B2F2
	private IEnumerator DelayedActivateTextField(Event e)
	{
		this.typingTarget.ActivateInputField();
		this.typingTarget.Select();
		if (e.character != ' ')
		{
			InputField inputField = this.typingTarget;
			inputField.text += " ";
		}
		this.typingTarget.MoveTextEnd(false);
		this.typingTarget.ProcessEvent(e);
		yield return null;
		this.typingTarget.caretPosition = this.typingTarget.text.Length;
		this.typingTarget.ForceLabelUpdate();
		yield break;
	}
}

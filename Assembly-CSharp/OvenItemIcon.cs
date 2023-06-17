using System;
using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using UnityEngine;

// Token: 0x02000842 RID: 2114
public class OvenItemIcon : MonoBehaviour
{
	// Token: 0x04002F45 RID: 12101
	public ItemIcon ItemIcon;

	// Token: 0x04002F46 RID: 12102
	public RustText ItemLabel;

	// Token: 0x04002F47 RID: 12103
	public RustText MaterialLabel;

	// Token: 0x04002F48 RID: 12104
	public OvenSlotType SlotType;

	// Token: 0x04002F49 RID: 12105
	public Translate.Phrase EmptyPhrase = new Translate.Phrase("empty", "empty");

	// Token: 0x04002F4A RID: 12106
	public List<OvenItemIcon.OvenSlotConfig> SlotConfigs = new List<OvenItemIcon.OvenSlotConfig>();

	// Token: 0x04002F4B RID: 12107
	public float DisabledAlphaScale;

	// Token: 0x04002F4C RID: 12108
	public CanvasGroup CanvasGroup;

	// Token: 0x04002F4D RID: 12109
	private Item _item;

	// Token: 0x0600360B RID: 13835 RVA: 0x001487B0 File Offset: 0x001469B0
	private void Start()
	{
		OvenItemIcon.OvenSlotConfig ovenSlotConfig = this.SlotConfigs.FirstOrDefault((OvenItemIcon.OvenSlotConfig x) => x.Type == this.SlotType);
		if (ovenSlotConfig == null)
		{
			Debug.LogError(string.Format("Can't find slot config for '{0}'", this.SlotType));
			return;
		}
		this.ItemIcon.emptySlotBackgroundSprite = ovenSlotConfig.BackgroundImage;
		this.MaterialLabel.SetPhrase(ovenSlotConfig.SlotPhrase);
		this.UpdateLabels();
	}

	// Token: 0x0600360C RID: 13836 RVA: 0x0014881B File Offset: 0x00146A1B
	private void Update()
	{
		if (this.ItemIcon.item == this._item)
		{
			return;
		}
		this._item = this.ItemIcon.item;
		this.UpdateLabels();
	}

	// Token: 0x0600360D RID: 13837 RVA: 0x00148848 File Offset: 0x00146A48
	private void UpdateLabels()
	{
		this.CanvasGroup.alpha = ((this._item != null) ? 1f : this.DisabledAlphaScale);
		RustText itemLabel = this.ItemLabel;
		if (itemLabel == null)
		{
			return;
		}
		itemLabel.SetPhrase((this._item == null) ? this.EmptyPhrase : this._item.info.displayName);
	}

	// Token: 0x02000E86 RID: 3718
	[Serializable]
	public class OvenSlotConfig
	{
		// Token: 0x04004BEF RID: 19439
		public OvenSlotType Type;

		// Token: 0x04004BF0 RID: 19440
		public Sprite BackgroundImage;

		// Token: 0x04004BF1 RID: 19441
		public Translate.Phrase SlotPhrase;
	}
}

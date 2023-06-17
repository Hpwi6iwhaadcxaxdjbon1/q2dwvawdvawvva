using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000171 RID: 369
public class UIMarketTerminal : UIDialog, IVendingMachineInterface
{
	// Token: 0x0400103A RID: 4154
	public static readonly Translate.Phrase PendingDeliveryPluralPhrase = new Translate.Phrase("market.pending_delivery.plural", "Waiting for {n} deliveries...");

	// Token: 0x0400103B RID: 4155
	public static readonly Translate.Phrase PendingDeliverySingularPhrase = new Translate.Phrase("market.pending_delivery.singular", "Waiting for delivery...");

	// Token: 0x0400103C RID: 4156
	public Canvas canvas;

	// Token: 0x0400103D RID: 4157
	public MapView mapView;

	// Token: 0x0400103E RID: 4158
	public RectTransform shopDetailsPanel;

	// Token: 0x0400103F RID: 4159
	public float shopDetailsMargin = 16f;

	// Token: 0x04001040 RID: 4160
	public float easeDuration = 0.2f;

	// Token: 0x04001041 RID: 4161
	public LeanTweenType easeType = LeanTweenType.linear;

	// Token: 0x04001042 RID: 4162
	public RustText shopName;

	// Token: 0x04001043 RID: 4163
	public GameObject shopOrderingPanel;

	// Token: 0x04001044 RID: 4164
	public RectTransform sellOrderContainer;

	// Token: 0x04001045 RID: 4165
	public GameObjectRef sellOrderPrefab;

	// Token: 0x04001046 RID: 4166
	public VirtualItemIcon deliveryFeeIcon;

	// Token: 0x04001047 RID: 4167
	public GameObject deliveryFeeCantAffordIndicator;

	// Token: 0x04001048 RID: 4168
	public GameObject inventoryFullIndicator;

	// Token: 0x04001049 RID: 4169
	public GameObject notEligiblePanel;

	// Token: 0x0400104A RID: 4170
	public GameObject pendingDeliveryPanel;

	// Token: 0x0400104B RID: 4171
	public RustText pendingDeliveryLabel;

	// Token: 0x0400104C RID: 4172
	public RectTransform itemNoticesContainer;

	// Token: 0x0400104D RID: 4173
	public GameObjectRef itemRemovedPrefab;

	// Token: 0x0400104E RID: 4174
	public GameObjectRef itemPendingPrefab;

	// Token: 0x0400104F RID: 4175
	public GameObjectRef itemAddedPrefab;

	// Token: 0x04001050 RID: 4176
	public CanvasGroup gettingStartedTip;

	// Token: 0x04001051 RID: 4177
	public SoundDefinition buyItemSoundDef;

	// Token: 0x04001052 RID: 4178
	public SoundDefinition buttonPressSoundDef;
}

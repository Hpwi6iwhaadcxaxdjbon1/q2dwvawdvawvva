using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085A RID: 2138
public class VehicleEditingPanel : LootPanel
{
	// Token: 0x04002FE3 RID: 12259
	[SerializeField]
	[Range(0f, 1f)]
	private float disabledAlpha = 0.25f;

	// Token: 0x04002FE4 RID: 12260
	[Header("Edit Vehicle")]
	[SerializeField]
	private CanvasGroup editGroup;

	// Token: 0x04002FE5 RID: 12261
	[SerializeField]
	private GameObject moduleInternalItemsGroup;

	// Token: 0x04002FE6 RID: 12262
	[SerializeField]
	private GameObject moduleInternalLiquidsGroup;

	// Token: 0x04002FE7 RID: 12263
	[SerializeField]
	private GameObject destroyChassisGroup;

	// Token: 0x04002FE8 RID: 12264
	[SerializeField]
	private Button itemTakeButton;

	// Token: 0x04002FE9 RID: 12265
	[SerializeField]
	private Button liquidTakeButton;

	// Token: 0x04002FEA RID: 12266
	[SerializeField]
	private GameObject liquidHelp;

	// Token: 0x04002FEB RID: 12267
	[SerializeField]
	private GameObject liquidButton;

	// Token: 0x04002FEC RID: 12268
	[SerializeField]
	private Color gotColor;

	// Token: 0x04002FED RID: 12269
	[SerializeField]
	private Color notGotColor;

	// Token: 0x04002FEE RID: 12270
	[SerializeField]
	private Text generalInfoText;

	// Token: 0x04002FEF RID: 12271
	[SerializeField]
	private Text generalWarningText;

	// Token: 0x04002FF0 RID: 12272
	[SerializeField]
	private Image generalWarningImage;

	// Token: 0x04002FF1 RID: 12273
	[SerializeField]
	private Text repairInfoText;

	// Token: 0x04002FF2 RID: 12274
	[SerializeField]
	private Button repairButton;

	// Token: 0x04002FF3 RID: 12275
	[SerializeField]
	private Text destroyChassisButtonText;

	// Token: 0x04002FF4 RID: 12276
	[SerializeField]
	private Text destroyChassisCountdown;

	// Token: 0x04002FF5 RID: 12277
	[SerializeField]
	private Translate.Phrase phraseEditingInfo;

	// Token: 0x04002FF6 RID: 12278
	[SerializeField]
	private Translate.Phrase phraseNoOccupant;

	// Token: 0x04002FF7 RID: 12279
	[SerializeField]
	private Translate.Phrase phraseBadOccupant;

	// Token: 0x04002FF8 RID: 12280
	[SerializeField]
	private Translate.Phrase phrasePlayerObstructing;

	// Token: 0x04002FF9 RID: 12281
	[SerializeField]
	private Translate.Phrase phraseNotDriveable;

	// Token: 0x04002FFA RID: 12282
	[SerializeField]
	private Translate.Phrase phraseNotRepairable;

	// Token: 0x04002FFB RID: 12283
	[SerializeField]
	private Translate.Phrase phraseRepairNotNeeded;

	// Token: 0x04002FFC RID: 12284
	[SerializeField]
	private Translate.Phrase phraseRepairSelectInfo;

	// Token: 0x04002FFD RID: 12285
	[SerializeField]
	private Translate.Phrase phraseRepairEnactInfo;

	// Token: 0x04002FFE RID: 12286
	[SerializeField]
	private Translate.Phrase phraseHasLock;

	// Token: 0x04002FFF RID: 12287
	[SerializeField]
	private Translate.Phrase phraseHasNoLock;

	// Token: 0x04003000 RID: 12288
	[SerializeField]
	private Translate.Phrase phraseAddLock;

	// Token: 0x04003001 RID: 12289
	[SerializeField]
	private Translate.Phrase phraseAddLockButton;

	// Token: 0x04003002 RID: 12290
	[SerializeField]
	private Translate.Phrase phraseChangeLockCodeButton;

	// Token: 0x04003003 RID: 12291
	[SerializeField]
	private Text carLockInfoText;

	// Token: 0x04003004 RID: 12292
	[SerializeField]
	private RustText carLockButtonText;

	// Token: 0x04003005 RID: 12293
	[SerializeField]
	private Button actionLockButton;

	// Token: 0x04003006 RID: 12294
	[SerializeField]
	private Button removeLockButton;

	// Token: 0x04003007 RID: 12295
	[SerializeField]
	private GameObjectRef keyEnterDialog;

	// Token: 0x04003008 RID: 12296
	[SerializeField]
	private Translate.Phrase phraseEmptyStorage;

	// Token: 0x04003009 RID: 12297
	[Header("Create Chassis")]
	[SerializeField]
	private VehicleEditingPanel.CreateChassisEntry[] chassisOptions;

	// Token: 0x02000E88 RID: 3720
	[Serializable]
	private class CreateChassisEntry
	{
		// Token: 0x04004BF4 RID: 19444
		public byte garageChassisIndex;

		// Token: 0x04004BF5 RID: 19445
		public Button craftButton;

		// Token: 0x04004BF6 RID: 19446
		public Text craftButtonText;

		// Token: 0x04004BF7 RID: 19447
		public Text requirementsText;

		// Token: 0x060052CA RID: 21194 RVA: 0x001B1065 File Offset: 0x001AF265
		public ItemDefinition GetChassisItemDef(ModularCarGarage garage)
		{
			return garage.chassisBuildOptions[(int)this.garageChassisIndex].itemDef;
		}
	}
}

using System;
using UnityEngine;

// Token: 0x02000809 RID: 2057
public class UIHUD : SingletonComponent<UIHUD>, IUIScreen
{
	// Token: 0x04002E1F RID: 11807
	public UIChat chatPanel;

	// Token: 0x04002E20 RID: 11808
	public HudElement Hunger;

	// Token: 0x04002E21 RID: 11809
	public HudElement Thirst;

	// Token: 0x04002E22 RID: 11810
	public HudElement Health;

	// Token: 0x04002E23 RID: 11811
	public HudElement PendingHealth;

	// Token: 0x04002E24 RID: 11812
	public HudElement VehicleHealth;

	// Token: 0x04002E25 RID: 11813
	public HudElement AnimalStamina;

	// Token: 0x04002E26 RID: 11814
	public HudElement AnimalStaminaMax;

	// Token: 0x04002E27 RID: 11815
	public RectTransform vitalsRect;

	// Token: 0x04002E28 RID: 11816
	public Canvas healthCanvas;

	// Token: 0x04002E29 RID: 11817
	public UICompass CompassWidget;

	// Token: 0x04002E2A RID: 11818
	public GameObject KeyboardCaptureMode;
}

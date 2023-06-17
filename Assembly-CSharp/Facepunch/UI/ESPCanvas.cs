using System;
using UnityEngine;

namespace Facepunch.UI
{
	// Token: 0x02000AF0 RID: 2800
	public class ESPCanvas : SingletonComponent<ESPCanvas>
	{
		// Token: 0x04003C74 RID: 15476
		[Tooltip("Amount of times per second we should update the visible panels")]
		public float RefreshRate = 5f;

		// Token: 0x04003C75 RID: 15477
		[Tooltip("This object will be duplicated in place")]
		public ESPPlayerInfo Source;

		// Token: 0x04003C76 RID: 15478
		[Tooltip("Entities this far away won't be overlayed")]
		public float MaxDistance = 64f;

		// Token: 0x04003C77 RID: 15479
		private static int NameplateCount = 32;

		// Token: 0x04003C78 RID: 15480
		[ClientVar(ClientAdmin = true)]
		public static float OverrideMaxDisplayDistance = 0f;

		// Token: 0x04003C79 RID: 15481
		[ClientVar(ClientAdmin = true)]
		public static bool DisableOcclusionChecks = false;

		// Token: 0x04003C7A RID: 15482
		[ClientVar(ClientAdmin = true)]
		public static bool ShowHealth = false;

		// Token: 0x04003C7B RID: 15483
		[ClientVar(ClientAdmin = true)]
		public static bool ColourCodeTeams = false;

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x060043A4 RID: 17316 RVA: 0x0018F37C File Offset: 0x0018D57C
		// (set) Token: 0x060043A5 RID: 17317 RVA: 0x0018F383 File Offset: 0x0018D583
		[ClientVar(ClientAdmin = true, Help = "Max amount of nameplates to show at once")]
		public static int MaxNameplates
		{
			get
			{
				return ESPCanvas.NameplateCount;
			}
			set
			{
				ESPCanvas.NameplateCount = Mathf.Clamp(value, 16, 150);
			}
		}
	}
}

using System;
using TMPro;
using UnityEngine;

namespace Facepunch.UI
{
	// Token: 0x02000AF1 RID: 2801
	public class ESPPlayerInfo : MonoBehaviour
	{
		// Token: 0x04003C7C RID: 15484
		public Vector3 WorldOffset;

		// Token: 0x04003C7D RID: 15485
		public TextMeshProUGUI Text;

		// Token: 0x04003C7E RID: 15486
		public TextMeshProUGUI Image;

		// Token: 0x04003C7F RID: 15487
		public CanvasGroup group;

		// Token: 0x04003C80 RID: 15488
		public Gradient gradientNormal;

		// Token: 0x04003C81 RID: 15489
		public Gradient gradientTeam;

		// Token: 0x04003C82 RID: 15490
		public Color TeamColor;

		// Token: 0x04003C83 RID: 15491
		public Color AllyColor = Color.blue;

		// Token: 0x04003C84 RID: 15492
		public Color EnemyColor;

		// Token: 0x04003C85 RID: 15493
		public QueryVis visCheck;

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x060043A8 RID: 17320 RVA: 0x0018F3DA File Offset: 0x0018D5DA
		// (set) Token: 0x060043A9 RID: 17321 RVA: 0x0018F3E2 File Offset: 0x0018D5E2
		public BasePlayer Entity { get; set; }
	}
}

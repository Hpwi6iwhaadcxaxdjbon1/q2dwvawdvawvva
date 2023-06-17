using System;
using UnityEngine;

namespace ConVar
{
	// Token: 0x02000ABA RID: 2746
	[ConsoleSystem.Factory("fps")]
	public class FPS : ConsoleSystem
	{
		// Token: 0x04003B3A RID: 15162
		private static int _limit = 240;

		// Token: 0x04003B3B RID: 15163
		private static int m_graph;

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x060041D3 RID: 16851 RVA: 0x001868C1 File Offset: 0x00184AC1
		// (set) Token: 0x060041D4 RID: 16852 RVA: 0x001868DA File Offset: 0x00184ADA
		[ClientVar(Saved = true)]
		[ServerVar(Saved = true)]
		public static int limit
		{
			get
			{
				if (FPS._limit == -1)
				{
					FPS._limit = Application.targetFrameRate;
				}
				return FPS._limit;
			}
			set
			{
				FPS._limit = value;
				Application.targetFrameRate = FPS._limit;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x060041D5 RID: 16853 RVA: 0x001868EC File Offset: 0x00184AEC
		// (set) Token: 0x060041D6 RID: 16854 RVA: 0x001868F4 File Offset: 0x00184AF4
		[ClientVar]
		public static int graph
		{
			get
			{
				return FPS.m_graph;
			}
			set
			{
				FPS.m_graph = value;
				if (!MainCamera.mainCamera)
				{
					return;
				}
				FPSGraph component = MainCamera.mainCamera.GetComponent<FPSGraph>();
				if (!component)
				{
					return;
				}
				component.Refresh();
			}
		}
	}
}

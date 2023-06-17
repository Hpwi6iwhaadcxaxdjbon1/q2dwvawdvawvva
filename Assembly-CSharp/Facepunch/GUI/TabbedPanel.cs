using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.GUI
{
	// Token: 0x02000AFD RID: 2813
	internal class TabbedPanel
	{
		// Token: 0x04003CD2 RID: 15570
		private int selectedTabID;

		// Token: 0x04003CD3 RID: 15571
		private List<TabbedPanel.Tab> tabs = new List<TabbedPanel.Tab>();

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x060044AB RID: 17579 RVA: 0x00193305 File Offset: 0x00191505
		public TabbedPanel.Tab selectedTab
		{
			get
			{
				return this.tabs[this.selectedTabID];
			}
		}

		// Token: 0x060044AC RID: 17580 RVA: 0x00193318 File Offset: 0x00191518
		public void Add(TabbedPanel.Tab tab)
		{
			this.tabs.Add(tab);
		}

		// Token: 0x060044AD RID: 17581 RVA: 0x00193328 File Offset: 0x00191528
		internal void DrawVertical(float width)
		{
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(width),
				GUILayout.ExpandHeight(true)
			});
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (GUILayout.Toggle(this.selectedTabID == i, this.tabs[i].name, new GUIStyle("devtab"), Array.Empty<GUILayoutOption>()))
				{
					this.selectedTabID = i;
				}
			}
			if (GUILayout.Toggle(false, "", new GUIStyle("devtab"), new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			}))
			{
				this.selectedTabID = -1;
			}
			GUILayout.EndVertical();
		}

		// Token: 0x060044AE RID: 17582 RVA: 0x001933DC File Offset: 0x001915DC
		internal void DrawContents()
		{
			if (this.selectedTabID < 0)
			{
				return;
			}
			TabbedPanel.Tab selectedTab = this.selectedTab;
			GUILayout.BeginVertical(new GUIStyle("devtabcontents"), new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			if (selectedTab.drawFunc != null)
			{
				selectedTab.drawFunc();
			}
			GUILayout.EndVertical();
		}

		// Token: 0x02000F82 RID: 3970
		public struct Tab
		{
			// Token: 0x04004FF8 RID: 20472
			public string name;

			// Token: 0x04004FF9 RID: 20473
			public Action drawFunc;
		}
	}
}

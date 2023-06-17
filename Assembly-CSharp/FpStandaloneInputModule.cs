using System;
using UnityEngine.EventSystems;

// Token: 0x020007E3 RID: 2019
public class FpStandaloneInputModule : StandaloneInputModule
{
	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x06003569 RID: 13673 RVA: 0x00146CC3 File Offset: 0x00144EC3
	public PointerEventData CurrentData
	{
		get
		{
			if (!this.m_PointerData.ContainsKey(-1))
			{
				return new PointerEventData(EventSystem.current);
			}
			return this.m_PointerData[-1];
		}
	}
}

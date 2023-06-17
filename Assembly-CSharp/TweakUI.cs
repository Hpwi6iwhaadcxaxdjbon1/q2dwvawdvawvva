using System;
using UnityEngine;

// Token: 0x02000885 RID: 2181
public class TweakUI : SingletonComponent<TweakUI>
{
	// Token: 0x040030FE RID: 12542
	public static bool isOpen;

	// Token: 0x0600369D RID: 13981 RVA: 0x0014A4D7 File Offset: 0x001486D7
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F2) && this.CanToggle())
		{
			this.SetVisible(!TweakUI.isOpen);
		}
	}

	// Token: 0x0600369E RID: 13982 RVA: 0x0014A4FB File Offset: 0x001486FB
	protected bool CanToggle()
	{
		return LevelManager.isLoaded;
	}

	// Token: 0x0600369F RID: 13983 RVA: 0x0014A507 File Offset: 0x00148707
	public void SetVisible(bool b)
	{
		if (b)
		{
			TweakUI.isOpen = true;
			return;
		}
		TweakUI.isOpen = false;
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "writecfg", Array.Empty<object>());
	}
}

using System;
using UnityEngine;

// Token: 0x020007A1 RID: 1953
public class CursorManager : SingletonComponent<CursorManager>
{
	// Token: 0x04002B58 RID: 11096
	private static int iHoldOpen;

	// Token: 0x04002B59 RID: 11097
	private static int iPreviousOpen;

	// Token: 0x04002B5A RID: 11098
	private static float lastTimeVisible;

	// Token: 0x04002B5B RID: 11099
	private static float lastTimeInvisible;

	// Token: 0x06003507 RID: 13575 RVA: 0x00146510 File Offset: 0x00144710
	private void Update()
	{
		if (SingletonComponent<CursorManager>.Instance != this)
		{
			return;
		}
		if (CursorManager.iHoldOpen == 0 && CursorManager.iPreviousOpen == 0)
		{
			this.SwitchToGame();
		}
		else
		{
			this.SwitchToUI();
		}
		CursorManager.iPreviousOpen = CursorManager.iHoldOpen;
		CursorManager.iHoldOpen = 0;
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x0014654C File Offset: 0x0014474C
	public void SwitchToGame()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		if (Cursor.visible)
		{
			Cursor.visible = false;
		}
		CursorManager.lastTimeInvisible = Time.time;
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x00146573 File Offset: 0x00144773
	private void SwitchToUI()
	{
		if (Cursor.lockState != CursorLockMode.None)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
		CursorManager.lastTimeVisible = Time.time;
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x00146599 File Offset: 0x00144799
	public static void HoldOpen(bool cursorVisible = false)
	{
		CursorManager.iHoldOpen++;
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x001465A7 File Offset: 0x001447A7
	public static bool WasVisible(float deltaTime)
	{
		return Time.time - CursorManager.lastTimeVisible <= deltaTime;
	}

	// Token: 0x0600350C RID: 13580 RVA: 0x001465BA File Offset: 0x001447BA
	public static bool WasInvisible(float deltaTime)
	{
		return Time.time - CursorManager.lastTimeInvisible <= deltaTime;
	}
}

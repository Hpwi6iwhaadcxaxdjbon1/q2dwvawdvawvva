using System;
using UnityEngine;

// Token: 0x020005C0 RID: 1472
public class InputState
{
	// Token: 0x040023F5 RID: 9205
	public InputMessage current = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x040023F6 RID: 9206
	public InputMessage previous = new InputMessage
	{
		ShouldPool = false
	};

	// Token: 0x040023F7 RID: 9207
	private int SwallowedButtons;

	// Token: 0x06002C21 RID: 11297 RVA: 0x0010B49E File Offset: 0x0010969E
	public bool IsDown(BUTTON btn)
	{
		return this.current != null && (this.SwallowedButtons & (int)btn) != (int)btn && (this.current.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x0010B4C7 File Offset: 0x001096C7
	public bool WasDown(BUTTON btn)
	{
		return this.previous != null && (this.previous.buttons & (int)btn) == (int)btn;
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x0010B4E3 File Offset: 0x001096E3
	public bool IsAnyDown()
	{
		return this.current != null && (float)(this.current.buttons & ~(float)this.SwallowedButtons) > 0f;
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x0010B50A File Offset: 0x0010970A
	public bool WasJustPressed(BUTTON btn)
	{
		return this.IsDown(btn) && !this.WasDown(btn);
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x0010B521 File Offset: 0x00109721
	public bool WasJustReleased(BUTTON btn)
	{
		return !this.IsDown(btn) && this.WasDown(btn);
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x0010B535 File Offset: 0x00109735
	public void SwallowButton(BUTTON btn)
	{
		if (this.current == null)
		{
			return;
		}
		this.SwallowedButtons |= (int)btn;
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x0010B54E File Offset: 0x0010974E
	public Quaternion AimAngle()
	{
		if (this.current == null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(this.current.aimAngles);
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x0010B56E File Offset: 0x0010976E
	public Vector3 MouseDelta()
	{
		if (this.current == null)
		{
			return Vector3.zero;
		}
		return this.current.mouseDelta;
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x0010B58C File Offset: 0x0010978C
	public void Flip(InputMessage newcurrent)
	{
		this.SwallowedButtons = 0;
		this.previous.aimAngles = this.current.aimAngles;
		this.previous.buttons = this.current.buttons;
		this.previous.mouseDelta = this.current.mouseDelta;
		this.current.aimAngles = newcurrent.aimAngles;
		this.current.buttons = newcurrent.buttons;
		this.current.mouseDelta = newcurrent.mouseDelta;
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x0010B615 File Offset: 0x00109815
	public void Clear()
	{
		this.current.buttons = 0;
		this.previous.buttons = 0;
		this.current.mouseDelta = Vector3.zero;
		this.SwallowedButtons = 0;
	}
}

using System;
using UnityEngine.UI;

// Token: 0x0200088A RID: 2186
public class TweakUIToggle : TweakUIBase
{
	// Token: 0x04003111 RID: 12561
	public Toggle toggleControl;

	// Token: 0x04003112 RID: 12562
	public bool inverse;

	// Token: 0x04003113 RID: 12563
	public static string lastConVarChanged;

	// Token: 0x04003114 RID: 12564
	public static TimeSince timeSinceLastConVarChange;

	// Token: 0x060036C2 RID: 14018 RVA: 0x0014AA4B File Offset: 0x00148C4B
	protected override void Init()
	{
		base.Init();
		this.ResetToConvar();
	}

	// Token: 0x060036C3 RID: 14019 RVA: 0x0014A5BC File Offset: 0x001487BC
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x0014A708 File Offset: 0x00148908
	public void OnToggleChanged()
	{
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x0014AB84 File Offset: 0x00148D84
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.toggleControl.isOn;
		if (this.inverse)
		{
			flag = !flag;
		}
		if (this.conVar.AsBool == flag)
		{
			return;
		}
		TweakUIToggle.lastConVarChanged = this.conVar.FullName;
		TweakUIToggle.timeSinceLastConVarChange = 0f;
		this.conVar.Set(flag);
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x0014ABF4 File Offset: 0x00148DF4
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		bool flag = this.conVar.AsBool;
		if (this.inverse)
		{
			flag = !flag;
		}
		this.toggleControl.isOn = flag;
	}
}

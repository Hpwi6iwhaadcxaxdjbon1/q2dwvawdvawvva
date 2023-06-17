using System;
using System.Linq;
using UnityEngine.UI;

// Token: 0x02000888 RID: 2184
public class TweakUIMultiSelect : TweakUIBase
{
	// Token: 0x0400310C RID: 12556
	public ToggleGroup toggleGroup;

	// Token: 0x060036B5 RID: 14005 RVA: 0x0014A954 File Offset: 0x00148B54
	protected override void Init()
	{
		base.Init();
		this.UpdateToggleGroup();
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x0014A962 File Offset: 0x00148B62
	protected void OnEnable()
	{
		this.UpdateToggleGroup();
	}

	// Token: 0x060036B7 RID: 14007 RVA: 0x0014A96A File Offset: 0x00148B6A
	public void OnChanged()
	{
		this.UpdateConVar();
	}

	// Token: 0x060036B8 RID: 14008 RVA: 0x0014A974 File Offset: 0x00148B74
	private void UpdateToggleGroup()
	{
		if (this.conVar == null)
		{
			return;
		}
		string @string = this.conVar.String;
		foreach (Toggle toggle in this.toggleGroup.GetComponentsInChildren<Toggle>())
		{
			toggle.isOn = (toggle.name == @string);
		}
	}

	// Token: 0x060036B9 RID: 14009 RVA: 0x0014A9C4 File Offset: 0x00148BC4
	private void UpdateConVar()
	{
		if (this.conVar == null)
		{
			return;
		}
		Toggle toggle = (from x in this.toggleGroup.GetComponentsInChildren<Toggle>()
		where x.isOn
		select x).FirstOrDefault<Toggle>();
		if (toggle == null)
		{
			return;
		}
		if (this.conVar.String == toggle.name)
		{
			return;
		}
		this.conVar.Set(toggle.name);
	}
}

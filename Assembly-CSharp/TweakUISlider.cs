using System;
using TMPro;
using UnityEngine.UI;

// Token: 0x02000889 RID: 2185
public class TweakUISlider : TweakUIBase
{
	// Token: 0x0400310D RID: 12557
	public Slider sliderControl;

	// Token: 0x0400310E RID: 12558
	public TextMeshProUGUI textControl;

	// Token: 0x0400310F RID: 12559
	public static string lastConVarChanged;

	// Token: 0x04003110 RID: 12560
	public static TimeSince timeSinceLastConVarChange;

	// Token: 0x060036BB RID: 14011 RVA: 0x0014AA4B File Offset: 0x00148C4B
	protected override void Init()
	{
		base.Init();
		this.ResetToConvar();
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x0014A5BC File Offset: 0x001487BC
	protected void OnEnable()
	{
		this.ResetToConvar();
	}

	// Token: 0x060036BD RID: 14013 RVA: 0x0014AA59 File Offset: 0x00148C59
	public void OnChanged()
	{
		this.RefreshSliderDisplay(this.sliderControl.value);
		if (this.ApplyImmediatelyOnChange)
		{
			this.SetConvarValue();
		}
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x0014AA7C File Offset: 0x00148C7C
	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		if (this.conVar == null)
		{
			return;
		}
		float value = this.sliderControl.value;
		if (this.conVar.AsFloat == value)
		{
			return;
		}
		this.conVar.Set(value);
		this.RefreshSliderDisplay(this.conVar.AsFloat);
		TweakUISlider.lastConVarChanged = this.conVar.FullName;
		TweakUISlider.timeSinceLastConVarChange = 0f;
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x0014AAEF File Offset: 0x00148CEF
	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (this.conVar == null)
		{
			return;
		}
		this.RefreshSliderDisplay(this.conVar.AsFloat);
	}

	// Token: 0x060036C0 RID: 14016 RVA: 0x0014AB14 File Offset: 0x00148D14
	private void RefreshSliderDisplay(float value)
	{
		this.sliderControl.value = value;
		if (this.sliderControl.wholeNumbers)
		{
			this.textControl.text = this.sliderControl.value.ToString("N0");
			return;
		}
		this.textControl.text = this.sliderControl.value.ToString("0.0");
	}
}

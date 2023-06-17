using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008BD RID: 2237
public class TimeSlider : MonoBehaviour
{
	// Token: 0x0400323F RID: 12863
	private Slider slider;

	// Token: 0x0600373A RID: 14138 RVA: 0x0014CC9C File Offset: 0x0014AE9C
	private void Start()
	{
		this.slider = base.GetComponent<Slider>();
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x0014CCAA File Offset: 0x0014AEAA
	private void Update()
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		this.slider.value = TOD_Sky.Instance.Cycle.Hour;
	}

	// Token: 0x0600373C RID: 14140 RVA: 0x0014CCD4 File Offset: 0x0014AED4
	public void OnValue(float f)
	{
		if (TOD_Sky.Instance == null)
		{
			return;
		}
		TOD_Sky.Instance.Cycle.Hour = f;
		TOD_Sky.Instance.UpdateAmbient();
		TOD_Sky.Instance.UpdateReflection();
		TOD_Sky.Instance.UpdateFog();
	}
}

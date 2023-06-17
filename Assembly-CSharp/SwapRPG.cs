using System;
using UnityEngine;

// Token: 0x02000973 RID: 2419
public class SwapRPG : MonoBehaviour
{
	// Token: 0x04003401 RID: 13313
	public GameObject[] rpgModels;

	// Token: 0x04003402 RID: 13314
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x060039E5 RID: 14821 RVA: 0x00157500 File Offset: 0x00155700
	public void SelectRPGType(int iType)
	{
		GameObject[] array = this.rpgModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		this.rpgModels[iType].SetActive(true);
	}

	// Token: 0x060039E6 RID: 14822 RVA: 0x0015753C File Offset: 0x0015573C
	public void UpdateAmmoType(ItemDefinition ammoType)
	{
		if (this.curAmmoType == ammoType.shortname)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		string a = this.curAmmoType;
		if (!(a == "ammo.rocket.basic"))
		{
			if (a == "ammo.rocket.fire")
			{
				this.SelectRPGType(1);
				return;
			}
			if (a == "ammo.rocket.hv")
			{
				this.SelectRPGType(2);
				return;
			}
			if (a == "ammo.rocket.smoke")
			{
				this.SelectRPGType(3);
				return;
			}
		}
		this.SelectRPGType(0);
	}

	// Token: 0x060039E7 RID: 14823 RVA: 0x000063A5 File Offset: 0x000045A5
	private void Start()
	{
	}

	// Token: 0x02000EC8 RID: 3784
	public enum RPGType
	{
		// Token: 0x04004CF0 RID: 19696
		One,
		// Token: 0x04004CF1 RID: 19697
		Two,
		// Token: 0x04004CF2 RID: 19698
		Three,
		// Token: 0x04004CF3 RID: 19699
		Four
	}
}

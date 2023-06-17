using System;
using Rust;
using UnityEngine;

// Token: 0x02000971 RID: 2417
public class SwapArrows : MonoBehaviour, IClientComponent
{
	// Token: 0x040033FD RID: 13309
	public GameObject[] arrowModels;

	// Token: 0x040033FE RID: 13310
	[NonSerialized]
	private string curAmmoType = "";

	// Token: 0x040033FF RID: 13311
	private bool wasHidden;

	// Token: 0x060039DB RID: 14811 RVA: 0x00157331 File Offset: 0x00155531
	public void SelectArrowType(int iType)
	{
		this.HideAllArrowHeads();
		if (iType < this.arrowModels.Length)
		{
			this.arrowModels[iType].SetActive(true);
		}
	}

	// Token: 0x060039DC RID: 14812 RVA: 0x00157354 File Offset: 0x00155554
	public void HideAllArrowHeads()
	{
		GameObject[] array = this.arrowModels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x060039DD RID: 14813 RVA: 0x00157380 File Offset: 0x00155580
	public void UpdateAmmoType(ItemDefinition ammoType, bool hidden = false)
	{
		if (hidden)
		{
			this.wasHidden = hidden;
			this.HideAllArrowHeads();
			return;
		}
		if (this.curAmmoType == ammoType.shortname && hidden == this.wasHidden)
		{
			return;
		}
		this.curAmmoType = ammoType.shortname;
		this.wasHidden = hidden;
		string a = this.curAmmoType;
		if (!(a == "ammo_arrow"))
		{
			if (a == "arrow.bone")
			{
				this.SelectArrowType(0);
				return;
			}
			if (a == "arrow.fire")
			{
				this.SelectArrowType(1);
				return;
			}
			if (a == "arrow.hv")
			{
				this.SelectArrowType(2);
				return;
			}
			if (a == "ammo_arrow_poison")
			{
				this.SelectArrowType(3);
				return;
			}
			if (a == "ammo_arrow_stone")
			{
				this.SelectArrowType(4);
				return;
			}
		}
		this.HideAllArrowHeads();
	}

	// Token: 0x060039DE RID: 14814 RVA: 0x00157451 File Offset: 0x00155651
	private void Cleanup()
	{
		this.HideAllArrowHeads();
		this.curAmmoType = "";
	}

	// Token: 0x060039DF RID: 14815 RVA: 0x00157464 File Offset: 0x00155664
	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Cleanup();
	}

	// Token: 0x060039E0 RID: 14816 RVA: 0x00157474 File Offset: 0x00155674
	public void OnEnable()
	{
		this.Cleanup();
	}

	// Token: 0x02000EC7 RID: 3783
	public enum ArrowType
	{
		// Token: 0x04004CEB RID: 19691
		One,
		// Token: 0x04004CEC RID: 19692
		Two,
		// Token: 0x04004CED RID: 19693
		Three,
		// Token: 0x04004CEE RID: 19694
		Four
	}
}

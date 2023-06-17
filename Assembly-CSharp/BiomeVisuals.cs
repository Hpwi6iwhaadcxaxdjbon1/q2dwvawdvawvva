using System;
using UnityEngine;

// Token: 0x02000674 RID: 1652
public class BiomeVisuals : MonoBehaviour
{
	// Token: 0x04002708 RID: 9992
	public GameObject Arid;

	// Token: 0x04002709 RID: 9993
	public GameObject Temperate;

	// Token: 0x0400270A RID: 9994
	public GameObject Tundra;

	// Token: 0x0400270B RID: 9995
	public GameObject Arctic;

	// Token: 0x06002F9A RID: 12186 RVA: 0x0011E44C File Offset: 0x0011C64C
	protected void Start()
	{
		int num = (TerrainMeta.BiomeMap != null) ? TerrainMeta.BiomeMap.GetBiomeMaxType(base.transform.position, -1) : 2;
		switch (num)
		{
		case 1:
			this.SetChoice(this.Arid);
			return;
		case 2:
			this.SetChoice(this.Temperate);
			return;
		case 3:
			break;
		case 4:
			this.SetChoice(this.Tundra);
			return;
		default:
			if (num != 8)
			{
				return;
			}
			this.SetChoice(this.Arctic);
			break;
		}
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x0011E4D0 File Offset: 0x0011C6D0
	private void SetChoice(GameObject selection)
	{
		bool shouldDestroy = !base.gameObject.SupportsPoolingInParent();
		this.ApplyChoice(selection, this.Arid, shouldDestroy);
		this.ApplyChoice(selection, this.Temperate, shouldDestroy);
		this.ApplyChoice(selection, this.Tundra, shouldDestroy);
		this.ApplyChoice(selection, this.Arctic, shouldDestroy);
		if (selection != null)
		{
			selection.SetActive(true);
		}
		GameManager.Destroy(this, 0f);
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x0011E53F File Offset: 0x0011C73F
	private void ApplyChoice(GameObject selection, GameObject target, bool shouldDestroy)
	{
		if (target != null && target != selection)
		{
			if (shouldDestroy)
			{
				GameManager.Destroy(target, 0f);
				return;
			}
			target.SetActive(false);
		}
	}
}

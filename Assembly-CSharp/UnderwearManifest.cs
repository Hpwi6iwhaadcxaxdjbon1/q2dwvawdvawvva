using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000765 RID: 1893
[CreateAssetMenu(menuName = "Rust/Underwear Manifest")]
public class UnderwearManifest : ScriptableObject
{
	// Token: 0x04002AF2 RID: 10994
	public static UnderwearManifest instance;

	// Token: 0x04002AF3 RID: 10995
	public List<Underwear> underwears;

	// Token: 0x060034B6 RID: 13494 RVA: 0x00145FFF File Offset: 0x001441FF
	public static UnderwearManifest Get()
	{
		if (UnderwearManifest.instance == null)
		{
			UnderwearManifest.instance = Resources.Load<UnderwearManifest>("UnderwearManifest");
		}
		return UnderwearManifest.instance;
	}

	// Token: 0x060034B7 RID: 13495 RVA: 0x00146024 File Offset: 0x00144224
	public void PrintManifest()
	{
		Debug.Log("MANIFEST CONTENTS");
		foreach (Underwear underwear in this.underwears)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Underwear name : ",
				underwear.shortname,
				" underwear ID : ",
				underwear.GetID()
			}));
		}
	}

	// Token: 0x060034B8 RID: 13496 RVA: 0x001460B4 File Offset: 0x001442B4
	public Underwear GetUnderwear(uint id)
	{
		foreach (Underwear underwear in this.underwears)
		{
			if (underwear.GetID() == id)
			{
				return underwear;
			}
		}
		return null;
	}
}

using System;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class AnimalSkin : MonoBehaviour, IClientComponent
{
	// Token: 0x040015A5 RID: 5541
	public SkinnedMeshRenderer[] animalMesh;

	// Token: 0x040015A6 RID: 5542
	public AnimalMultiSkin[] animalSkins;

	// Token: 0x040015A7 RID: 5543
	private Model model;

	// Token: 0x040015A8 RID: 5544
	public bool dontRandomizeOnStart;

	// Token: 0x06001CF8 RID: 7416 RVA: 0x000C883C File Offset: 0x000C6A3C
	private void Start()
	{
		this.model = base.gameObject.GetComponent<Model>();
		if (!this.dontRandomizeOnStart)
		{
			int iSkin = Mathf.FloorToInt((float)UnityEngine.Random.Range(0, this.animalSkins.Length));
			this.ChangeSkin(iSkin);
		}
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x000C8880 File Offset: 0x000C6A80
	public void ChangeSkin(int iSkin)
	{
		if (this.animalSkins.Length == 0)
		{
			return;
		}
		iSkin = Mathf.Clamp(iSkin, 0, this.animalSkins.Length - 1);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.animalMesh)
		{
			Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			if (sharedMaterials != null)
			{
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					sharedMaterials[j] = this.animalSkins[iSkin].multiSkin[j];
				}
				skinnedMeshRenderer.sharedMaterials = sharedMaterials;
			}
		}
		if (this.model != null)
		{
			this.model.skin = iSkin;
		}
	}
}

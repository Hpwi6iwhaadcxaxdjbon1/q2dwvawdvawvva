using System;
using UnityEngine;

// Token: 0x0200020F RID: 527
[CreateAssetMenu(menuName = "Scriptable Object/Horse Breed", fileName = "newbreed.asset")]
public class HorseBreed : ScriptableObject
{
	// Token: 0x04001364 RID: 4964
	public Translate.Phrase breedName;

	// Token: 0x04001365 RID: 4965
	public Translate.Phrase breedDesc;

	// Token: 0x04001366 RID: 4966
	public Material[] materialOverrides;

	// Token: 0x04001367 RID: 4967
	public float maxHealth = 1f;

	// Token: 0x04001368 RID: 4968
	public float maxSpeed = 1f;

	// Token: 0x04001369 RID: 4969
	public float staminaDrain = 1f;

	// Token: 0x0400136A RID: 4970
	public float maxStamina = 1f;
}

using System;
using UnityEngine;

// Token: 0x02000394 RID: 916
public interface IPet
{
	// Token: 0x06002053 RID: 8275
	bool IsPet();

	// Token: 0x06002054 RID: 8276
	void SetPetOwner(BasePlayer player);

	// Token: 0x06002055 RID: 8277
	bool IsOwnedBy(BasePlayer player);

	// Token: 0x06002056 RID: 8278
	bool IssuePetCommand(PetCommandType cmd, int param, Ray? ray);
}

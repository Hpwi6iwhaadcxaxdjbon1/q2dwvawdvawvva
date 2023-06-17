using System;
using UnityEngine;

// Token: 0x020003DA RID: 986
public interface ISignage
{
	// Token: 0x060021EA RID: 8682
	bool CanUpdateSign(BasePlayer player);

	// Token: 0x060021EB RID: 8683
	float Distance(Vector3 position);

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x060021EC RID: 8684
	Vector2i TextureSize { get; }

	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x060021ED RID: 8685
	int TextureCount { get; }

	// Token: 0x060021EE RID: 8686
	uint[] GetTextureCRCs();

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x060021EF RID: 8687
	NetworkableId NetworkID { get; }

	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x060021F0 RID: 8688
	FileStorage.Type FileType { get; }

	// Token: 0x060021F1 RID: 8689
	void SetTextureCRCs(uint[] crcs);
}

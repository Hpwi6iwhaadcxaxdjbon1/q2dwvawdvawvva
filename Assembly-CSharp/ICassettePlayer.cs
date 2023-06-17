using System;

// Token: 0x02000398 RID: 920
public interface ICassettePlayer
{
	// Token: 0x06002073 RID: 8307
	void OnCassetteInserted(Cassette c);

	// Token: 0x06002074 RID: 8308
	void OnCassetteRemoved(Cassette c);

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06002075 RID: 8309
	BaseEntity ToBaseEntity { get; }
}

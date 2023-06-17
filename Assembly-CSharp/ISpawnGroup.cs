using System;

// Token: 0x0200056E RID: 1390
public interface ISpawnGroup
{
	// Token: 0x06002A9B RID: 10907
	void Clear();

	// Token: 0x06002A9C RID: 10908
	void Fill();

	// Token: 0x06002A9D RID: 10909
	void SpawnInitial();

	// Token: 0x06002A9E RID: 10910
	void SpawnRepeating();

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06002A9F RID: 10911
	int currentPopulation { get; }
}

using System;

// Token: 0x0200056F RID: 1391
public interface ISpawnPointUser
{
	// Token: 0x06002AA0 RID: 10912
	void ObjectSpawned(SpawnPointInstance instance);

	// Token: 0x06002AA1 RID: 10913
	void ObjectRetired(SpawnPointInstance instance);
}

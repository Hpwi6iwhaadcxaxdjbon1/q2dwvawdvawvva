using System;

// Token: 0x02000414 RID: 1044
public interface ISplashable
{
	// Token: 0x06002340 RID: 9024
	bool WantsSplash(ItemDefinition splashType, int amount);

	// Token: 0x06002341 RID: 9025
	int DoSplash(ItemDefinition splashType, int amount);
}

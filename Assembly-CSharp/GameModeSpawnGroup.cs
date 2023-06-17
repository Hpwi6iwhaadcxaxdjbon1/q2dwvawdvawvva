using System;

// Token: 0x0200056C RID: 1388
public class GameModeSpawnGroup : SpawnGroup
{
	// Token: 0x040022A2 RID: 8866
	public string[] gameModeTags;

	// Token: 0x06002A92 RID: 10898 RVA: 0x00103825 File Offset: 0x00101A25
	public void ResetSpawnGroup()
	{
		base.Clear();
		this.SpawnInitial();
	}

	// Token: 0x06002A93 RID: 10899 RVA: 0x00103834 File Offset: 0x00101A34
	public bool ShouldSpawn()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		return !(activeGameMode == null) && (this.gameModeTags.Length == 0 || activeGameMode.HasAnyGameModeTag(this.gameModeTags));
	}

	// Token: 0x06002A94 RID: 10900 RVA: 0x0010386F File Offset: 0x00101A6F
	protected override void Spawn(int numToSpawn)
	{
		if (this.ShouldSpawn())
		{
			base.Spawn(numToSpawn);
		}
	}
}

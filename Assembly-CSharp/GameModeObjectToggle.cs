using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000511 RID: 1297
public class GameModeObjectToggle : BaseMonoBehaviour
{
	// Token: 0x0400216A RID: 8554
	public string[] gameModeTags;

	// Token: 0x0400216B RID: 8555
	public string[] tagsToDisable;

	// Token: 0x0400216C RID: 8556
	public GameObject[] toToggle;

	// Token: 0x0400216D RID: 8557
	public bool defaultState;

	// Token: 0x06002965 RID: 10597 RVA: 0x000FDEE6 File Offset: 0x000FC0E6
	public void Awake()
	{
		this.SetToggle(this.defaultState);
		BaseGameMode.GameModeChanged += this.OnGameModeChanged;
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x000FDF05 File Offset: 0x000FC105
	public void OnDestroy()
	{
		BaseGameMode.GameModeChanged -= this.OnGameModeChanged;
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x000FDF18 File Offset: 0x000FC118
	public void OnGameModeChanged(BaseGameMode newGameMode)
	{
		bool toggle = this.ShouldBeVisible(newGameMode);
		this.SetToggle(toggle);
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x000FDF34 File Offset: 0x000FC134
	public void SetToggle(bool wantsOn)
	{
		foreach (GameObject gameObject in this.toToggle)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(wantsOn);
			}
		}
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x000FDF6C File Offset: 0x000FC16C
	public bool ShouldBeVisible(BaseGameMode newGameMode)
	{
		if (newGameMode == null)
		{
			return this.defaultState;
		}
		return (this.tagsToDisable.Length == 0 || (!newGameMode.HasAnyGameModeTag(this.tagsToDisable) && !this.tagsToDisable.Contains("*"))) && ((this.gameModeTags.Length != 0 && (newGameMode.HasAnyGameModeTag(this.gameModeTags) || this.gameModeTags.Contains("*"))) || this.defaultState);
	}
}

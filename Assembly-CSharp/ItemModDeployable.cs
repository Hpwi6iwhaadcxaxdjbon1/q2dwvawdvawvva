using System;
using UnityEngine;

// Token: 0x020005E9 RID: 1513
public class ItemModDeployable : MonoBehaviour
{
	// Token: 0x040024D8 RID: 9432
	public GameObjectRef entityPrefab = new GameObjectRef();

	// Token: 0x040024D9 RID: 9433
	[Header("Tooltips")]
	public bool showCrosshair;

	// Token: 0x040024DA RID: 9434
	public string UnlockAchievement;

	// Token: 0x06002D49 RID: 11593 RVA: 0x00110E58 File Offset: 0x0010F058
	public Deployable GetDeployable(BaseEntity entity)
	{
		if (entity.gameManager.FindPrefab(this.entityPrefab.resourcePath) == null)
		{
			return null;
		}
		return entity.prefabAttribute.Find<Deployable>(this.entityPrefab.resourceID);
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x00110E90 File Offset: 0x0010F090
	internal void OnDeployed(BaseEntity ent, BasePlayer player)
	{
		if (player.IsValid() && !string.IsNullOrEmpty(this.UnlockAchievement))
		{
			player.GiveAchievement(this.UnlockAchievement);
		}
		BuildingPrivlidge buildingPrivlidge;
		if ((buildingPrivlidge = (ent as BuildingPrivlidge)) != null)
		{
			buildingPrivlidge.AddPlayer(player);
		}
	}
}

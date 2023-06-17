using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class BaseArcadeGame : BaseMonoBehaviour
{
	// Token: 0x04000F7B RID: 3963
	public static List<BaseArcadeGame> globalActiveGames = new List<BaseArcadeGame>();

	// Token: 0x04000F7C RID: 3964
	public Camera cameraToRender;

	// Token: 0x04000F7D RID: 3965
	public RenderTexture renderTexture;

	// Token: 0x04000F7E RID: 3966
	public Texture2D distantTexture;

	// Token: 0x04000F7F RID: 3967
	public Transform center;

	// Token: 0x04000F80 RID: 3968
	public int frameRate = 30;

	// Token: 0x04000F81 RID: 3969
	public Dictionary<uint, ArcadeEntity> activeArcadeEntities = new Dictionary<uint, ArcadeEntity>();

	// Token: 0x04000F82 RID: 3970
	public Sprite[] spriteManifest;

	// Token: 0x04000F83 RID: 3971
	public ArcadeEntity[] entityManifest;

	// Token: 0x04000F84 RID: 3972
	public bool clientside;

	// Token: 0x04000F85 RID: 3973
	public bool clientsideInput = true;

	// Token: 0x04000F86 RID: 3974
	public const int spriteIndexInvisible = 1555;

	// Token: 0x04000F87 RID: 3975
	public GameObject arcadeEntityPrefab;

	// Token: 0x04000F88 RID: 3976
	public BaseArcadeMachine ownerMachine;

	// Token: 0x04000F89 RID: 3977
	public static int gameOffsetIndex = 0;

	// Token: 0x04000F8A RID: 3978
	private bool isAuthorative;

	// Token: 0x04000F8B RID: 3979
	public Canvas canvas;

	// Token: 0x06001707 RID: 5895 RVA: 0x000AFF7B File Offset: 0x000AE17B
	public BasePlayer GetHostPlayer()
	{
		if (this.ownerMachine)
		{
			return this.ownerMachine.GetDriver();
		}
		return null;
	}
}

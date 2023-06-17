using System;
using UnityEngine;

// Token: 0x020005B7 RID: 1463
[CreateAssetMenu(menuName = "Rust/Gestures/Gesture Config")]
public class GestureConfig : ScriptableObject
{
	// Token: 0x040023B6 RID: 9142
	[ReadOnly]
	public uint gestureId;

	// Token: 0x040023B7 RID: 9143
	public string gestureCommand;

	// Token: 0x040023B8 RID: 9144
	public string convarName;

	// Token: 0x040023B9 RID: 9145
	public Translate.Phrase gestureName;

	// Token: 0x040023BA RID: 9146
	public Sprite icon;

	// Token: 0x040023BB RID: 9147
	public int order = 1;

	// Token: 0x040023BC RID: 9148
	public float duration = 1.5f;

	// Token: 0x040023BD RID: 9149
	public bool canCancel = true;

	// Token: 0x040023BE RID: 9150
	[Header("Player model setup")]
	public GestureConfig.PlayerModelLayer playerModelLayer = GestureConfig.PlayerModelLayer.UpperBody;

	// Token: 0x040023BF RID: 9151
	public GestureConfig.GestureType gestureType;

	// Token: 0x040023C0 RID: 9152
	public bool hideHeldEntity = true;

	// Token: 0x040023C1 RID: 9153
	public bool canDuckDuringGesture;

	// Token: 0x040023C2 RID: 9154
	public GestureConfig.MovementCapabilities movementMode;

	// Token: 0x040023C3 RID: 9155
	public GestureConfig.AnimationType animationType;

	// Token: 0x040023C4 RID: 9156
	public BasePlayer.CameraMode viewMode;

	// Token: 0x040023C5 RID: 9157
	public bool useRootMotion;

	// Token: 0x040023C6 RID: 9158
	[Header("Ownership")]
	public GestureConfig.GestureActionType actionType;

	// Token: 0x040023C7 RID: 9159
	public bool forceUnlock;

	// Token: 0x040023C8 RID: 9160
	public SteamDLCItem dlcItem;

	// Token: 0x040023C9 RID: 9161
	public SteamInventoryItem inventoryItem;

	// Token: 0x06002C15 RID: 11285 RVA: 0x0010B1AC File Offset: 0x001093AC
	public bool IsOwnedBy(BasePlayer player)
	{
		if (this.forceUnlock)
		{
			return true;
		}
		if (this.gestureType == GestureConfig.GestureType.NPC)
		{
			return player.IsNpc;
		}
		if (this.gestureType == GestureConfig.GestureType.Cinematic)
		{
			return player.IsAdmin;
		}
		return (this.dlcItem != null && this.dlcItem.CanUse(player)) || (this.inventoryItem != null && player.blueprints.steamInventory.HasItem(this.inventoryItem.id));
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x0010B230 File Offset: 0x00109430
	public bool CanBeUsedBy(BasePlayer player)
	{
		if (player.isMounted)
		{
			if (this.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody)
			{
				return false;
			}
			if (player.GetMounted().allowedGestures == BaseMountable.MountGestureType.None)
			{
				return false;
			}
		}
		return (!player.IsSwimming() || this.playerModelLayer != GestureConfig.PlayerModelLayer.FullBody) && (this.playerModelLayer != GestureConfig.PlayerModelLayer.FullBody || !player.modelState.ducked);
	}

	// Token: 0x02000D67 RID: 3431
	public enum GestureType
	{
		// Token: 0x0400475E RID: 18270
		Player,
		// Token: 0x0400475F RID: 18271
		NPC,
		// Token: 0x04004760 RID: 18272
		Cinematic
	}

	// Token: 0x02000D68 RID: 3432
	public enum PlayerModelLayer
	{
		// Token: 0x04004762 RID: 18274
		UpperBody = 3,
		// Token: 0x04004763 RID: 18275
		FullBody
	}

	// Token: 0x02000D69 RID: 3433
	public enum MovementCapabilities
	{
		// Token: 0x04004765 RID: 18277
		FullMovement,
		// Token: 0x04004766 RID: 18278
		NoMovement
	}

	// Token: 0x02000D6A RID: 3434
	public enum AnimationType
	{
		// Token: 0x04004768 RID: 18280
		OneShot,
		// Token: 0x04004769 RID: 18281
		Loop
	}

	// Token: 0x02000D6B RID: 3435
	public enum ViewMode
	{
		// Token: 0x0400476B RID: 18283
		FirstPerson,
		// Token: 0x0400476C RID: 18284
		ThirdPerson
	}

	// Token: 0x02000D6C RID: 3436
	public enum GestureActionType
	{
		// Token: 0x0400476E RID: 18286
		None,
		// Token: 0x0400476F RID: 18287
		ShowNameTag,
		// Token: 0x04004770 RID: 18288
		DanceAchievement
	}
}

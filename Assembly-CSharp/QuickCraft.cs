using System;
using UnityEngine;

// Token: 0x0200084A RID: 2122
public class QuickCraft : SingletonComponent<QuickCraft>, IInventoryChanged
{
	// Token: 0x04002F89 RID: 12169
	public GameObjectRef craftButton;

	// Token: 0x04002F8A RID: 12170
	public GameObject empty;

	// Token: 0x04002F8B RID: 12171
	public Sprite FavouriteOnSprite;

	// Token: 0x04002F8C RID: 12172
	public Sprite FavouriteOffSprite;

	// Token: 0x04002F8D RID: 12173
	public Color FavouriteOnColor;

	// Token: 0x04002F8E RID: 12174
	public Color FavouriteOffColor;
}

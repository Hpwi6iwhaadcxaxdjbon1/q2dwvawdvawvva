using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000818 RID: 2072
public class UIBlueprints : ListComponent<UIBlueprints>
{
	// Token: 0x04002E7C RID: 11900
	public GameObjectRef buttonPrefab;

	// Token: 0x04002E7D RID: 11901
	public ScrollRect scrollRect;

	// Token: 0x04002E7E RID: 11902
	public CanvasGroup ScrollRectCanvasGroup;

	// Token: 0x04002E7F RID: 11903
	public InputField searchField;

	// Token: 0x04002E80 RID: 11904
	public GameObject searchFieldPlaceholder;

	// Token: 0x04002E81 RID: 11905
	public GameObject listAvailable;

	// Token: 0x04002E82 RID: 11906
	public GameObject listLocked;

	// Token: 0x04002E83 RID: 11907
	public GameObject Categories;

	// Token: 0x04002E84 RID: 11908
	public VerticalLayoutGroup CategoryVerticalLayoutGroup;

	// Token: 0x04002E85 RID: 11909
	public BlueprintCategoryButton FavouriteCategoryButton;
}

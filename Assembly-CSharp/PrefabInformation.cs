using System;
using UnityEngine;

// Token: 0x02000553 RID: 1363
public class PrefabInformation : PrefabAttribute
{
	// Token: 0x04002246 RID: 8774
	public ItemDefinition associatedItemDefinition;

	// Token: 0x04002247 RID: 8775
	public Translate.Phrase title;

	// Token: 0x04002248 RID: 8776
	public Translate.Phrase description;

	// Token: 0x04002249 RID: 8777
	public Sprite sprite;

	// Token: 0x0400224A RID: 8778
	public bool shownOnDeathScreen;

	// Token: 0x06002A1E RID: 10782 RVA: 0x0010090D File Offset: 0x000FEB0D
	protected override Type GetIndexedType()
	{
		return typeof(PrefabInformation);
	}
}

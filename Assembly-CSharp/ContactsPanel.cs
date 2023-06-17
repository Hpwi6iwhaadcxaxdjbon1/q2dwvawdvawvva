using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007EC RID: 2028
public class ContactsPanel : SingletonComponent<ContactsPanel>
{
	// Token: 0x04002D55 RID: 11605
	public RectTransform alliesBucket;

	// Token: 0x04002D56 RID: 11606
	public RectTransform seenBucket;

	// Token: 0x04002D57 RID: 11607
	public RectTransform enemiesBucket;

	// Token: 0x04002D58 RID: 11608
	public RectTransform contentsBucket;

	// Token: 0x04002D59 RID: 11609
	public ContactsEntry contactEntryPrefab;

	// Token: 0x04002D5A RID: 11610
	public RawImage mugshotTest;

	// Token: 0x04002D5B RID: 11611
	public RawImage fullBodyTest;

	// Token: 0x04002D5C RID: 11612
	public RustButton[] filterButtons;

	// Token: 0x04002D5D RID: 11613
	public RelationshipManager.RelationshipType selectedRelationshipType = RelationshipManager.RelationshipType.Friend;

	// Token: 0x04002D5E RID: 11614
	public RustButton lastSeenToggle;

	// Token: 0x04002D5F RID: 11615
	public Translate.Phrase sortingByLastSeenPhrase;

	// Token: 0x04002D60 RID: 11616
	public Translate.Phrase sortingByFirstSeen;

	// Token: 0x04002D61 RID: 11617
	public RustText sortText;

	// Token: 0x02000E7C RID: 3708
	public enum SortMode
	{
		// Token: 0x04004BDD RID: 19421
		None,
		// Token: 0x04004BDE RID: 19422
		RecentlySeen
	}
}

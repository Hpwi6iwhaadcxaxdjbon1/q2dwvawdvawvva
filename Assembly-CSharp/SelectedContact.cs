using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000803 RID: 2051
public class SelectedContact : SingletonComponent<SelectedContact>
{
	// Token: 0x04002DFC RID: 11772
	public RustText nameText;

	// Token: 0x04002DFD RID: 11773
	public RustText seenText;

	// Token: 0x04002DFE RID: 11774
	public RawImage mugshotImage;

	// Token: 0x04002DFF RID: 11775
	public Texture2D unknownMugshot;

	// Token: 0x04002E00 RID: 11776
	public InputField noteInput;

	// Token: 0x04002E01 RID: 11777
	public GameObject[] relationshipTypeTags;

	// Token: 0x04002E02 RID: 11778
	public Translate.Phrase lastSeenPrefix;

	// Token: 0x04002E03 RID: 11779
	public Translate.Phrase nowPhrase;

	// Token: 0x04002E04 RID: 11780
	public Translate.Phrase agoSuffix;

	// Token: 0x04002E05 RID: 11781
	public RustButton FriendlyButton;

	// Token: 0x04002E06 RID: 11782
	public RustButton SeenButton;

	// Token: 0x04002E07 RID: 11783
	public RustButton EnemyButton;

	// Token: 0x04002E08 RID: 11784
	public RustButton chatMute;
}

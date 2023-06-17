using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000796 RID: 1942
public class ChatEntry : MonoBehaviour
{
	// Token: 0x04002B25 RID: 11045
	public TextMeshProUGUI text;

	// Token: 0x04002B26 RID: 11046
	public RawImage avatar;

	// Token: 0x04002B27 RID: 11047
	public CanvasGroup canvasGroup;

	// Token: 0x04002B28 RID: 11048
	public float lifeStarted;

	// Token: 0x04002B29 RID: 11049
	public ulong steamid;

	// Token: 0x04002B2A RID: 11050
	public Translate.Phrase LocalPhrase = new Translate.Phrase("local", "local");

	// Token: 0x04002B2B RID: 11051
	public Translate.Phrase CardsPhrase = new Translate.Phrase("cards", "cards");

	// Token: 0x04002B2C RID: 11052
	public Translate.Phrase TeamPhrase = new Translate.Phrase("team", "team");
}

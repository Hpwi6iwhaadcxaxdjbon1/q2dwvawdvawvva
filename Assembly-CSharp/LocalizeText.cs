using System;
using Rust.Localization;
using UnityEngine;

// Token: 0x020008E1 RID: 2273
public class LocalizeText : MonoBehaviour, IClientComponent, ILocalize
{
	// Token: 0x04003293 RID: 12947
	public string token;

	// Token: 0x04003294 RID: 12948
	[TextArea]
	public string english;

	// Token: 0x04003295 RID: 12949
	public string append;

	// Token: 0x04003296 RID: 12950
	public LocalizeText.SpecialMode specialMode;

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06003794 RID: 14228 RVA: 0x0014DB08 File Offset: 0x0014BD08
	// (set) Token: 0x06003795 RID: 14229 RVA: 0x0014DB10 File Offset: 0x0014BD10
	public string LanguageToken
	{
		get
		{
			return this.token;
		}
		set
		{
			this.token = value;
		}
	}

	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06003796 RID: 14230 RVA: 0x0014DB19 File Offset: 0x0014BD19
	// (set) Token: 0x06003797 RID: 14231 RVA: 0x0014DB21 File Offset: 0x0014BD21
	public string LanguageEnglish
	{
		get
		{
			return this.english;
		}
		set
		{
			this.english = value;
		}
	}

	// Token: 0x02000EAA RID: 3754
	public enum SpecialMode
	{
		// Token: 0x04004C82 RID: 19586
		None,
		// Token: 0x04004C83 RID: 19587
		AllUppercase,
		// Token: 0x04004C84 RID: 19588
		AllLowercase
	}
}

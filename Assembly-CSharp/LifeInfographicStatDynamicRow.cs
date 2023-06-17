using System;
using Rust.UI;

// Token: 0x02000862 RID: 2146
public class LifeInfographicStatDynamicRow : LifeInfographicStat
{
	// Token: 0x04003036 RID: 12342
	public RustText StatName;

	// Token: 0x0600363E RID: 13886 RVA: 0x001493A9 File Offset: 0x001475A9
	public void SetStatName(Translate.Phrase phrase)
	{
		this.StatName.SetPhrase(phrase);
	}
}

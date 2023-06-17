using System;

// Token: 0x02000897 RID: 2199
public class WorkshopMainMenu : SingletonComponent<WorkshopMainMenu>
{
	// Token: 0x0400314E RID: 12622
	public static Translate.Phrase loading_workshop = new TokenisedPhrase("loading.workshop", "Loading Workshop");

	// Token: 0x0400314F RID: 12623
	public static Translate.Phrase loading_workshop_setup = new TokenisedPhrase("loading.workshop.initializing", "Setting Up Scene");

	// Token: 0x04003150 RID: 12624
	public static Translate.Phrase loading_workshop_skinnables = new TokenisedPhrase("loading.workshop.skinnables", "Getting Skinnables");

	// Token: 0x04003151 RID: 12625
	public static Translate.Phrase loading_workshop_item = new TokenisedPhrase("loading.workshop.item", "Loading Item Data");
}

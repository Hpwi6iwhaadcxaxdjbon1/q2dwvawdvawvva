using System;

// Token: 0x020002A4 RID: 676
public class Client : SingletonComponent<Client>
{
	// Token: 0x04001618 RID: 5656
	public static Translate.Phrase loading_loading = new Translate.Phrase("loading.loading", "Loading");

	// Token: 0x04001619 RID: 5657
	public static Translate.Phrase loading_connecting = new Translate.Phrase("loading.connecting", "Connecting");

	// Token: 0x0400161A RID: 5658
	public static Translate.Phrase loading_connectionaccepted = new Translate.Phrase("loading.connectionaccepted", "Connection Accepted");

	// Token: 0x0400161B RID: 5659
	public static Translate.Phrase loading_connecting_negotiate = new Translate.Phrase("loading.connecting.negotiate", "Negotiating Connection");

	// Token: 0x0400161C RID: 5660
	public static Translate.Phrase loading_level = new Translate.Phrase("loading.loadinglevel", "Loading Level");

	// Token: 0x0400161D RID: 5661
	public static Translate.Phrase loading_skinnablewarmup = new Translate.Phrase("loading.skinnablewarmup", "Skinnable Warmup");

	// Token: 0x0400161E RID: 5662
	public static Translate.Phrase loading_preloadcomplete = new Translate.Phrase("loading.preloadcomplete", "Preload Complete");

	// Token: 0x0400161F RID: 5663
	public static Translate.Phrase loading_openingscene = new Translate.Phrase("loading.openingscene", "Opening Scene");

	// Token: 0x04001620 RID: 5664
	public static Translate.Phrase loading_clientready = new Translate.Phrase("loading.clientready", "Client Ready");

	// Token: 0x04001621 RID: 5665
	public static Translate.Phrase loading_prefabwarmup = new Translate.Phrase("loading.prefabwarmup", "Warming Prefabs [{0}/{1}]");
}

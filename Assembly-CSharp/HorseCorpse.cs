using System;
using Facepunch;
using ProtoBuf;

// Token: 0x02000210 RID: 528
public class HorseCorpse : global::LootableCorpse
{
	// Token: 0x0400136B RID: 4971
	public int breedIndex;

	// Token: 0x0400136C RID: 4972
	public Translate.Phrase lootPanelTitle;

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x06001B66 RID: 7014 RVA: 0x000C2147 File Offset: 0x000C0347
	public override string playerName
	{
		get
		{
			return this.lootPanelTitle.translated;
		}
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x000C2154 File Offset: 0x000C0354
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.horse = Pool.Get<ProtoBuf.Horse>();
		info.msg.horse.breedIndex = this.breedIndex;
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x000C2183 File Offset: 0x000C0383
	public override void Load(global::BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.horse != null)
		{
			this.breedIndex = info.msg.horse.breedIndex;
		}
	}
}

using System;
using ProtoBuf;

// Token: 0x0200038F RID: 911
internal interface IAIDesign
{
	// Token: 0x06002039 RID: 8249
	void LoadAIDesign(ProtoBuf.AIDesign design, global::BasePlayer player);

	// Token: 0x0600203A RID: 8250
	void StopDesigning();

	// Token: 0x0600203B RID: 8251
	bool CanPlayerDesignAI(global::BasePlayer player);
}

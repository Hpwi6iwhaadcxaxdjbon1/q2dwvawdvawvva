using System;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class ConversationManager : MonoBehaviour
{
	// Token: 0x02000C35 RID: 3125
	public class Conversation : MonoBehaviour
	{
		// Token: 0x04004284 RID: 17028
		public ConversationData data;

		// Token: 0x04004285 RID: 17029
		public int currentSpeechNodeIndex;

		// Token: 0x04004286 RID: 17030
		public IConversationProvider provider;

		// Token: 0x06004E29 RID: 20009 RVA: 0x001A2101 File Offset: 0x001A0301
		public int GetSpeechNodeIndex(string name)
		{
			if (this.data == null)
			{
				return -1;
			}
			return this.data.GetSpeechNodeIndex(name);
		}
	}
}

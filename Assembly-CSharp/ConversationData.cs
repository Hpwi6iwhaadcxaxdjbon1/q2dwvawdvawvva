using System;
using UnityEngine;

// Token: 0x02000197 RID: 407
[CreateAssetMenu(fileName = "NewConversation", menuName = "Rust/ConversationData", order = 1)]
public class ConversationData : ScriptableObject
{
	// Token: 0x04001110 RID: 4368
	public string shortname;

	// Token: 0x04001111 RID: 4369
	public Translate.Phrase providerNameTranslated;

	// Token: 0x04001112 RID: 4370
	public ConversationData.SpeechNode[] speeches;

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001839 RID: 6201 RVA: 0x000B5B28 File Offset: 0x000B3D28
	public string providerName
	{
		get
		{
			return this.providerNameTranslated.translated;
		}
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x000B5B38 File Offset: 0x000B3D38
	public int GetSpeechNodeIndex(string speechShortName)
	{
		for (int i = 0; i < this.speeches.Length; i++)
		{
			if (this.speeches[i].shortname == speechShortName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x02000C32 RID: 3122
	[Serializable]
	public class ConversationCondition
	{
		// Token: 0x04004278 RID: 17016
		public ConversationData.ConversationCondition.ConditionType conditionType;

		// Token: 0x04004279 RID: 17017
		public uint conditionAmount;

		// Token: 0x0400427A RID: 17018
		public bool inverse;

		// Token: 0x0400427B RID: 17019
		public string failedSpeechNode;

		// Token: 0x06004E21 RID: 20001 RVA: 0x001A1FB0 File Offset: 0x001A01B0
		public bool Passes(BasePlayer player, IConversationProvider provider)
		{
			bool flag = false;
			if (this.conditionType == ConversationData.ConversationCondition.ConditionType.HASSCRAP)
			{
				flag = ((long)player.inventory.GetAmount(ItemManager.FindItemDefinition("scrap").itemid) >= (long)((ulong)this.conditionAmount));
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.HASHEALTH)
			{
				flag = (player.health >= this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.PROVIDERBUSY)
			{
				flag = provider.ProviderBusy();
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.MISSIONCOMPLETE)
			{
				flag = player.HasCompletedMission(this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.MISSIONATTEMPTED)
			{
				flag = player.HasAttemptedMission(this.conditionAmount);
			}
			else if (this.conditionType == ConversationData.ConversationCondition.ConditionType.CANACCEPT)
			{
				flag = player.CanAcceptMission(this.conditionAmount);
			}
			if (!this.inverse)
			{
				return flag;
			}
			return !flag;
		}

		// Token: 0x02000FC9 RID: 4041
		public enum ConditionType
		{
			// Token: 0x040050CF RID: 20687
			NONE,
			// Token: 0x040050D0 RID: 20688
			HASHEALTH,
			// Token: 0x040050D1 RID: 20689
			HASSCRAP,
			// Token: 0x040050D2 RID: 20690
			PROVIDERBUSY,
			// Token: 0x040050D3 RID: 20691
			MISSIONCOMPLETE,
			// Token: 0x040050D4 RID: 20692
			MISSIONATTEMPTED,
			// Token: 0x040050D5 RID: 20693
			CANACCEPT
		}
	}

	// Token: 0x02000C33 RID: 3123
	[Serializable]
	public class ResponseNode
	{
		// Token: 0x0400427C RID: 17020
		public Translate.Phrase responseTextLocalized;

		// Token: 0x0400427D RID: 17021
		public ConversationData.ConversationCondition[] conditions;

		// Token: 0x0400427E RID: 17022
		public string actionString;

		// Token: 0x0400427F RID: 17023
		public string resultingSpeechNode;

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06004E23 RID: 20003 RVA: 0x001A2077 File Offset: 0x001A0277
		public string responseText
		{
			get
			{
				return this.responseTextLocalized.translated;
			}
		}

		// Token: 0x06004E24 RID: 20004 RVA: 0x001A2084 File Offset: 0x001A0284
		public bool PassesConditions(BasePlayer player, IConversationProvider provider)
		{
			ConversationData.ConversationCondition[] array = this.conditions;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].Passes(player, provider))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004E25 RID: 20005 RVA: 0x001A20B8 File Offset: 0x001A02B8
		public string GetFailedSpeechNode(BasePlayer player, IConversationProvider provider)
		{
			foreach (ConversationData.ConversationCondition conversationCondition in this.conditions)
			{
				if (!conversationCondition.Passes(player, provider))
				{
					return conversationCondition.failedSpeechNode;
				}
			}
			return "";
		}
	}

	// Token: 0x02000C34 RID: 3124
	[Serializable]
	public class SpeechNode
	{
		// Token: 0x04004280 RID: 17024
		public string shortname;

		// Token: 0x04004281 RID: 17025
		public Translate.Phrase statementLocalized;

		// Token: 0x04004282 RID: 17026
		public ConversationData.ResponseNode[] responses;

		// Token: 0x04004283 RID: 17027
		public Vector2 nodePosition;

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06004E27 RID: 20007 RVA: 0x001A20F4 File Offset: 0x001A02F4
		public string statement
		{
			get
			{
				return this.statementLocalized.translated;
			}
		}
	}
}

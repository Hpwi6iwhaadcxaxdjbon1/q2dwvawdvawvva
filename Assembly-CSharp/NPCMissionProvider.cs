using System;
using UnityEngine;

// Token: 0x0200019B RID: 411
public class NPCMissionProvider : NPCTalking, IMissionProvider
{
	// Token: 0x04001116 RID: 4374
	public MissionManifest manifest;

	// Token: 0x06001843 RID: 6211 RVA: 0x00050EF0 File Offset: 0x0004F0F0
	public NetworkableId ProviderID()
	{
		return this.net.ID;
	}

	// Token: 0x06001844 RID: 6212 RVA: 0x0002C673 File Offset: 0x0002A873
	public Vector3 ProviderPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity Entity()
	{
		return this;
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x000B5C28 File Offset: 0x000B3E28
	public override void OnConversationEnded(BasePlayer player)
	{
		player.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, this.ProviderID().Value.ToString(), 0f);
		base.OnConversationEnded(player);
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x000B5C5C File Offset: 0x000B3E5C
	public override void OnConversationStarted(BasePlayer speakingTo)
	{
		speakingTo.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, this.ProviderID().Value.ToString(), 1f);
		base.OnConversationStarted(speakingTo);
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x000B5C90 File Offset: 0x000B3E90
	public bool ContainsSpeech(string speech)
	{
		ConversationData[] conversations = this.conversations;
		for (int i = 0; i < conversations.Length; i++)
		{
			ConversationData.SpeechNode[] speeches = conversations[i].speeches;
			for (int j = 0; j < speeches.Length; j++)
			{
				if (speeches[j].shortname == speech)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x000B5CDC File Offset: 0x000B3EDC
	public string IntroOverride(string overrideSpeech)
	{
		if (!this.ContainsSpeech(overrideSpeech))
		{
			return "intro";
		}
		return overrideSpeech;
	}

	// Token: 0x0600184A RID: 6218 RVA: 0x000B5CF0 File Offset: 0x000B3EF0
	public override string GetConversationStartSpeech(BasePlayer player)
	{
		string text = "";
		foreach (BaseMission.MissionInstance missionInstance in player.missions)
		{
			if (missionInstance.status == BaseMission.MissionStatus.Active)
			{
				text = this.IntroOverride("missionactive");
			}
			if (missionInstance.status == BaseMission.MissionStatus.Completed && missionInstance.providerID == this.ProviderID() && Time.time - missionInstance.endTime < 5f)
			{
				text = this.IntroOverride("missionreturn");
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = base.GetConversationStartSpeech(player);
		}
		return text;
	}

	// Token: 0x0600184B RID: 6219 RVA: 0x000B5DA4 File Offset: 0x000B3FA4
	public override void OnConversationAction(BasePlayer player, string action)
	{
		if (action.Contains("assignmission"))
		{
			int num = action.IndexOf(" ");
			BaseMission fromShortName = MissionManifest.GetFromShortName(action.Substring(num + 1));
			if (fromShortName)
			{
				BaseMission.AssignMission(player, this, fromShortName);
			}
		}
		base.OnConversationAction(player, action);
	}
}

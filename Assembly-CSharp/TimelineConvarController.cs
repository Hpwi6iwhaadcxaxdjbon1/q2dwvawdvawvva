using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// Token: 0x0200076B RID: 1899
[Serializable]
public class TimelineConvarController : PlayableAsset, ITimelineClipAsset
{
	// Token: 0x04002AF7 RID: 10999
	public string convarName = string.Empty;

	// Token: 0x04002AF8 RID: 11000
	public TimelineConvarPlayable template = new TimelineConvarPlayable();

	// Token: 0x060034BF RID: 13503 RVA: 0x00146138 File Offset: 0x00144338
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		ScriptPlayable<TimelineConvarPlayable> playable = ScriptPlayable<TimelineConvarPlayable>.Create(graph, this.template, 0);
		playable.GetBehaviour().convar = this.convarName;
		return playable;
	}

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x060034C0 RID: 13504 RVA: 0x0004E73F File Offset: 0x0004C93F
	public ClipCaps clipCaps
	{
		get
		{
			return ClipCaps.Extrapolation;
		}
	}
}

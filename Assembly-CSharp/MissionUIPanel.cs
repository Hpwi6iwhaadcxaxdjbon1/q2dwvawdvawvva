using System;
using Rust.UI;
using UnityEngine;

// Token: 0x02000613 RID: 1555
public class MissionUIPanel : MonoBehaviour
{
	// Token: 0x04002596 RID: 9622
	public GameObject activeMissionParent;

	// Token: 0x04002597 RID: 9623
	public RustText missionTitleText;

	// Token: 0x04002598 RID: 9624
	public RustText missionDescText;

	// Token: 0x04002599 RID: 9625
	public VirtualItemIcon[] rewardIcons;

	// Token: 0x0400259A RID: 9626
	public Translate.Phrase noMissionText;
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020008AC RID: 2220
public class ProgressBar : UIBehaviour
{
	// Token: 0x040031D6 RID: 12758
	public static ProgressBar Instance;

	// Token: 0x040031D7 RID: 12759
	private Action<BasePlayer> action;

	// Token: 0x040031D8 RID: 12760
	public float timeFinished;

	// Token: 0x040031D9 RID: 12761
	private float timeCounter;

	// Token: 0x040031DA RID: 12762
	public GameObject scaleTarget;

	// Token: 0x040031DB RID: 12763
	public Image progressField;

	// Token: 0x040031DC RID: 12764
	public Image iconField;

	// Token: 0x040031DD RID: 12765
	public Text leftField;

	// Token: 0x040031DE RID: 12766
	public Text rightField;

	// Token: 0x040031DF RID: 12767
	public SoundDefinition clipOpen;

	// Token: 0x040031E0 RID: 12768
	public SoundDefinition clipCancel;

	// Token: 0x040031E1 RID: 12769
	private bool isOpen;

	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x06003716 RID: 14102 RVA: 0x0014C836 File Offset: 0x0014AA36
	public bool InstanceIsOpen
	{
		get
		{
			if (ProgressBar.Instance == this)
			{
				return this.isOpen;
			}
			return ProgressBar.Instance.InstanceIsOpen;
		}
	}
}

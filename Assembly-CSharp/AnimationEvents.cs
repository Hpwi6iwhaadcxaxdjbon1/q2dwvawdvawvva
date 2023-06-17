using System;
using UnityEngine;

// Token: 0x0200032F RID: 815
public class AnimationEvents : BaseMonoBehaviour
{
	// Token: 0x04001808 RID: 6152
	public Transform rootObject;

	// Token: 0x04001809 RID: 6153
	public HeldEntity targetEntity;

	// Token: 0x0400180A RID: 6154
	[Tooltip("Path to the effect folder for these animations. Relative to this object.")]
	public string effectFolder;

	// Token: 0x0400180B RID: 6155
	public bool enforceClipWeights;

	// Token: 0x0400180C RID: 6156
	public string localFolder;

	// Token: 0x0400180D RID: 6157
	[Tooltip("If true the localFolder field won't update with manifest updates, use for custom paths")]
	public bool customLocalFolder;

	// Token: 0x0400180E RID: 6158
	public bool IsBusy;

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000D2972 File Offset: 0x000D0B72
	protected void OnEnable()
	{
		if (this.rootObject == null)
		{
			this.rootObject = base.transform;
		}
	}
}

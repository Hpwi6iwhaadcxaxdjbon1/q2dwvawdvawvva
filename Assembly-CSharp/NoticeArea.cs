using System;
using UnityEngine;

// Token: 0x0200083F RID: 2111
public class NoticeArea : SingletonComponent<NoticeArea>
{
	// Token: 0x04002F34 RID: 12084
	public GameObjectRef itemPickupPrefab;

	// Token: 0x04002F35 RID: 12085
	public GameObjectRef itemPickupCondensedText;

	// Token: 0x04002F36 RID: 12086
	public GameObjectRef itemDroppedPrefab;

	// Token: 0x04002F37 RID: 12087
	public AnimationCurve pickupSizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F38 RID: 12088
	public AnimationCurve pickupAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F39 RID: 12089
	public AnimationCurve reuseAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F3A RID: 12090
	public AnimationCurve reuseSizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002F3B RID: 12091
	private IVitalNotice[] notices;

	// Token: 0x06003606 RID: 13830 RVA: 0x001485E6 File Offset: 0x001467E6
	protected override void Awake()
	{
		base.Awake();
		this.notices = base.GetComponentsInChildren<IVitalNotice>(true);
	}
}

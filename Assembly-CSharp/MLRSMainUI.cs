using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

// Token: 0x02000487 RID: 1159
public class MLRSMainUI : MonoBehaviour
{
	// Token: 0x04001E73 RID: 7795
	[SerializeField]
	private bool isFullscreen;

	// Token: 0x04001E74 RID: 7796
	[SerializeField]
	private GameObject noAimingModuleModeGO;

	// Token: 0x04001E75 RID: 7797
	[SerializeField]
	private GameObject activeModeGO;

	// Token: 0x04001E76 RID: 7798
	[SerializeField]
	private MLRSAmmoUI noAimingModuleAmmoUI;

	// Token: 0x04001E77 RID: 7799
	[SerializeField]
	private MLRSAmmoUI activeAmmoUI;

	// Token: 0x04001E78 RID: 7800
	[SerializeField]
	private MLRSVelocityUI velocityUI;

	// Token: 0x04001E79 RID: 7801
	[SerializeField]
	private RustText titleText;

	// Token: 0x04001E7A RID: 7802
	[SerializeField]
	private RustText usernameText;

	// Token: 0x04001E7B RID: 7803
	[SerializeField]
	private TokenisedPhrase readyStatus;

	// Token: 0x04001E7C RID: 7804
	[SerializeField]
	private TokenisedPhrase realigningStatus;

	// Token: 0x04001E7D RID: 7805
	[SerializeField]
	private TokenisedPhrase firingStatus;

	// Token: 0x04001E7E RID: 7806
	[SerializeField]
	private RustText statusText;

	// Token: 0x04001E7F RID: 7807
	[SerializeField]
	private MapView mapView;

	// Token: 0x04001E80 RID: 7808
	[SerializeField]
	private ScrollRectEx mapScrollRect;

	// Token: 0x04001E81 RID: 7809
	[SerializeField]
	private ScrollRectZoom mapScrollRectZoom;

	// Token: 0x04001E82 RID: 7810
	[SerializeField]
	private RectTransform mapBaseRect;

	// Token: 0x04001E83 RID: 7811
	[SerializeField]
	private RectTransform minRangeCircle;

	// Token: 0x04001E84 RID: 7812
	[SerializeField]
	private RectTransform targetAimRect;

	// Token: 0x04001E85 RID: 7813
	[SerializeField]
	private RectTransform trueAimRect;

	// Token: 0x04001E86 RID: 7814
	[SerializeField]
	private UILineRenderer connectingLine;

	// Token: 0x04001E87 RID: 7815
	[SerializeField]
	private GameObject noTargetCirclePrefab;

	// Token: 0x04001E88 RID: 7816
	[SerializeField]
	private Transform noTargetCircleParent;

	// Token: 0x04001E89 RID: 7817
	[SerializeField]
	private SoundDefinition changeTargetSoundDef;

	// Token: 0x04001E8A RID: 7818
	[SerializeField]
	private SoundDefinition readyToFireSoundDef;
}

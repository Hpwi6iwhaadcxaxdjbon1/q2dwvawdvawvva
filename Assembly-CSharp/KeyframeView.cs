using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007B5 RID: 1973
public class KeyframeView : MonoBehaviour
{
	// Token: 0x04002BA1 RID: 11169
	public ScrollRect Scroller;

	// Token: 0x04002BA2 RID: 11170
	public GameObjectRef KeyframePrefab;

	// Token: 0x04002BA3 RID: 11171
	public RectTransform KeyframeRoot;

	// Token: 0x04002BA4 RID: 11172
	public Transform CurrentPositionIndicator;

	// Token: 0x04002BA5 RID: 11173
	public bool LockScrollToCurrentPosition;

	// Token: 0x04002BA6 RID: 11174
	public RustText TrackName;
}

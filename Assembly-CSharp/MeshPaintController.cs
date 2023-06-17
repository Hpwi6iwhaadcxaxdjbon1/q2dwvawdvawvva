using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002CC RID: 716
public class MeshPaintController : MonoBehaviour, IClientComponent
{
	// Token: 0x0400168C RID: 5772
	public Camera pickerCamera;

	// Token: 0x0400168D RID: 5773
	public Texture2D brushTexture;

	// Token: 0x0400168E RID: 5774
	public Vector2 brushScale = new Vector2(8f, 8f);

	// Token: 0x0400168F RID: 5775
	public Color brushColor = Color.white;

	// Token: 0x04001690 RID: 5776
	public float brushSpacing = 2f;

	// Token: 0x04001691 RID: 5777
	public RawImage brushImage;

	// Token: 0x04001692 RID: 5778
	public float brushPreviewScaleMultiplier = 1f;

	// Token: 0x04001693 RID: 5779
	public bool applyDefaults;

	// Token: 0x04001694 RID: 5780
	public Texture2D defaltBrushTexture;

	// Token: 0x04001695 RID: 5781
	public float defaultBrushSize = 16f;

	// Token: 0x04001696 RID: 5782
	public Color defaultBrushColor = Color.black;

	// Token: 0x04001697 RID: 5783
	public float defaultBrushAlpha = 0.5f;

	// Token: 0x04001698 RID: 5784
	public Toggle lastBrush;

	// Token: 0x04001699 RID: 5785
	public Button UndoButton;

	// Token: 0x0400169A RID: 5786
	public Button RedoButton;

	// Token: 0x0400169B RID: 5787
	private Vector3 lastPosition;
}

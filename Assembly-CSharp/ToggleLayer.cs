using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000881 RID: 2177
public class ToggleLayer : MonoBehaviour, IClientComponent
{
	// Token: 0x040030F7 RID: 12535
	public Toggle toggleControl;

	// Token: 0x040030F8 RID: 12536
	public TextMeshProUGUI textControl;

	// Token: 0x040030F9 RID: 12537
	public LayerSelect layer;

	// Token: 0x06003690 RID: 13968 RVA: 0x0014A354 File Offset: 0x00148554
	protected void OnEnable()
	{
		if (MainCamera.mainCamera)
		{
			this.toggleControl.isOn = ((MainCamera.mainCamera.cullingMask & this.layer.Mask) != 0);
		}
	}

	// Token: 0x06003691 RID: 13969 RVA: 0x0014A388 File Offset: 0x00148588
	public void OnToggleChanged()
	{
		if (MainCamera.mainCamera)
		{
			if (this.toggleControl.isOn)
			{
				MainCamera.mainCamera.cullingMask |= this.layer.Mask;
				return;
			}
			MainCamera.mainCamera.cullingMask &= ~this.layer.Mask;
		}
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x0014A3E8 File Offset: 0x001485E8
	protected void OnValidate()
	{
		if (this.textControl)
		{
			this.textControl.text = this.layer.Name;
		}
	}
}

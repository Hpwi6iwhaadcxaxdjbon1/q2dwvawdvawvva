using System;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class StatusLightRenderer : MonoBehaviour, IClientComponent
{
	// Token: 0x04001751 RID: 5969
	public Material offMaterial;

	// Token: 0x04001752 RID: 5970
	public Material onMaterial;

	// Token: 0x04001753 RID: 5971
	private MaterialPropertyBlock propertyBlock;

	// Token: 0x04001754 RID: 5972
	private Renderer targetRenderer;

	// Token: 0x04001755 RID: 5973
	private Color lightColor;

	// Token: 0x04001756 RID: 5974
	private Light targetLight;

	// Token: 0x04001757 RID: 5975
	private int colorID;

	// Token: 0x04001758 RID: 5976
	private int emissionID;

	// Token: 0x06001DF1 RID: 7665 RVA: 0x000CC81C File Offset: 0x000CAA1C
	protected void Awake()
	{
		this.propertyBlock = new MaterialPropertyBlock();
		this.targetRenderer = base.GetComponent<Renderer>();
		this.targetLight = base.GetComponent<Light>();
		this.colorID = Shader.PropertyToID("_Color");
		this.emissionID = Shader.PropertyToID("_EmissionColor");
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x000CC86C File Offset: 0x000CAA6C
	public void SetOff()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.offMaterial;
			this.targetRenderer.SetPropertyBlock(null);
		}
		if (this.targetLight)
		{
			this.targetLight.color = Color.clear;
		}
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x000CC8C0 File Offset: 0x000CAAC0
	public void SetOn()
	{
		if (this.targetRenderer)
		{
			this.targetRenderer.sharedMaterial = this.onMaterial;
			this.targetRenderer.SetPropertyBlock(this.propertyBlock);
		}
		if (this.targetLight)
		{
			this.targetLight.color = this.lightColor;
		}
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x000CC91C File Offset: 0x000CAB1C
	public void SetRed()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(197, 46, 0, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(191, 0, 2, byte.MaxValue, 2.916925f));
		this.lightColor = this.GetColor(byte.MaxValue, 111, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x000CC9A0 File Offset: 0x000CABA0
	public void SetGreen()
	{
		this.propertyBlock.Clear();
		this.propertyBlock.SetColor(this.colorID, this.GetColor(19, 191, 13, byte.MaxValue));
		this.propertyBlock.SetColor(this.emissionID, this.GetColor(19, 191, 13, byte.MaxValue, 2.5f));
		this.lightColor = this.GetColor(156, byte.MaxValue, 102, byte.MaxValue);
		this.SetOn();
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x000CCA2A File Offset: 0x000CAC2A
	private Color GetColor(byte r, byte g, byte b, byte a)
	{
		return new Color32(r, g, b, a);
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x000CCA3B File Offset: 0x000CAC3B
	private Color GetColor(byte r, byte g, byte b, byte a, float intensity)
	{
		return new Color32(r, g, b, a) * intensity;
	}
}

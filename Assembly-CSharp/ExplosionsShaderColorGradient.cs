using System;
using UnityEngine;

// Token: 0x02000996 RID: 2454
public class ExplosionsShaderColorGradient : MonoBehaviour
{
	// Token: 0x040034AC RID: 13484
	public string ShaderProperty = "_TintColor";

	// Token: 0x040034AD RID: 13485
	public int MaterialID;

	// Token: 0x040034AE RID: 13486
	public Gradient Color = new Gradient();

	// Token: 0x040034AF RID: 13487
	public float TimeMultiplier = 1f;

	// Token: 0x040034B0 RID: 13488
	private bool canUpdate;

	// Token: 0x040034B1 RID: 13489
	private Material matInstance;

	// Token: 0x040034B2 RID: 13490
	private int propertyID;

	// Token: 0x040034B3 RID: 13491
	private float startTime;

	// Token: 0x040034B4 RID: 13492
	private Color oldColor;

	// Token: 0x06003A5D RID: 14941 RVA: 0x00159480 File Offset: 0x00157680
	private void Start()
	{
		Material[] materials = base.GetComponent<Renderer>().materials;
		if (this.MaterialID >= materials.Length)
		{
			Debug.Log("ShaderColorGradient: Material ID more than shader materials count.");
		}
		this.matInstance = materials[this.MaterialID];
		if (!this.matInstance.HasProperty(this.ShaderProperty))
		{
			Debug.Log("ShaderColorGradient: Shader not have \"" + this.ShaderProperty + "\" property");
		}
		this.propertyID = Shader.PropertyToID(this.ShaderProperty);
		this.oldColor = this.matInstance.GetColor(this.propertyID);
	}

	// Token: 0x06003A5E RID: 14942 RVA: 0x00159511 File Offset: 0x00157711
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06003A5F RID: 14943 RVA: 0x00159528 File Offset: 0x00157728
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			Color a = this.Color.Evaluate(num / this.TimeMultiplier);
			this.matInstance.SetColor(this.propertyID, a * this.oldColor);
		}
		if (num >= this.TimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}

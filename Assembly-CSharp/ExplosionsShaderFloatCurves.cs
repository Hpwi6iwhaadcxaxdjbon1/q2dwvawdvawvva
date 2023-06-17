using System;
using UnityEngine;

// Token: 0x02000997 RID: 2455
public class ExplosionsShaderFloatCurves : MonoBehaviour
{
	// Token: 0x040034B5 RID: 13493
	public string ShaderProperty = "_BumpAmt";

	// Token: 0x040034B6 RID: 13494
	public int MaterialID;

	// Token: 0x040034B7 RID: 13495
	public AnimationCurve FloatPropertyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040034B8 RID: 13496
	public float GraphTimeMultiplier = 1f;

	// Token: 0x040034B9 RID: 13497
	public float GraphScaleMultiplier = 1f;

	// Token: 0x040034BA RID: 13498
	private bool canUpdate;

	// Token: 0x040034BB RID: 13499
	private Material matInstance;

	// Token: 0x040034BC RID: 13500
	private int propertyID;

	// Token: 0x040034BD RID: 13501
	private float startTime;

	// Token: 0x06003A61 RID: 14945 RVA: 0x001595B4 File Offset: 0x001577B4
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
	}

	// Token: 0x06003A62 RID: 14946 RVA: 0x0015962E File Offset: 0x0015782E
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.canUpdate = true;
	}

	// Token: 0x06003A63 RID: 14947 RVA: 0x00159644 File Offset: 0x00157844
	private void Update()
	{
		float num = Time.time - this.startTime;
		if (this.canUpdate)
		{
			float value = this.FloatPropertyCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphScaleMultiplier;
			this.matInstance.SetFloat(this.propertyID, value);
		}
		if (num >= this.GraphTimeMultiplier)
		{
			this.canUpdate = false;
		}
	}
}

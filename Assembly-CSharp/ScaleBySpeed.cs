using System;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class ScaleBySpeed : MonoBehaviour
{
	// Token: 0x04001734 RID: 5940
	public float minScale = 0.001f;

	// Token: 0x04001735 RID: 5941
	public float maxScale = 1f;

	// Token: 0x04001736 RID: 5942
	public float minSpeed;

	// Token: 0x04001737 RID: 5943
	public float maxSpeed = 1f;

	// Token: 0x04001738 RID: 5944
	public MonoBehaviour component;

	// Token: 0x04001739 RID: 5945
	public bool toggleComponent = true;

	// Token: 0x0400173A RID: 5946
	public bool onlyWhenSubmerged;

	// Token: 0x0400173B RID: 5947
	public float submergedThickness = 0.33f;

	// Token: 0x0400173C RID: 5948
	private Vector3 prevPosition = Vector3.zero;

	// Token: 0x06001DE9 RID: 7657 RVA: 0x000CC66F File Offset: 0x000CA86F
	private void Start()
	{
		this.prevPosition = base.transform.position;
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x000CC684 File Offset: 0x000CA884
	private void Update()
	{
		Vector3 position = base.transform.position;
		float num = (position - this.prevPosition).sqrMagnitude;
		float num2 = this.minScale;
		bool enabled = WaterSystem.GetHeight(position) > position.y - this.submergedThickness;
		if (num > 0.0001f)
		{
			num = Mathf.Sqrt(num);
			float value = Mathf.Clamp(num, this.minSpeed, this.maxSpeed) / (this.maxSpeed - this.minSpeed);
			num2 = Mathf.Lerp(this.minScale, this.maxScale, Mathf.Clamp01(value));
			if (this.component != null && this.toggleComponent)
			{
				this.component.enabled = enabled;
			}
		}
		else if (this.component != null && this.toggleComponent)
		{
			this.component.enabled = false;
		}
		base.transform.localScale = new Vector3(num2, num2, num2);
		this.prevPosition = position;
	}
}

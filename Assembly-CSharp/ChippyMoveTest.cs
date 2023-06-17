using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class ChippyMoveTest : MonoBehaviour
{
	// Token: 0x04000FB5 RID: 4021
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	// Token: 0x04000FB6 RID: 4022
	public float speed = 0.2f;

	// Token: 0x04000FB7 RID: 4023
	public float maxSpeed = 1f;

	// Token: 0x0600170F RID: 5903 RVA: 0x000B006C File Offset: 0x000AE26C
	private void FixedUpdate()
	{
		float num = (Mathf.Abs(this.heading.magnitude) > 0f) ? 1f : 0f;
		this.speed = Mathf.MoveTowards(this.speed, this.maxSpeed * num, Time.fixedDeltaTime * ((num == 0f) ? 2f : 2f));
		Ray ray = new Ray(base.transform.position, new Vector3(this.heading.x, this.heading.y, 0f).normalized);
		if (!Physics.Raycast(ray, this.speed * Time.fixedDeltaTime, 16777216))
		{
			base.transform.position += ray.direction * Time.fixedDeltaTime * this.speed;
			if (Mathf.Abs(this.heading.magnitude) > 0f)
			{
				base.transform.rotation = QuaternionEx.LookRotationForcedUp(base.transform.forward, new Vector3(this.heading.x, this.heading.y, 0f).normalized);
			}
		}
	}
}

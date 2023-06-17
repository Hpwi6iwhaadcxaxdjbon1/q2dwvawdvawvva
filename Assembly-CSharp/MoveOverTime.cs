using System;
using UnityEngine;

// Token: 0x0200061C RID: 1564
public class MoveOverTime : MonoBehaviour
{
	// Token: 0x040025B3 RID: 9651
	[Range(-10f, 10f)]
	public float speed = 1f;

	// Token: 0x040025B4 RID: 9652
	public Vector3 position;

	// Token: 0x040025B5 RID: 9653
	public Vector3 rotation;

	// Token: 0x040025B6 RID: 9654
	public Vector3 scale;

	// Token: 0x06002E21 RID: 11809 RVA: 0x00114CEC File Offset: 0x00112EEC
	private void Update()
	{
		base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles + this.rotation * this.speed * Time.deltaTime);
		base.transform.localScale += this.scale * this.speed * Time.deltaTime;
		base.transform.localPosition += this.position * this.speed * Time.deltaTime;
	}
}

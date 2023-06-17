using System;
using UnityEngine;

// Token: 0x0200030E RID: 782
public class RotateObject : MonoBehaviour
{
	// Token: 0x040017B3 RID: 6067
	public float rotateSpeed_X = 1f;

	// Token: 0x040017B4 RID: 6068
	public float rotateSpeed_Y = 1f;

	// Token: 0x040017B5 RID: 6069
	public float rotateSpeed_Z = 1f;

	// Token: 0x040017B6 RID: 6070
	public bool localSpace;

	// Token: 0x040017B7 RID: 6071
	private Vector3 rotateVector;

	// Token: 0x06001EA7 RID: 7847 RVA: 0x000D0D64 File Offset: 0x000CEF64
	private void Awake()
	{
		this.rotateVector = new Vector3(this.rotateSpeed_X, this.rotateSpeed_Y, this.rotateSpeed_Z);
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x000D0D84 File Offset: 0x000CEF84
	private void Update()
	{
		if (this.localSpace)
		{
			base.transform.Rotate(this.rotateVector * Time.deltaTime, Space.Self);
			return;
		}
		if (this.rotateSpeed_X != 0f)
		{
			base.transform.Rotate(Vector3.up, Time.deltaTime * this.rotateSpeed_X);
		}
		if (this.rotateSpeed_Y != 0f)
		{
			base.transform.Rotate(base.transform.forward, Time.deltaTime * this.rotateSpeed_Y);
		}
		if (this.rotateSpeed_Z != 0f)
		{
			base.transform.Rotate(base.transform.right, Time.deltaTime * this.rotateSpeed_Z);
		}
	}
}

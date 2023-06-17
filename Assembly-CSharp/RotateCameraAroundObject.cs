using System;
using UnityEngine;

// Token: 0x02000914 RID: 2324
public class RotateCameraAroundObject : MonoBehaviour
{
	// Token: 0x0400332F RID: 13103
	public GameObject m_goObjectToRotateAround;

	// Token: 0x04003330 RID: 13104
	public float m_flRotateSpeed = 10f;

	// Token: 0x06003827 RID: 14375 RVA: 0x0014F194 File Offset: 0x0014D394
	private void FixedUpdate()
	{
		if (this.m_goObjectToRotateAround != null)
		{
			base.transform.LookAt(this.m_goObjectToRotateAround.transform.position + Vector3.up * 0.75f);
			base.transform.Translate(Vector3.right * this.m_flRotateSpeed * Time.deltaTime);
		}
	}
}

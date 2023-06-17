using System;
using UnityEngine;

// Token: 0x0200096E RID: 2414
public class RagdollEditor : SingletonComponent<RagdollEditor>
{
	// Token: 0x040033F0 RID: 13296
	private Vector3 view;

	// Token: 0x040033F1 RID: 13297
	private Rigidbody grabbedRigid;

	// Token: 0x040033F2 RID: 13298
	private Vector3 grabPos;

	// Token: 0x040033F3 RID: 13299
	private Vector3 grabOffset;

	// Token: 0x060039CF RID: 14799 RVA: 0x00156FC5 File Offset: 0x001551C5
	private void OnGUI()
	{
		GUI.Box(new Rect((float)Screen.width * 0.5f - 2f, (float)Screen.height * 0.5f - 2f, 4f, 4f), "");
	}

	// Token: 0x060039D0 RID: 14800 RVA: 0x00157004 File Offset: 0x00155204
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060039D1 RID: 14801 RVA: 0x0015700C File Offset: 0x0015520C
	private void Update()
	{
		Camera.main.fieldOfView = 75f;
		if (Input.GetKey(KeyCode.Mouse1))
		{
			this.view.y = this.view.y + Input.GetAxisRaw("Mouse X") * 3f;
			this.view.x = this.view.x - Input.GetAxisRaw("Mouse Y") * 3f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		Camera.main.transform.rotation = Quaternion.Euler(this.view);
		Vector3 vector = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vector += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += Vector3.right;
		}
		Camera.main.transform.position += base.transform.rotation * vector * 0.05f;
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.StartGrab();
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			this.StopGrab();
		}
	}

	// Token: 0x060039D2 RID: 14802 RVA: 0x00157159 File Offset: 0x00155359
	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			this.UpdateGrab();
		}
	}

	// Token: 0x060039D3 RID: 14803 RVA: 0x00157170 File Offset: 0x00155370
	private void StartGrab()
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, 100f))
		{
			return;
		}
		this.grabbedRigid = raycastHit.collider.GetComponent<Rigidbody>();
		if (this.grabbedRigid == null)
		{
			return;
		}
		this.grabPos = this.grabbedRigid.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
		this.grabOffset = base.transform.worldToLocalMatrix.MultiplyPoint(raycastHit.point);
	}

	// Token: 0x060039D4 RID: 14804 RVA: 0x00157208 File Offset: 0x00155408
	private void UpdateGrab()
	{
		if (this.grabbedRigid == null)
		{
			return;
		}
		Vector3 a = base.transform.TransformPoint(this.grabOffset);
		Vector3 vector = this.grabbedRigid.transform.TransformPoint(this.grabPos);
		Vector3 a2 = a - vector;
		this.grabbedRigid.AddForceAtPosition(a2 * 100f, vector, ForceMode.Acceleration);
	}

	// Token: 0x060039D5 RID: 14805 RVA: 0x0015726B File Offset: 0x0015546B
	private void StopGrab()
	{
		this.grabbedRigid = null;
	}
}

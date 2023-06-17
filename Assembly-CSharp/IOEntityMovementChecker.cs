using System;
using UnityEngine;

// Token: 0x020004D1 RID: 1233
[RequireComponent(typeof(IOEntity))]
public class IOEntityMovementChecker : FacepunchBehaviour
{
	// Token: 0x04002046 RID: 8262
	private IOEntity ioEntity;

	// Token: 0x04002047 RID: 8263
	private Vector3 prevPos;

	// Token: 0x04002048 RID: 8264
	private const float MAX_MOVE = 0.05f;

	// Token: 0x04002049 RID: 8265
	private const float MAX_MOVE_SQR = 0.0025000002f;

	// Token: 0x06002818 RID: 10264 RVA: 0x000F889D File Offset: 0x000F6A9D
	protected void Awake()
	{
		this.ioEntity = base.GetComponent<IOEntity>();
	}

	// Token: 0x06002819 RID: 10265 RVA: 0x000F88AB File Offset: 0x000F6AAB
	protected void OnEnable()
	{
		base.InvokeRepeating(new Action(this.CheckPosition), UnityEngine.Random.Range(0f, 0.25f), 0.25f);
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x000F88D3 File Offset: 0x000F6AD3
	protected void OnDisable()
	{
		base.CancelInvoke(new Action(this.CheckPosition));
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000F88E8 File Offset: 0x000F6AE8
	private void CheckPosition()
	{
		if (this.ioEntity.isClient)
		{
			return;
		}
		if (Vector3.SqrMagnitude(base.transform.position - this.prevPos) > 0.0025000002f)
		{
			this.prevPos = base.transform.position;
			if (this.ioEntity.HasConnections())
			{
				this.ioEntity.SendChangedToRoot(true);
				this.ioEntity.ClearConnections();
			}
		}
	}
}

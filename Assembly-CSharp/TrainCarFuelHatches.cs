using System;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class TrainCarFuelHatches : MonoBehaviour
{
	// Token: 0x04001FB2 RID: 8114
	[SerializeField]
	private TrainCar owner;

	// Token: 0x04001FB3 RID: 8115
	[SerializeField]
	private float animSpeed = 1f;

	// Token: 0x04001FB4 RID: 8116
	[SerializeField]
	private Transform hatch1Col;

	// Token: 0x04001FB5 RID: 8117
	[SerializeField]
	private Transform hatch1Vis;

	// Token: 0x04001FB6 RID: 8118
	[SerializeField]
	private Transform hatch2Col;

	// Token: 0x04001FB7 RID: 8119
	[SerializeField]
	private Transform hatch2Vis;

	// Token: 0x04001FB8 RID: 8120
	[SerializeField]
	private Transform hatch3Col;

	// Token: 0x04001FB9 RID: 8121
	[SerializeField]
	private Transform hatch3Vis;

	// Token: 0x04001FBA RID: 8122
	private const float closedXAngle = 0f;

	// Token: 0x04001FBB RID: 8123
	private const float openXAngle = -145f;

	// Token: 0x04001FBC RID: 8124
	[SerializeField]
	private SoundDefinition hatchOpenSoundDef;

	// Token: 0x04001FBD RID: 8125
	[SerializeField]
	private SoundDefinition hatchCloseSoundDef;

	// Token: 0x04001FBE RID: 8126
	private Vector3 _angles = Vector3.zero;

	// Token: 0x04001FBF RID: 8127
	private float _hatchLerp;

	// Token: 0x04001FC0 RID: 8128
	private bool opening;

	// Token: 0x04001FC1 RID: 8129
	private bool openingQueued;

	// Token: 0x04001FC2 RID: 8130
	private bool isMoving;

	// Token: 0x0600273A RID: 10042 RVA: 0x000F5200 File Offset: 0x000F3400
	public void LinedUpStateChanged(bool linedUp)
	{
		this.openingQueued = linedUp;
		if (!this.isMoving)
		{
			this.opening = linedUp;
			bool flag = this.opening;
			this.isMoving = true;
			InvokeHandler.InvokeRepeating(this, new Action(this.MoveTick), 0f, 0f);
		}
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000F5250 File Offset: 0x000F3450
	private void MoveTick()
	{
		if (this.opening)
		{
			this._hatchLerp += Time.deltaTime * this.animSpeed;
			if (this._hatchLerp >= 1f)
			{
				this.EndMove();
				return;
			}
			this.SetAngleOnAll(this._hatchLerp, false);
			return;
		}
		else
		{
			this._hatchLerp += Time.deltaTime * this.animSpeed;
			if (this._hatchLerp >= 1f)
			{
				this.EndMove();
				return;
			}
			this.SetAngleOnAll(this._hatchLerp, true);
			return;
		}
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000F52DC File Offset: 0x000F34DC
	private void EndMove()
	{
		this._hatchLerp = 0f;
		if (this.openingQueued == this.opening)
		{
			InvokeHandler.CancelInvoke(this, new Action(this.MoveTick));
			this.isMoving = false;
			return;
		}
		this.opening = this.openingQueued;
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000F5328 File Offset: 0x000F3528
	private void SetAngleOnAll(float lerpT, bool closing)
	{
		float angle;
		float angle2;
		float angle3;
		if (closing)
		{
			angle = LeanTween.easeOutBounce(-145f, 0f, Mathf.Clamp01(this._hatchLerp * 1.15f));
			angle2 = LeanTween.easeOutBounce(-145f, 0f, this._hatchLerp);
			angle3 = LeanTween.easeOutBounce(-145f, 0f, Mathf.Clamp01(this._hatchLerp * 1.25f));
		}
		else
		{
			angle = LeanTween.easeOutBounce(0f, -145f, Mathf.Clamp01(this._hatchLerp * 1.15f));
			angle2 = LeanTween.easeOutBounce(0f, -145f, this._hatchLerp);
			angle3 = LeanTween.easeOutBounce(0f, -145f, Mathf.Clamp01(this._hatchLerp * 1.25f));
		}
		this.SetAngle(this.hatch1Col, angle);
		this.SetAngle(this.hatch2Col, angle2);
		this.SetAngle(this.hatch3Col, angle3);
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x000F5411 File Offset: 0x000F3611
	private void SetAngle(Transform transform, float angle)
	{
		this._angles.x = angle;
		transform.localEulerAngles = this._angles;
	}
}

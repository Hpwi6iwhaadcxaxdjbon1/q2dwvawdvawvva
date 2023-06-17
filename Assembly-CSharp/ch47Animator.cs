using System;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class ch47Animator : MonoBehaviour
{
	// Token: 0x0400112F RID: 4399
	public Animator animator;

	// Token: 0x04001130 RID: 4400
	public bool bottomDoorOpen;

	// Token: 0x04001131 RID: 4401
	public bool landingGearDown;

	// Token: 0x04001132 RID: 4402
	public bool leftDoorOpen;

	// Token: 0x04001133 RID: 4403
	public bool rightDoorOpen;

	// Token: 0x04001134 RID: 4404
	public bool rearDoorOpen;

	// Token: 0x04001135 RID: 4405
	public bool rearDoorExtensionOpen;

	// Token: 0x04001136 RID: 4406
	public Transform rearRotorBlade;

	// Token: 0x04001137 RID: 4407
	public Transform frontRotorBlade;

	// Token: 0x04001138 RID: 4408
	public float rotorBladeSpeed;

	// Token: 0x04001139 RID: 4409
	public float wheelTurnSpeed;

	// Token: 0x0400113A RID: 4410
	public float wheelTurnAngle;

	// Token: 0x0400113B RID: 4411
	public SkinnedMeshRenderer[] blurredRotorBlades;

	// Token: 0x0400113C RID: 4412
	public SkinnedMeshRenderer[] RotorBlades;

	// Token: 0x0400113D RID: 4413
	private bool blurredRotorBladesEnabled;

	// Token: 0x0400113E RID: 4414
	public float blurSpeedThreshold = 100f;

	// Token: 0x06001872 RID: 6258 RVA: 0x000B6D60 File Offset: 0x000B4F60
	private void Start()
	{
		this.EnableBlurredRotorBlades(false);
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x000B6D7A File Offset: 0x000B4F7A
	public void SetDropDoorOpen(bool isOpen)
	{
		this.bottomDoorOpen = isOpen;
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x000B6D84 File Offset: 0x000B4F84
	private void Update()
	{
		this.animator.SetBool("bottomdoor", this.bottomDoorOpen);
		this.animator.SetBool("landinggear", this.landingGearDown);
		this.animator.SetBool("leftdoor", this.leftDoorOpen);
		this.animator.SetBool("rightdoor", this.rightDoorOpen);
		this.animator.SetBool("reardoor", this.rearDoorOpen);
		this.animator.SetBool("reardoor_extension", this.rearDoorExtensionOpen);
		if (this.rotorBladeSpeed >= this.blurSpeedThreshold && !this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(true);
		}
		else if (this.rotorBladeSpeed < this.blurSpeedThreshold && this.blurredRotorBladesEnabled)
		{
			this.EnableBlurredRotorBlades(false);
		}
		if (this.rotorBladeSpeed <= 0f)
		{
			this.animator.SetBool("rotorblade_stop", true);
			return;
		}
		this.animator.SetBool("rotorblade_stop", false);
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x000B6E84 File Offset: 0x000B5084
	private void LateUpdate()
	{
		float num = Time.deltaTime * this.rotorBladeSpeed * 15f;
		Vector3 localEulerAngles = this.frontRotorBlade.localEulerAngles;
		this.frontRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y + num, localEulerAngles.z);
		localEulerAngles = this.rearRotorBlade.localEulerAngles;
		this.rearRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y - num, localEulerAngles.z);
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x000B6F04 File Offset: 0x000B5104
	private void EnableBlurredRotorBlades(bool enabled)
	{
		this.blurredRotorBladesEnabled = enabled;
		SkinnedMeshRenderer[] rotorBlades = this.blurredRotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = enabled;
		}
		rotorBlades = this.RotorBlades;
		for (int i = 0; i < rotorBlades.Length; i++)
		{
			rotorBlades[i].enabled = !enabled;
		}
	}
}

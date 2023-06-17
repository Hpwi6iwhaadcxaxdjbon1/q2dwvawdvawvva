using System;
using UnityEngine;

// Token: 0x02000417 RID: 1047
public class m2bradleyAnimator : MonoBehaviour
{
	// Token: 0x04001B64 RID: 7012
	public Animator m2Animator;

	// Token: 0x04001B65 RID: 7013
	public Material treadLeftMaterial;

	// Token: 0x04001B66 RID: 7014
	public Material treadRightMaterial;

	// Token: 0x04001B67 RID: 7015
	private Rigidbody mainRigidbody;

	// Token: 0x04001B68 RID: 7016
	[Header("GunBones")]
	public Transform turret;

	// Token: 0x04001B69 RID: 7017
	public Transform mainCannon;

	// Token: 0x04001B6A RID: 7018
	public Transform coaxGun;

	// Token: 0x04001B6B RID: 7019
	public Transform rocketsPitch;

	// Token: 0x04001B6C RID: 7020
	public Transform spotLightYaw;

	// Token: 0x04001B6D RID: 7021
	public Transform spotLightPitch;

	// Token: 0x04001B6E RID: 7022
	public Transform sideMG;

	// Token: 0x04001B6F RID: 7023
	public Transform[] sideguns;

	// Token: 0x04001B70 RID: 7024
	[Header("WheelBones")]
	public Transform[] ShocksBones;

	// Token: 0x04001B71 RID: 7025
	public Transform[] ShockTraceLineBegin;

	// Token: 0x04001B72 RID: 7026
	public Vector3[] vecShocksOffsetPosition;

	// Token: 0x04001B73 RID: 7027
	[Header("Targeting")]
	public Transform targetTurret;

	// Token: 0x04001B74 RID: 7028
	public Transform targetSpotLight;

	// Token: 0x04001B75 RID: 7029
	public Transform[] targetSideguns;

	// Token: 0x04001B76 RID: 7030
	private Vector3 vecTurret = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B77 RID: 7031
	private Vector3 vecMainCannon = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B78 RID: 7032
	private Vector3 vecCoaxGun = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B79 RID: 7033
	private Vector3 vecRocketsPitch = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B7A RID: 7034
	private Vector3 vecSpotLightBase = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B7B RID: 7035
	private Vector3 vecSpotLight = new Vector3(0f, 0f, 0f);

	// Token: 0x04001B7C RID: 7036
	private float sideMGPitchValue;

	// Token: 0x04001B7D RID: 7037
	[Header("MuzzleFlash locations")]
	public GameObject muzzleflashCannon;

	// Token: 0x04001B7E RID: 7038
	public GameObject muzzleflashCoaxGun;

	// Token: 0x04001B7F RID: 7039
	public GameObject muzzleflashSideMG;

	// Token: 0x04001B80 RID: 7040
	public GameObject[] muzzleflashRockets;

	// Token: 0x04001B81 RID: 7041
	public GameObject spotLightHaloSawnpoint;

	// Token: 0x04001B82 RID: 7042
	public GameObject[] muzzleflashSideguns;

	// Token: 0x04001B83 RID: 7043
	[Header("MuzzleFlash Particle Systems")]
	public GameObjectRef machineGunMuzzleFlashFX;

	// Token: 0x04001B84 RID: 7044
	public GameObjectRef mainCannonFireFX;

	// Token: 0x04001B85 RID: 7045
	public GameObjectRef rocketLaunchFX;

	// Token: 0x04001B86 RID: 7046
	[Header("Misc")]
	public bool rocketsOpen;

	// Token: 0x04001B87 RID: 7047
	public Vector3[] vecSideGunRotation;

	// Token: 0x04001B88 RID: 7048
	public float treadConstant = 0.14f;

	// Token: 0x04001B89 RID: 7049
	public float wheelSpinConstant = 80f;

	// Token: 0x04001B8A RID: 7050
	[Header("Gun Movement speeds")]
	public float sidegunsTurnSpeed = 30f;

	// Token: 0x04001B8B RID: 7051
	public float turretTurnSpeed = 6f;

	// Token: 0x04001B8C RID: 7052
	public float cannonPitchSpeed = 10f;

	// Token: 0x04001B8D RID: 7053
	public float rocketPitchSpeed = 20f;

	// Token: 0x04001B8E RID: 7054
	public float spotLightTurnSpeed = 60f;

	// Token: 0x04001B8F RID: 7055
	public float machineGunSpeed = 20f;

	// Token: 0x04001B90 RID: 7056
	private float wheelAngle;

	// Token: 0x06002355 RID: 9045 RVA: 0x000E191C File Offset: 0x000DFB1C
	private void Start()
	{
		this.mainRigidbody = base.GetComponent<Rigidbody>();
		for (int i = 0; i < this.ShocksBones.Length; i++)
		{
			this.vecShocksOffsetPosition[i] = this.ShocksBones[i].localPosition;
		}
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x000E1961 File Offset: 0x000DFB61
	private void Update()
	{
		this.TrackTurret();
		this.TrackSpotLight();
		this.TrackSideGuns();
		this.AnimateWheelsTreads();
		this.AdjustShocksHeight();
		this.m2Animator.SetBool("rocketpods", this.rocketsOpen);
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x000E1998 File Offset: 0x000DFB98
	private void AnimateWheelsTreads()
	{
		float num = 0f;
		if (this.mainRigidbody != null)
		{
			num = Vector3.Dot(this.mainRigidbody.velocity, base.transform.forward);
		}
		float x = Time.time * -1f * num * this.treadConstant % 1f;
		this.treadLeftMaterial.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.treadLeftMaterial.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.treadLeftMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_MainTex", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_BumpMap", new Vector2(x, 0f));
		this.treadRightMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(x, 0f));
		if (num >= 0f)
		{
			this.wheelAngle = (this.wheelAngle + Time.deltaTime * num * this.wheelSpinConstant) % 360f;
		}
		else
		{
			this.wheelAngle += Time.deltaTime * num * this.wheelSpinConstant;
			if (this.wheelAngle <= 0f)
			{
				this.wheelAngle = 360f;
			}
		}
		this.m2Animator.SetFloat("wheel_spin", this.wheelAngle);
		this.m2Animator.SetFloat("speed", num);
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000E1B18 File Offset: 0x000DFD18
	private void AdjustShocksHeight()
	{
		Ray ray = default(Ray);
		int mask = LayerMask.GetMask(new string[]
		{
			"Terrain",
			"World",
			"Construction"
		});
		int num = this.ShocksBones.Length;
		float num2 = 0.55f;
		float maxDistance = 0.79f;
		for (int i = 0; i < num; i++)
		{
			ray.origin = this.ShockTraceLineBegin[i].position;
			ray.direction = base.transform.up * -1f;
			RaycastHit raycastHit;
			float num3;
			if (Physics.SphereCast(ray, 0.15f, out raycastHit, maxDistance, mask))
			{
				num3 = raycastHit.distance - num2;
			}
			else
			{
				num3 = 0.26f;
			}
			this.vecShocksOffsetPosition[i].y = Mathf.Lerp(this.vecShocksOffsetPosition[i].y, Mathf.Clamp(num3 * -1f, -0.26f, 0f), Time.deltaTime * 5f);
			this.ShocksBones[i].localPosition = this.vecShocksOffsetPosition[i];
		}
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x000E1C44 File Offset: 0x000DFE44
	private void TrackTurret()
	{
		if (this.targetTurret != null)
		{
			Vector3 normalized = (this.targetTurret.position - this.turret.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.turret, this.turret.position, this.targetTurret.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.turretTurnSpeed;
			if (num < -0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecTurret.y = (this.vecTurret.y + num3) % 360f;
			}
			this.turret.localEulerAngles = this.vecTurret;
			float num4 = Time.deltaTime * this.cannonPitchSpeed;
			this.CalculateYawPitchOffset(this.mainCannon, this.mainCannon.position, this.targetTurret.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecMainCannon.x = this.vecMainCannon.x + num4;
			}
			this.vecMainCannon.x = Mathf.Clamp(this.vecMainCannon.x, -55f, 5f);
			this.mainCannon.localEulerAngles = this.vecMainCannon;
			if (num2 < -0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x - num4;
			}
			else if (num2 > 0.5f)
			{
				this.vecCoaxGun.x = this.vecCoaxGun.x + num4;
			}
			this.vecCoaxGun.x = Mathf.Clamp(this.vecCoaxGun.x, -65f, 15f);
			this.coaxGun.localEulerAngles = this.vecCoaxGun;
			if (this.rocketsOpen)
			{
				num4 = Time.deltaTime * this.rocketPitchSpeed;
				this.CalculateYawPitchOffset(this.rocketsPitch, this.rocketsPitch.position, this.targetTurret.position, out num, out num2);
				if (num2 < -0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x - num4;
				}
				else if (num2 > 0.5f)
				{
					this.vecRocketsPitch.x = this.vecRocketsPitch.x + num4;
				}
				this.vecRocketsPitch.x = Mathf.Clamp(this.vecRocketsPitch.x, -45f, 45f);
			}
			else
			{
				this.vecRocketsPitch.x = Mathf.Lerp(this.vecRocketsPitch.x, 0f, Time.deltaTime * 1.7f);
			}
			this.rocketsPitch.localEulerAngles = this.vecRocketsPitch;
		}
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000E1F18 File Offset: 0x000E0118
	private void TrackSpotLight()
	{
		if (this.targetSpotLight != null)
		{
			Vector3 normalized = (this.targetSpotLight.position - this.spotLightYaw.position).normalized;
			float num;
			float num2;
			this.CalculateYawPitchOffset(this.spotLightYaw, this.spotLightYaw.position, this.targetSpotLight.position, out num, out num2);
			num = this.NormalizeYaw(num);
			float num3 = Time.deltaTime * this.spotLightTurnSpeed;
			if (num < -0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y - num3) % 360f;
			}
			else if (num > 0.5f)
			{
				this.vecSpotLightBase.y = (this.vecSpotLightBase.y + num3) % 360f;
			}
			this.spotLightYaw.localEulerAngles = this.vecSpotLightBase;
			this.CalculateYawPitchOffset(this.spotLightPitch, this.spotLightPitch.position, this.targetSpotLight.position, out num, out num2);
			if (num2 < -0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x - num3;
			}
			else if (num2 > 0.5f)
			{
				this.vecSpotLight.x = this.vecSpotLight.x + num3;
			}
			this.vecSpotLight.x = Mathf.Clamp(this.vecSpotLight.x, -50f, 50f);
			this.spotLightPitch.localEulerAngles = this.vecSpotLight;
			this.m2Animator.SetFloat("sideMG_pitch", this.vecSpotLight.x, 0.5f, Time.deltaTime);
		}
	}

	// Token: 0x0600235B RID: 9051 RVA: 0x000E20A8 File Offset: 0x000E02A8
	private void TrackSideGuns()
	{
		for (int i = 0; i < this.sideguns.Length; i++)
		{
			if (!(this.targetSideguns[i] == null))
			{
				Vector3 normalized = (this.targetSideguns[i].position - this.sideguns[i].position).normalized;
				float num;
				float num2;
				this.CalculateYawPitchOffset(this.sideguns[i], this.sideguns[i].position, this.targetSideguns[i].position, out num, out num2);
				num = this.NormalizeYaw(num);
				float num3 = Time.deltaTime * this.sidegunsTurnSpeed;
				if (num < -0.5f)
				{
					Vector3[] array = this.vecSideGunRotation;
					int num4 = i;
					array[num4].y = array[num4].y - num3;
				}
				else if (num > 0.5f)
				{
					Vector3[] array2 = this.vecSideGunRotation;
					int num5 = i;
					array2[num5].y = array2[num5].y + num3;
				}
				if (num2 < -0.5f)
				{
					Vector3[] array3 = this.vecSideGunRotation;
					int num6 = i;
					array3[num6].x = array3[num6].x - num3;
				}
				else if (num2 > 0.5f)
				{
					Vector3[] array4 = this.vecSideGunRotation;
					int num7 = i;
					array4[num7].x = array4[num7].x + num3;
				}
				this.vecSideGunRotation[i].x = Mathf.Clamp(this.vecSideGunRotation[i].x, -45f, 45f);
				this.vecSideGunRotation[i].y = Mathf.Clamp(this.vecSideGunRotation[i].y, -45f, 45f);
				this.sideguns[i].localEulerAngles = this.vecSideGunRotation[i];
			}
		}
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x000E2244 File Offset: 0x000E0444
	public void CalculateYawPitchOffset(Transform objectTransform, Vector3 vecStart, Vector3 vecEnd, out float yaw, out float pitch)
	{
		Vector3 vector = objectTransform.InverseTransformDirection(vecEnd - vecStart);
		float x = Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
		pitch = -Mathf.Atan2(vector.y, x) * 57.295776f;
		vector = (vecEnd - vecStart).normalized;
		Vector3 forward = objectTransform.forward;
		forward.y = 0f;
		forward.Normalize();
		float num = Vector3.Dot(vector, forward);
		float num2 = Vector3.Dot(vector, objectTransform.right);
		float y = 360f * num2;
		float x2 = 360f * -num;
		yaw = (Mathf.Atan2(y, x2) + 3.1415927f) * 57.295776f;
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x000E2304 File Offset: 0x000E0504
	public float NormalizeYaw(float flYaw)
	{
		float result;
		if (flYaw > 180f)
		{
			result = 360f - flYaw;
		}
		else
		{
			result = flYaw * -1f;
		}
		return result;
	}
}

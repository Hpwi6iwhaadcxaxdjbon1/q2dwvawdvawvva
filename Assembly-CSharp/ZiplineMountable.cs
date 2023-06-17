using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class ZiplineMountable : BaseMountable
{
	// Token: 0x04000D96 RID: 3478
	public float MoveSpeed = 4f;

	// Token: 0x04000D97 RID: 3479
	public float ForwardAdditive = 5f;

	// Token: 0x04000D98 RID: 3480
	public CapsuleCollider ZipCollider;

	// Token: 0x04000D99 RID: 3481
	public Transform ZiplineGrabRoot;

	// Token: 0x04000D9A RID: 3482
	public Transform LeftHandIkPoint;

	// Token: 0x04000D9B RID: 3483
	public Transform RightHandIkPoint;

	// Token: 0x04000D9C RID: 3484
	public float SpeedUpTime = 0.6f;

	// Token: 0x04000D9D RID: 3485
	public bool EditorHoldInPlace;

	// Token: 0x04000D9E RID: 3486
	private List<Vector3> linePoints;

	// Token: 0x04000D9F RID: 3487
	private const global::BaseEntity.Flags PushForward = global::BaseEntity.Flags.Reserved1;

	// Token: 0x04000DA0 RID: 3488
	public AnimationCurve MountPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000DA1 RID: 3489
	public AnimationCurve MountRotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000DA2 RID: 3490
	public float MountEaseInTime = 0.5f;

	// Token: 0x04000DA3 RID: 3491
	private const global::BaseEntity.Flags ShowHandle = global::BaseEntity.Flags.Reserved2;

	// Token: 0x04000DA4 RID: 3492
	private float additiveValue;

	// Token: 0x04000DA5 RID: 3493
	private float currentTravelDistance;

	// Token: 0x04000DA6 RID: 3494
	private TimeSince mountTime;

	// Token: 0x04000DA7 RID: 3495
	private bool hasEnded;

	// Token: 0x04000DA8 RID: 3496
	private List<Collider> ignoreColliders = new List<Collider>();

	// Token: 0x04000DA9 RID: 3497
	private Vector3 lastSafePosition;

	// Token: 0x04000DAA RID: 3498
	private Vector3 startPosition = Vector3.zero;

	// Token: 0x04000DAB RID: 3499
	private Vector3 endPosition = Vector3.zero;

	// Token: 0x04000DAC RID: 3500
	private Quaternion startRotation = Quaternion.identity;

	// Token: 0x04000DAD RID: 3501
	private Quaternion endRotation = Quaternion.identity;

	// Token: 0x04000DAE RID: 3502
	private float elapsedMoveTime;

	// Token: 0x04000DAF RID: 3503
	private bool isAnimatingIn;

	// Token: 0x06001561 RID: 5473 RVA: 0x000A96A4 File Offset: 0x000A78A4
	private Vector3 ProcessBezierMovement(float distanceToTravel)
	{
		if (this.linePoints == null)
		{
			return Vector3.zero;
		}
		float num = 0f;
		for (int i = 0; i < this.linePoints.Count - 1; i++)
		{
			float num2 = Vector3.Distance(this.linePoints[i], this.linePoints[i + 1]);
			if (num + num2 > distanceToTravel)
			{
				float t = Mathf.Clamp((distanceToTravel - num) / num2, 0f, 1f);
				return Vector3.Lerp(this.linePoints[i], this.linePoints[i + 1], t);
			}
			num += num2;
		}
		return this.linePoints[this.linePoints.Count - 1];
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x000A9758 File Offset: 0x000A7958
	private Vector3 GetLineEndPoint(bool applyDismountOffset = false)
	{
		if (applyDismountOffset && this.linePoints != null)
		{
			Vector3 normalized = (this.linePoints[this.linePoints.Count - 2] - this.linePoints[this.linePoints.Count - 1]).normalized;
			return this.linePoints[this.linePoints.Count - 1] + normalized * 1.5f;
		}
		List<Vector3> list = this.linePoints;
		if (list == null)
		{
			return Vector3.zero;
		}
		return list[this.linePoints.Count - 1];
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x000A97FC File Offset: 0x000A79FC
	private Vector3 GetNextLinePoint(Transform forTransform)
	{
		Vector3 position = forTransform.position;
		Vector3 forward = forTransform.forward;
		for (int i = 1; i < this.linePoints.Count - 1; i++)
		{
			Vector3 normalized = (this.linePoints[i + 1] - position).normalized;
			Vector3 normalized2 = (this.linePoints[i - 1] - position).normalized;
			float num = Vector3.Dot(forward, normalized);
			float num2 = Vector3.Dot(forward, normalized2);
			if (num > 0f && num2 < 0f)
			{
				return this.linePoints[i + 1];
			}
		}
		return this.GetLineEndPoint(false);
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x000A98AA File Offset: 0x000A7AAA
	public override void ResetState()
	{
		base.ResetState();
		this.additiveValue = 0f;
		this.currentTravelDistance = 0f;
		this.hasEnded = false;
		this.linePoints = null;
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x000A98D6 File Offset: 0x000A7AD6
	public override float MaxVelocity()
	{
		return this.MoveSpeed + this.ForwardAdditive;
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x000A98E8 File Offset: 0x000A7AE8
	public void SetDestination(List<Vector3> targetLinePoints, Vector3 lineStartPos, Quaternion lineStartRot)
	{
		this.linePoints = targetLinePoints;
		this.currentTravelDistance = 0f;
		this.mountTime = 0f;
		GamePhysics.OverlapSphere(base.transform.position, 6f, this.ignoreColliders, 1218511105, QueryTriggerInteraction.Ignore);
		this.startPosition = base.transform.position;
		this.startRotation = base.transform.rotation;
		this.lastSafePosition = this.startPosition;
		this.endPosition = lineStartPos;
		this.endRotation = lineStartRot;
		this.elapsedMoveTime = 0f;
		this.isAnimatingIn = true;
		base.InvokeRepeating(new Action(this.MovePlayerToPosition), 0f, 0f);
		Analytics.Server.UsedZipline();
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x000A99A8 File Offset: 0x000A7BA8
	private void Update()
	{
		if (this.linePoints == null || base.isClient)
		{
			return;
		}
		if (this.isAnimatingIn)
		{
			return;
		}
		if (this.hasEnded)
		{
			return;
		}
		float num = (this.MoveSpeed + this.additiveValue * this.ForwardAdditive) * Mathf.Clamp(this.mountTime / this.SpeedUpTime, 0f, 1f) * UnityEngine.Time.smoothDeltaTime;
		this.currentTravelDistance += num;
		Vector3 vector = this.ProcessBezierMovement(this.currentTravelDistance);
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		Vector3 position = vector.WithY(vector.y - this.ZipCollider.height * 0.6f);
		Vector3 position2 = vector;
		GamePhysics.CapsuleSweep(position, position2, this.ZipCollider.radius, base.transform.forward, num, list, 1218511105, QueryTriggerInteraction.Ignore);
		foreach (RaycastHit raycastHit in list)
		{
			if (!(raycastHit.collider == this.ZipCollider) && !this.ignoreColliders.Contains(raycastHit.collider) && !(raycastHit.collider.GetComponentInParent<PowerlineNode>() != null))
			{
				global::ZiplineMountable componentInParent = raycastHit.collider.GetComponentInParent<global::ZiplineMountable>();
				if (componentInParent != null)
				{
					componentInParent.EndZipline();
				}
				Vector3 vector2;
				if (!this.GetDismountPosition(this._mounted, out vector2))
				{
					base.transform.position = this.lastSafePosition;
				}
				this.EndZipline();
				Facepunch.Pool.FreeList<RaycastHit>(ref list);
				return;
			}
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		if (Vector3.Distance(vector, this.GetLineEndPoint(false)) < 0.1f)
		{
			base.transform.position = this.GetLineEndPoint(true);
			this.hasEnded = true;
			return;
		}
		if (Vector3.Distance(this.lastSafePosition, base.transform.position) > 0.75f)
		{
			this.lastSafePosition = base.transform.position;
		}
		Vector3 normalized = (vector - base.transform.position.WithY(vector.y)).normalized;
		base.transform.position = Vector3.Lerp(base.transform.position, vector, UnityEngine.Time.deltaTime * 12f);
		base.transform.forward = normalized;
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x000A9C08 File Offset: 0x000A7E08
	public override void PlayerServerInput(InputState inputState, global::BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (this.linePoints == null)
		{
			return;
		}
		if (this.hasEnded)
		{
			this.EndZipline();
			return;
		}
		Vector3 position = base.transform.position;
		float num = (this.GetNextLinePoint(base.transform).y < position.y + 0.1f && inputState.IsDown(BUTTON.FORWARD)) ? 1f : 0f;
		this.additiveValue = Mathf.MoveTowards(this.additiveValue, num, (float)Server.tickrate * ((num > 0f) ? 4f : 2f));
		base.SetFlag(global::BaseEntity.Flags.Reserved1, this.additiveValue > 0.5f, false, true);
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000A9CBF File Offset: 0x000A7EBF
	private void EndZipline()
	{
		this.DismountAllPlayers();
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x000A9CC7 File Offset: 0x000A7EC7
	public override void OnPlayerDismounted(global::BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (!base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x000A9CDF File Offset: 0x000A7EDF
	public override bool ValidDismountPosition(global::BasePlayer player, Vector3 disPos)
	{
		this.ZipCollider.enabled = false;
		bool result = base.ValidDismountPosition(player, disPos);
		this.ZipCollider.enabled = true;
		return result;
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x000A9D04 File Offset: 0x000A7F04
	public override void Save(global::BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.linePoints == null)
		{
			return;
		}
		if (info.msg.ziplineMountable == null)
		{
			info.msg.ziplineMountable = Facepunch.Pool.Get<ProtoBuf.ZiplineMountable>();
		}
		info.msg.ziplineMountable.linePoints = Facepunch.Pool.GetList<VectorData>();
		foreach (Vector3 v in this.linePoints)
		{
			info.msg.ziplineMountable.linePoints.Add(v);
		}
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x000A9DB0 File Offset: 0x000A7FB0
	private void MovePlayerToPosition()
	{
		this.elapsedMoveTime += UnityEngine.Time.deltaTime;
		float num = Mathf.Clamp(this.elapsedMoveTime / this.MountEaseInTime, 0f, 1f);
		Vector3 localPosition = Vector3.Lerp(this.startPosition, this.endPosition, this.MountPositionCurve.Evaluate(num));
		Quaternion localRotation = Quaternion.Lerp(this.startRotation, this.endRotation, this.MountRotationCurve.Evaluate(num));
		base.transform.localPosition = localPosition;
		base.transform.localRotation = localRotation;
		if (num >= 1f)
		{
			this.isAnimatingIn = false;
			base.SetFlag(global::BaseEntity.Flags.Reserved2, true, false, true);
			this.mountTime = 0f;
			base.CancelInvoke(new Action(this.MovePlayerToPosition));
		}
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x000A9E80 File Offset: 0x000A8080
	public override void OnFlagsChanged(global::BaseEntity.Flags old, global::BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(global::BaseEntity.Flags.Busy) && !next.HasFlag(global::BaseEntity.Flags.Busy) && !base.IsDestroyed)
		{
			base.Kill(global::BaseNetworkable.DestroyMode.None);
		}
	}
}

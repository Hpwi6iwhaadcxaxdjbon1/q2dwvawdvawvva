using System;
using UnityEngine;

// Token: 0x02000258 RID: 600
public class ConstructionSocket : Socket_Base
{
	// Token: 0x0400150B RID: 5387
	public ConstructionSocket.Type socketType;

	// Token: 0x0400150C RID: 5388
	public int rotationDegrees;

	// Token: 0x0400150D RID: 5389
	public int rotationOffset;

	// Token: 0x0400150E RID: 5390
	public bool restrictPlacementRotation;

	// Token: 0x0400150F RID: 5391
	public bool restrictPlacementAngle;

	// Token: 0x04001510 RID: 5392
	public float faceAngle;

	// Token: 0x04001511 RID: 5393
	public float angleAllowed = 150f;

	// Token: 0x04001512 RID: 5394
	[Range(0f, 1f)]
	public float support = 1f;

	// Token: 0x06001C4E RID: 7246 RVA: 0x000C5428 File Offset: 0x000C3628
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.6f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(base.transform.position, "light_circle_green.png", false);
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x000C54C4 File Offset: 0x000C36C4
	private void OnDrawGizmosSelected()
	{
		if (this.female)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
		}
	}

	// Token: 0x06001C50 RID: 7248 RVA: 0x000C54EF File Offset: 0x000C36EF
	public override bool TestTarget(Construction.Target target)
	{
		return base.TestTarget(target) && this.IsCompatible(target.socket);
	}

	// Token: 0x06001C51 RID: 7249 RVA: 0x000C5508 File Offset: 0x000C3708
	public override bool IsCompatible(Socket_Base socket)
	{
		if (!base.IsCompatible(socket))
		{
			return false;
		}
		ConstructionSocket constructionSocket = socket as ConstructionSocket;
		return !(constructionSocket == null) && constructionSocket.socketType != ConstructionSocket.Type.None && this.socketType != ConstructionSocket.Type.None && constructionSocket.socketType == this.socketType;
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x000C5558 File Offset: 0x000C3758
	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 matrix4x = Matrix4x4.TRS(position, rotation, Vector3.one);
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.one);
		Vector3 a = matrix4x.MultiplyPoint3x4(this.worldPosition);
		Vector3 b = matrix4x2.MultiplyPoint3x4(socket.worldPosition);
		if (Vector3.Distance(a, b) > 0.01f)
		{
			return false;
		}
		Vector3 vector = matrix4x.MultiplyVector(this.worldRotation * Vector3.forward);
		Vector3 vector2 = matrix4x2.MultiplyVector(socket.worldRotation * Vector3.forward);
		float num = Vector3.Angle(vector, vector2);
		if (this.male && this.female)
		{
			num = Mathf.Min(num, Vector3.Angle(-vector, vector2));
		}
		if (socket.male && socket.female)
		{
			num = Mathf.Min(num, Vector3.Angle(vector, -vector2));
		}
		return num <= 1f;
	}

	// Token: 0x06001C53 RID: 7251 RVA: 0x000C5650 File Offset: 0x000C3850
	public bool TestRestrictedAngles(Vector3 suggestedPos, Quaternion suggestedAng, Construction.Target target)
	{
		if (this.restrictPlacementAngle)
		{
			Quaternion rotation = Quaternion.Euler(0f, this.faceAngle, 0f) * suggestedAng;
			float num = target.ray.direction.XZ3D().DotDegrees(rotation * Vector3.forward);
			if (num > this.angleAllowed * 0.5f)
			{
				return false;
			}
			if (num < this.angleAllowed * -0.5f)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001C54 RID: 7252 RVA: 0x000C56C8 File Offset: 0x000C38C8
	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		if (!target.entity || !target.entity.transform)
		{
			return null;
		}
		if (!this.CanConnectToEntity(target))
		{
			return null;
		}
		ConstructionSocket constructionSocket = target.socket as ConstructionSocket;
		Vector3 worldPosition = target.GetWorldPosition();
		Quaternion worldRotation = target.GetWorldRotation(true);
		if (constructionSocket != null && !this.IsCompatible(constructionSocket))
		{
			return null;
		}
		if (this.rotationDegrees > 0 && (constructionSocket == null || !constructionSocket.restrictPlacementRotation))
		{
			Construction.Placement placement = new Construction.Placement();
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < 360; i += this.rotationDegrees)
			{
				Quaternion lhs = Quaternion.Euler(0f, (float)(this.rotationOffset + i), 0f);
				Vector3 direction = target.ray.direction;
				Vector3 to = lhs * worldRotation * Vector3.up;
				float num3 = Vector3.Angle(direction, to);
				if (num3 < num)
				{
					num = num3;
					num2 = (float)i;
				}
			}
			for (int j = 0; j < 360; j += this.rotationDegrees)
			{
				Quaternion rhs = worldRotation * Quaternion.Inverse(this.rotation);
				Quaternion lhs2 = Quaternion.Euler(target.rotation);
				Quaternion rhs2 = Quaternion.Euler(0f, (float)(this.rotationOffset + j) + num2, 0f);
				Quaternion rotation = lhs2 * rhs2 * rhs;
				Vector3 b = rotation * this.position;
				placement.position = worldPosition - b;
				placement.rotation = rotation;
				if (this.CheckSocketMods(placement))
				{
					return placement;
				}
			}
		}
		Construction.Placement placement2 = new Construction.Placement();
		Quaternion rotation2 = worldRotation * Quaternion.Inverse(this.rotation);
		Vector3 b2 = rotation2 * this.position;
		placement2.position = worldPosition - b2;
		placement2.rotation = rotation2;
		if (!this.TestRestrictedAngles(worldPosition, worldRotation, target))
		{
			return null;
		}
		return placement2;
	}

	// Token: 0x06001C55 RID: 7253 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanConnectToEntity(Construction.Target target)
	{
		return true;
	}

	// Token: 0x02000C85 RID: 3205
	public enum Type
	{
		// Token: 0x0400439E RID: 17310
		None,
		// Token: 0x0400439F RID: 17311
		Foundation,
		// Token: 0x040043A0 RID: 17312
		Floor,
		// Token: 0x040043A1 RID: 17313
		Misc,
		// Token: 0x040043A2 RID: 17314
		Doorway,
		// Token: 0x040043A3 RID: 17315
		Wall,
		// Token: 0x040043A4 RID: 17316
		Block,
		// Token: 0x040043A5 RID: 17317
		Ramp,
		// Token: 0x040043A6 RID: 17318
		StairsTriangle,
		// Token: 0x040043A7 RID: 17319
		Stairs,
		// Token: 0x040043A8 RID: 17320
		FloorFrameTriangle,
		// Token: 0x040043A9 RID: 17321
		Window,
		// Token: 0x040043AA RID: 17322
		Shutters,
		// Token: 0x040043AB RID: 17323
		WallFrame,
		// Token: 0x040043AC RID: 17324
		FloorFrame,
		// Token: 0x040043AD RID: 17325
		WindowDressing,
		// Token: 0x040043AE RID: 17326
		DoorDressing,
		// Token: 0x040043AF RID: 17327
		Elevator,
		// Token: 0x040043B0 RID: 17328
		DoubleDoorDressing
	}
}

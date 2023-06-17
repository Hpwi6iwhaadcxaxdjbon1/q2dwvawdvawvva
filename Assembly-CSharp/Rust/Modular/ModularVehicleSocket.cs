using System;
using UnityEngine;

namespace Rust.Modular
{
	// Token: 0x02000B2A RID: 2858
	[Serializable]
	public class ModularVehicleSocket
	{
		// Token: 0x04003DDC RID: 15836
		[SerializeField]
		private Transform socketTransform;

		// Token: 0x04003DDD RID: 15837
		[SerializeField]
		private ModularVehicleSocket.SocketWheelType wheelType;

		// Token: 0x04003DDE RID: 15838
		[SerializeField]
		private ModularVehicleSocket.SocketLocationType locationType;

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x0600455E RID: 17758 RVA: 0x00195DD0 File Offset: 0x00193FD0
		public Vector3 WorldPosition
		{
			get
			{
				return this.socketTransform.position;
			}
		}

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x0600455F RID: 17759 RVA: 0x00195DDD File Offset: 0x00193FDD
		public Quaternion WorldRotation
		{
			get
			{
				return this.socketTransform.rotation;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x06004560 RID: 17760 RVA: 0x00195DEA File Offset: 0x00193FEA
		public ModularVehicleSocket.SocketWheelType WheelType
		{
			get
			{
				return this.wheelType;
			}
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x06004561 RID: 17761 RVA: 0x00195DF2 File Offset: 0x00193FF2
		public ModularVehicleSocket.SocketLocationType LocationType
		{
			get
			{
				return this.locationType;
			}
		}

		// Token: 0x06004562 RID: 17762 RVA: 0x00195DFC File Offset: 0x00193FFC
		public bool ShouldBeActive(ConditionalSocketSettings modelSettings)
		{
			bool flag = true;
			if (modelSettings.restrictOnLocation)
			{
				ConditionalSocketSettings.LocationCondition locationRestriction = modelSettings.locationRestriction;
				switch (this.LocationType)
				{
				case ModularVehicleSocket.SocketLocationType.Middle:
					flag = (locationRestriction == ConditionalSocketSettings.LocationCondition.Middle || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack);
					break;
				case ModularVehicleSocket.SocketLocationType.Front:
					flag = (locationRestriction == ConditionalSocketSettings.LocationCondition.Front || locationRestriction == ConditionalSocketSettings.LocationCondition.NotBack || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle);
					break;
				case ModularVehicleSocket.SocketLocationType.Back:
					flag = (locationRestriction == ConditionalSocketSettings.LocationCondition.Back || locationRestriction == ConditionalSocketSettings.LocationCondition.NotFront || locationRestriction == ConditionalSocketSettings.LocationCondition.NotMiddle);
					break;
				}
			}
			if (flag && modelSettings.restrictOnWheel)
			{
				flag = (this.WheelType == modelSettings.wheelRestriction);
			}
			return flag;
		}

		// Token: 0x02000F96 RID: 3990
		public enum SocketWheelType
		{
			// Token: 0x04005053 RID: 20563
			NoWheel,
			// Token: 0x04005054 RID: 20564
			ForwardWheel,
			// Token: 0x04005055 RID: 20565
			BackWheel
		}

		// Token: 0x02000F97 RID: 3991
		public enum SocketLocationType
		{
			// Token: 0x04005057 RID: 20567
			Middle,
			// Token: 0x04005058 RID: 20568
			Front,
			// Token: 0x04005059 RID: 20569
			Back
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B45 RID: 2885
	public class WaypointSet : MonoBehaviour, IServerComponent
	{
		// Token: 0x04003E67 RID: 15975
		[SerializeField]
		private List<WaypointSet.Waypoint> _points = new List<WaypointSet.Waypoint>();

		// Token: 0x04003E68 RID: 15976
		[SerializeField]
		private WaypointSet.NavModes navMode;

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x060045EB RID: 17899 RVA: 0x0019846C File Offset: 0x0019666C
		// (set) Token: 0x060045EC RID: 17900 RVA: 0x00198474 File Offset: 0x00196674
		public List<WaypointSet.Waypoint> Points
		{
			get
			{
				return this._points;
			}
			set
			{
				this._points = value;
			}
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x060045ED RID: 17901 RVA: 0x0019847D File Offset: 0x0019667D
		public WaypointSet.NavModes NavMode
		{
			get
			{
				return this.navMode;
			}
		}

		// Token: 0x060045EE RID: 17902 RVA: 0x00198488 File Offset: 0x00196688
		private void OnDrawGizmos()
		{
			for (int i = 0; i < this.Points.Count; i++)
			{
				Transform transform = this.Points[i].Transform;
				if (transform != null)
				{
					if (this.Points[i].IsOccupied)
					{
						Gizmos.color = Color.red;
					}
					else
					{
						Gizmos.color = Color.cyan;
					}
					Gizmos.DrawSphere(transform.position, 0.25f);
					Gizmos.color = Color.cyan;
					if (i + 1 < this.Points.Count)
					{
						Gizmos.DrawLine(transform.position, this.Points[i + 1].Transform.position);
					}
					else if (this.NavMode == WaypointSet.NavModes.Loop)
					{
						Gizmos.DrawLine(transform.position, this.Points[0].Transform.position);
					}
					Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
					foreach (Transform transform2 in this.Points[i].LookatPoints)
					{
						Gizmos.DrawSphere(transform2.position, 0.1f);
						Gizmos.DrawLine(transform.position, transform2.position);
					}
				}
			}
		}

		// Token: 0x02000FA0 RID: 4000
		public enum NavModes
		{
			// Token: 0x04005080 RID: 20608
			Loop,
			// Token: 0x04005081 RID: 20609
			PingPong
		}

		// Token: 0x02000FA1 RID: 4001
		[Serializable]
		public struct Waypoint
		{
			// Token: 0x04005082 RID: 20610
			public Transform Transform;

			// Token: 0x04005083 RID: 20611
			public float WaitTime;

			// Token: 0x04005084 RID: 20612
			public Transform[] LookatPoints;

			// Token: 0x04005085 RID: 20613
			[NonSerialized]
			public bool IsOccupied;
		}
	}
}

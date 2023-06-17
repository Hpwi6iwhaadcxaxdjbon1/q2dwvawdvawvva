using System;
using UnityEngine;

namespace Sonar
{
	// Token: 0x02000A17 RID: 2583
	public class SonarObject : MonoBehaviour, IClientComponent
	{
		// Token: 0x04003750 RID: 14160
		[SerializeField]
		private SonarObject.SType sonarType;

		// Token: 0x02000F08 RID: 3848
		public enum SType
		{
			// Token: 0x04004E1F RID: 19999
			MoonPool,
			// Token: 0x04004E20 RID: 20000
			Sub
		}
	}
}

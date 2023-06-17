using System;
using UnityEngine;

namespace Sonar
{
	// Token: 0x02000A18 RID: 2584
	public class SubmarineSonar : FacepunchBehaviour
	{
		// Token: 0x04003751 RID: 14161
		[SerializeField]
		private float range = 100f;

		// Token: 0x04003752 RID: 14162
		[SerializeField]
		private ParticleSystem sonarPS;

		// Token: 0x04003753 RID: 14163
		[SerializeField]
		private ParticleSystem blipPS;

		// Token: 0x04003754 RID: 14164
		[SerializeField]
		private SonarObject us;

		// Token: 0x04003755 RID: 14165
		[SerializeField]
		private Color greenBlip;

		// Token: 0x04003756 RID: 14166
		[SerializeField]
		private Color redBlip;

		// Token: 0x04003757 RID: 14167
		[SerializeField]
		private Color whiteBlip;

		// Token: 0x04003758 RID: 14168
		[SerializeField]
		private SubmarineAudio submarineAudio;
	}
}

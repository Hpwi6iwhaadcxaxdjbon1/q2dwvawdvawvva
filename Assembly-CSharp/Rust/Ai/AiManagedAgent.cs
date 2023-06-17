using System;
using UnityEngine;

namespace Rust.Ai
{
	// Token: 0x02000B3B RID: 2875
	[DefaultExecutionOrder(-102)]
	public class AiManagedAgent : FacepunchBehaviour, IServerComponent
	{
		// Token: 0x04003E21 RID: 15905
		[Tooltip("TODO: Replace with actual agent type id on the NavMeshAgent when we upgrade to 5.6.1 or above.")]
		public int AgentTypeIndex;

		// Token: 0x04003E22 RID: 15906
		[ReadOnly]
		[NonSerialized]
		public Vector2i NavmeshGridCoord;

		// Token: 0x04003E23 RID: 15907
		private bool isRegistered;

		// Token: 0x060045B9 RID: 17849 RVA: 0x001972B7 File Offset: 0x001954B7
		private void OnEnable()
		{
			this.isRegistered = false;
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || AiManager.nav_disable)
			{
				base.enabled = false;
				return;
			}
		}

		// Token: 0x060045BA RID: 17850 RVA: 0x001972E8 File Offset: 0x001954E8
		private void DelayedRegistration()
		{
			if (!this.isRegistered)
			{
				this.isRegistered = true;
			}
		}

		// Token: 0x060045BB RID: 17851 RVA: 0x001972F9 File Offset: 0x001954F9
		private void OnDisable()
		{
			if (Application.isQuitting)
			{
				return;
			}
			if (!(SingletonComponent<AiManager>.Instance == null) && SingletonComponent<AiManager>.Instance.enabled)
			{
				bool flag = this.isRegistered;
			}
		}
	}
}

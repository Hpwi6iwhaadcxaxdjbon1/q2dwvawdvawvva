using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rust.Modular
{
	// Token: 0x02000B25 RID: 2853
	public class EnableDisableEvent : MonoBehaviour
	{
		// Token: 0x04003DB7 RID: 15799
		[SerializeField]
		private UnityEvent enableEvent;

		// Token: 0x04003DB8 RID: 15800
		[SerializeField]
		private UnityEvent disableEvent;

		// Token: 0x06004527 RID: 17703 RVA: 0x001951D9 File Offset: 0x001933D9
		protected void OnEnable()
		{
			if (this.enableEvent != null)
			{
				this.enableEvent.Invoke();
			}
		}

		// Token: 0x06004528 RID: 17704 RVA: 0x001951EE File Offset: 0x001933EE
		protected void OnDisable()
		{
			if (this.disableEvent != null)
			{
				this.disableEvent.Invoke();
			}
		}
	}
}

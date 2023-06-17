using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
	// Token: 0x02000B22 RID: 2850
	[RequireComponent(typeof(Toggle))]
	internal class ForceWeather : MonoBehaviour
	{
		// Token: 0x04003DA2 RID: 15778
		private Toggle component;

		// Token: 0x04003DA3 RID: 15779
		public bool Rain;

		// Token: 0x04003DA4 RID: 15780
		public bool Fog;

		// Token: 0x04003DA5 RID: 15781
		public bool Wind;

		// Token: 0x04003DA6 RID: 15782
		public bool Clouds;

		// Token: 0x0600451D RID: 17693 RVA: 0x00194FC9 File Offset: 0x001931C9
		public void OnEnable()
		{
			this.component = base.GetComponent<Toggle>();
		}

		// Token: 0x0600451E RID: 17694 RVA: 0x00194FD8 File Offset: 0x001931D8
		public void Update()
		{
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			if (this.Rain)
			{
				SingletonComponent<Climate>.Instance.Overrides.Rain = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Rain, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Fog)
			{
				SingletonComponent<Climate>.Instance.Overrides.Fog = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Fog, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Wind)
			{
				SingletonComponent<Climate>.Instance.Overrides.Wind = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Wind, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
			if (this.Clouds)
			{
				SingletonComponent<Climate>.Instance.Overrides.Clouds = Mathf.MoveTowards(SingletonComponent<Climate>.Instance.Overrides.Clouds, (float)(this.component.isOn ? 1 : 0), Time.deltaTime / 2f);
			}
		}
	}
}

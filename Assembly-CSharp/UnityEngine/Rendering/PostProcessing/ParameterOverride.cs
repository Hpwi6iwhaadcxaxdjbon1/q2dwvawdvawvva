using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7B RID: 2683
	public abstract class ParameterOverride
	{
		// Token: 0x04003945 RID: 14661
		public bool overrideState;

		// Token: 0x06003FFA RID: 16378
		internal abstract void Interp(ParameterOverride from, ParameterOverride to, float t);

		// Token: 0x06003FFB RID: 16379
		public abstract int GetHash();

		// Token: 0x06003FFC RID: 16380 RVA: 0x0017A89B File Offset: 0x00178A9B
		public T GetValue<T>()
		{
			return ((ParameterOverride<T>)this).value;
		}

		// Token: 0x06003FFD RID: 16381 RVA: 0x000063A5 File Offset: 0x000045A5
		protected internal virtual void OnEnable()
		{
		}

		// Token: 0x06003FFE RID: 16382 RVA: 0x000063A5 File Offset: 0x000045A5
		protected internal virtual void OnDisable()
		{
		}

		// Token: 0x06003FFF RID: 16383
		internal abstract void SetValue(ParameterOverride parameter);
	}
}

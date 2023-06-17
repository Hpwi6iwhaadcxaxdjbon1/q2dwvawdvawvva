using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7C RID: 2684
	[Serializable]
	public class ParameterOverride<T> : ParameterOverride
	{
		// Token: 0x04003946 RID: 14662
		public T value;

		// Token: 0x06004001 RID: 16385 RVA: 0x0017A8A8 File Offset: 0x00178AA8
		public ParameterOverride() : this(default(T), false)
		{
		}

		// Token: 0x06004002 RID: 16386 RVA: 0x0017A8C5 File Offset: 0x00178AC5
		public ParameterOverride(T value) : this(value, false)
		{
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x0017A8CF File Offset: 0x00178ACF
		public ParameterOverride(T value, bool overrideState)
		{
			this.value = value;
			this.overrideState = overrideState;
		}

		// Token: 0x06004004 RID: 16388 RVA: 0x0017A8E5 File Offset: 0x00178AE5
		internal override void Interp(ParameterOverride from, ParameterOverride to, float t)
		{
			this.Interp(from.GetValue<T>(), to.GetValue<T>(), t);
		}

		// Token: 0x06004005 RID: 16389 RVA: 0x0017A8FA File Offset: 0x00178AFA
		public virtual void Interp(T from, T to, float t)
		{
			this.value = ((t > 0f) ? to : from);
		}

		// Token: 0x06004006 RID: 16390 RVA: 0x0017A90E File Offset: 0x00178B0E
		public void Override(T x)
		{
			this.overrideState = true;
			this.value = x;
		}

		// Token: 0x06004007 RID: 16391 RVA: 0x0017A91E File Offset: 0x00178B1E
		internal override void SetValue(ParameterOverride parameter)
		{
			this.value = parameter.GetValue<T>();
		}

		// Token: 0x06004008 RID: 16392 RVA: 0x0017A92C File Offset: 0x00178B2C
		public override int GetHash()
		{
			return (17 * 23 + this.overrideState.GetHashCode()) * 23 + this.value.GetHashCode();
		}

		// Token: 0x06004009 RID: 16393 RVA: 0x0017A954 File Offset: 0x00178B54
		public static implicit operator T(ParameterOverride<T> prop)
		{
			return prop.value;
		}
	}
}

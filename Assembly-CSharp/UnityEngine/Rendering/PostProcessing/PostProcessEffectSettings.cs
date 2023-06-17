using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8E RID: 2702
	[Serializable]
	public class PostProcessEffectSettings : ScriptableObject
	{
		// Token: 0x04003977 RID: 14711
		public bool active = true;

		// Token: 0x04003978 RID: 14712
		public BoolParameter enabled = new BoolParameter
		{
			overrideState = true,
			value = false
		};

		// Token: 0x04003979 RID: 14713
		internal ReadOnlyCollection<ParameterOverride> parameters;

		// Token: 0x06004054 RID: 16468 RVA: 0x0017B8B0 File Offset: 0x00179AB0
		private void OnEnable()
		{
			this.parameters = (from t in base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
			where t.FieldType.IsSubclassOf(typeof(ParameterOverride))
			orderby t.MetadataToken
			select (ParameterOverride)t.GetValue(this)).ToList<ParameterOverride>().AsReadOnly();
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				parameterOverride.OnEnable();
			}
		}

		// Token: 0x06004055 RID: 16469 RVA: 0x0017B970 File Offset: 0x00179B70
		private void OnDisable()
		{
			if (this.parameters == null)
			{
				return;
			}
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				parameterOverride.OnDisable();
			}
		}

		// Token: 0x06004056 RID: 16470 RVA: 0x0017B9C4 File Offset: 0x00179BC4
		public void SetAllOverridesTo(bool state, bool excludeEnabled = true)
		{
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				if (!excludeEnabled || parameterOverride != this.enabled)
				{
					parameterOverride.overrideState = state;
				}
			}
		}

		// Token: 0x06004057 RID: 16471 RVA: 0x0017BA20 File Offset: 0x00179C20
		public virtual bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value;
		}

		// Token: 0x06004058 RID: 16472 RVA: 0x0017BA30 File Offset: 0x00179C30
		public int GetHash()
		{
			int num = 17;
			foreach (ParameterOverride parameterOverride in this.parameters)
			{
				num = num * 23 + parameterOverride.GetHash();
			}
			return num;
		}
	}
}

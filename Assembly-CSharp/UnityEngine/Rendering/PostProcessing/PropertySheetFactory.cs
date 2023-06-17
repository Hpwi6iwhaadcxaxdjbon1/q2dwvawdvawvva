using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9B RID: 2715
	public sealed class PropertySheetFactory
	{
		// Token: 0x040039AA RID: 14762
		private readonly Dictionary<Shader, PropertySheet> m_Sheets;

		// Token: 0x060040B2 RID: 16562 RVA: 0x0017D5A3 File Offset: 0x0017B7A3
		public PropertySheetFactory()
		{
			this.m_Sheets = new Dictionary<Shader, PropertySheet>();
		}

		// Token: 0x060040B3 RID: 16563 RVA: 0x0017D5B8 File Offset: 0x0017B7B8
		[Obsolete("Use PropertySheet.Get(Shader) with a direct reference to the Shader instead.")]
		public PropertySheet Get(string shaderName)
		{
			Shader shader = Shader.Find(shaderName);
			if (shader == null)
			{
				throw new ArgumentException(string.Format("Invalid shader ({0})", shaderName));
			}
			return this.Get(shader);
		}

		// Token: 0x060040B4 RID: 16564 RVA: 0x0017D5F0 File Offset: 0x0017B7F0
		public PropertySheet Get(Shader shader)
		{
			if (shader == null)
			{
				throw new ArgumentException(string.Format("Invalid shader ({0})", shader));
			}
			PropertySheet propertySheet;
			if (this.m_Sheets.TryGetValue(shader, out propertySheet))
			{
				return propertySheet;
			}
			string name = shader.name;
			propertySheet = new PropertySheet(new Material(shader)
			{
				name = string.Format("PostProcess - {0}", name.Substring(name.LastIndexOf('/') + 1)),
				hideFlags = HideFlags.DontSave
			});
			this.m_Sheets.Add(shader, propertySheet);
			return propertySheet;
		}

		// Token: 0x060040B5 RID: 16565 RVA: 0x0017D674 File Offset: 0x0017B874
		public void Release()
		{
			foreach (PropertySheet propertySheet in this.m_Sheets.Values)
			{
				propertySheet.Release();
			}
			this.m_Sheets.Clear();
		}
	}
}

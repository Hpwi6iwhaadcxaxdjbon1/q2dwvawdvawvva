using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x02000A14 RID: 2580
	public class RenderTextureUtility
	{
		// Token: 0x04003749 RID: 14153
		private List<RenderTexture> m_TemporaryRTs = new List<RenderTexture>();

		// Token: 0x06003D61 RID: 15713 RVA: 0x0016910C File Offset: 0x0016730C
		public RenderTexture GetTemporaryRenderTexture(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.ARGBHalf, FilterMode filterMode = FilterMode.Bilinear)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, depthBuffer, format);
			temporary.filterMode = filterMode;
			temporary.wrapMode = TextureWrapMode.Clamp;
			temporary.name = "RenderTextureUtilityTempTexture";
			this.m_TemporaryRTs.Add(temporary);
			return temporary;
		}

		// Token: 0x06003D62 RID: 15714 RVA: 0x0016914C File Offset: 0x0016734C
		public void ReleaseTemporaryRenderTexture(RenderTexture rt)
		{
			if (rt == null)
			{
				return;
			}
			if (!this.m_TemporaryRTs.Contains(rt))
			{
				Debug.LogErrorFormat("Attempting to remove texture that was not allocated: {0}", new object[]
				{
					rt
				});
				return;
			}
			this.m_TemporaryRTs.Remove(rt);
			RenderTexture.ReleaseTemporary(rt);
		}

		// Token: 0x06003D63 RID: 15715 RVA: 0x0016919C File Offset: 0x0016739C
		public void ReleaseAllTemporaryRenderTextures()
		{
			for (int i = 0; i < this.m_TemporaryRTs.Count; i++)
			{
				RenderTexture.ReleaseTemporary(this.m_TemporaryRTs[i]);
			}
			this.m_TemporaryRTs.Clear();
		}
	}
}

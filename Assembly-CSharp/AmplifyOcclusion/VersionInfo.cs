using System;
using UnityEngine;

namespace AmplifyOcclusion
{
	// Token: 0x020009C4 RID: 2500
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x04003652 RID: 13906
		public const byte Major = 2;

		// Token: 0x04003653 RID: 13907
		public const byte Minor = 0;

		// Token: 0x04003654 RID: 13908
		public const byte Release = 0;

		// Token: 0x04003655 RID: 13909
		private static string StageSuffix = "_dev002";

		// Token: 0x04003656 RID: 13910
		[SerializeField]
		private int m_major;

		// Token: 0x04003657 RID: 13911
		[SerializeField]
		private int m_minor;

		// Token: 0x04003658 RID: 13912
		[SerializeField]
		private int m_release;

		// Token: 0x06003BAC RID: 15276 RVA: 0x00160ED7 File Offset: 0x0015F0D7
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 2, 0, 0) + VersionInfo.StageSuffix;
		}

		// Token: 0x06003BAD RID: 15277 RVA: 0x00160EFF File Offset: 0x0015F0FF
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix;
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06003BAE RID: 15278 RVA: 0x00160F36 File Offset: 0x0015F136
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x06003BAF RID: 15279 RVA: 0x00160F52 File Offset: 0x0015F152
		private VersionInfo()
		{
			this.m_major = 2;
			this.m_minor = 0;
			this.m_release = 0;
		}

		// Token: 0x06003BB0 RID: 15280 RVA: 0x00160F6F File Offset: 0x0015F16F
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x06003BB1 RID: 15281 RVA: 0x00160F8C File Offset: 0x0015F18C
		public static VersionInfo Current()
		{
			return new VersionInfo(2, 0, 0);
		}

		// Token: 0x06003BB2 RID: 15282 RVA: 0x00160F96 File Offset: 0x0015F196
		public static bool Matches(VersionInfo version)
		{
			return 2 == version.m_major && version.m_minor == 0 && version.m_release == 0;
		}
	}
}

using System;
using Facepunch;

namespace ConVar
{
	// Token: 0x02000ACA RID: 2762
	public class Manifest
	{
		// Token: 0x0600426C RID: 17004 RVA: 0x00189DEE File Offset: 0x00187FEE
		[ClientVar]
		[ServerVar]
		public static object PrintManifest()
		{
			return Application.Manifest;
		}

		// Token: 0x0600426D RID: 17005 RVA: 0x00189DF5 File Offset: 0x00187FF5
		[ClientVar]
		[ServerVar]
		public static object PrintManifestRaw()
		{
			return Manifest.Contents;
		}
	}
}

using System;

namespace TinyJSON
{
	// Token: 0x020009C6 RID: 2502
	[Flags]
	public enum EncodeOptions
	{
		// Token: 0x0400365D RID: 13917
		None = 0,
		// Token: 0x0400365E RID: 13918
		PrettyPrint = 1,
		// Token: 0x0400365F RID: 13919
		NoTypeHints = 2,
		// Token: 0x04003660 RID: 13920
		IncludePublicProperties = 4,
		// Token: 0x04003661 RID: 13921
		EnforceHierarchyOrder = 8,
		// Token: 0x04003662 RID: 13922
		[Obsolete("Use EncodeOptions.EnforceHierarchyOrder instead.")]
		EnforceHeirarchyOrder = 8
	}
}

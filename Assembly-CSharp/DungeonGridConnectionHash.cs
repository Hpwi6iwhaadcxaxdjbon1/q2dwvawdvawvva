using System;

// Token: 0x0200066F RID: 1647
public struct DungeonGridConnectionHash
{
	// Token: 0x040026F5 RID: 9973
	public bool North;

	// Token: 0x040026F6 RID: 9974
	public bool South;

	// Token: 0x040026F7 RID: 9975
	public bool West;

	// Token: 0x040026F8 RID: 9976
	public bool East;

	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x06002F8C RID: 12172 RVA: 0x0011E116 File Offset: 0x0011C316
	public int Value
	{
		get
		{
			return (this.North ? 1 : 0) | (this.South ? 2 : 0) | (this.West ? 4 : 0) | (this.East ? 8 : 0);
		}
	}
}

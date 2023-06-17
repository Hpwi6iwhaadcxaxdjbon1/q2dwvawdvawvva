using System;

// Token: 0x02000717 RID: 1815
[Serializable]
public struct ExtendGBufferParams
{
	// Token: 0x04002977 RID: 10615
	public bool enabled;

	// Token: 0x04002978 RID: 10616
	public static ExtendGBufferParams Default = new ExtendGBufferParams
	{
		enabled = false
	};
}

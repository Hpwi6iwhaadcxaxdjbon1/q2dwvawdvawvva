using System;

// Token: 0x020002A5 RID: 677
public abstract class ComponentInfo<T> : ComponentInfo
{
	// Token: 0x04001622 RID: 5666
	public T component;

	// Token: 0x06001D32 RID: 7474 RVA: 0x000C956D File Offset: 0x000C776D
	public void Initialize(T source)
	{
		this.component = source;
		this.Setup();
	}
}

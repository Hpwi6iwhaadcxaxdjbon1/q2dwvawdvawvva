using System;

// Token: 0x020005AE RID: 1454
public abstract class WeatherEffectSting : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04002397 RID: 9111
	public float frequency = 600f;

	// Token: 0x04002398 RID: 9112
	public float variance = 300f;

	// Token: 0x04002399 RID: 9113
	public GameObjectRef[] effects;
}

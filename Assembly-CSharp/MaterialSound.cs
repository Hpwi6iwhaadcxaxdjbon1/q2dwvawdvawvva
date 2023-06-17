using System;
using UnityEngine;

// Token: 0x0200060A RID: 1546
[CreateAssetMenu(menuName = "Rust/MaterialSound")]
public class MaterialSound : ScriptableObject
{
	// Token: 0x04002563 RID: 9571
	public SoundDefinition DefaultSound;

	// Token: 0x04002564 RID: 9572
	public MaterialSound.Entry[] Entries;

	// Token: 0x02000D88 RID: 3464
	[Serializable]
	public class Entry
	{
		// Token: 0x040047CE RID: 18382
		public PhysicMaterial Material;

		// Token: 0x040047CF RID: 18383
		public SoundDefinition Sound;
	}
}

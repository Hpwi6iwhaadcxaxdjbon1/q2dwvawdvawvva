using System;
using UnityEngine;

// Token: 0x020003A4 RID: 932
[CreateAssetMenu]
public class RadioPlaylist : ScriptableObject
{
	// Token: 0x040019C4 RID: 6596
	public string Url;

	// Token: 0x040019C5 RID: 6597
	public AudioClip[] Playlist;

	// Token: 0x040019C6 RID: 6598
	public float PlaylistLength;
}

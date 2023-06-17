using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007F3 RID: 2035
public class MapInterface : SingletonComponent<MapInterface>
{
	// Token: 0x04002D82 RID: 11650
	public static bool IsOpen;

	// Token: 0x04002D83 RID: 11651
	public Image cameraPositon;

	// Token: 0x04002D84 RID: 11652
	public ScrollRectEx scrollRect;

	// Token: 0x04002D85 RID: 11653
	public Toggle showGridToggle;

	// Token: 0x04002D86 RID: 11654
	public Button FocusButton;

	// Token: 0x04002D87 RID: 11655
	public CanvasGroup CanvasGroup;

	// Token: 0x04002D88 RID: 11656
	public SoundDefinition PlaceMarkerSound;

	// Token: 0x04002D89 RID: 11657
	public SoundDefinition ClearMarkerSound;

	// Token: 0x04002D8A RID: 11658
	public MapView View;

	// Token: 0x04002D8B RID: 11659
	public Color[] PointOfInterestColours;

	// Token: 0x04002D8C RID: 11660
	public MapInterface.PointOfInterestSpriteConfig[] PointOfInterestSprites;

	// Token: 0x04002D8D RID: 11661
	public Sprite PingBackground;

	// Token: 0x04002D8E RID: 11662
	public bool DebugStayOpen;

	// Token: 0x04002D8F RID: 11663
	public GameObjectRef MarkerListPrefab;

	// Token: 0x04002D90 RID: 11664
	public GameObject MarkerHeader;

	// Token: 0x04002D91 RID: 11665
	public Transform LocalPlayerMarkerListParent;

	// Token: 0x04002D92 RID: 11666
	public Transform TeamMarkerListParent;

	// Token: 0x04002D93 RID: 11667
	public GameObject TeamLeaderHeader;

	// Token: 0x04002D94 RID: 11668
	public RustButton HideTeamLeaderMarkersToggle;

	// Token: 0x04002D95 RID: 11669
	public CanvasGroup TeamMarkersCanvas;

	// Token: 0x04002D96 RID: 11670
	public RustImageButton ShowSleepingBagsButton;

	// Token: 0x02000E82 RID: 3714
	[Serializable]
	public struct PointOfInterestSpriteConfig
	{
		// Token: 0x04004BED RID: 19437
		public Sprite inner;

		// Token: 0x04004BEE RID: 19438
		public Sprite outer;
	}
}

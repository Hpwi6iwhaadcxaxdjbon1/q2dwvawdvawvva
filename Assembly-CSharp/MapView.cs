using System;
using System.Collections.Generic;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020007F6 RID: 2038
public class MapView : FacepunchBehaviour
{
	// Token: 0x04002D9A RID: 11674
	public RawImage mapImage;

	// Token: 0x04002D9B RID: 11675
	public Image cameraPositon;

	// Token: 0x04002D9C RID: 11676
	public ScrollRectEx scrollRect;

	// Token: 0x04002D9D RID: 11677
	public GameObject monumentMarkerContainer;

	// Token: 0x04002D9E RID: 11678
	public Transform clusterMarkerContainer;

	// Token: 0x04002D9F RID: 11679
	public GameObjectRef monumentMarkerPrefab;

	// Token: 0x04002DA0 RID: 11680
	public GameObject missionMarkerContainer;

	// Token: 0x04002DA1 RID: 11681
	public GameObjectRef missionMarkerPrefab;

	// Token: 0x04002DA2 RID: 11682
	public Transform activeInteractionParent;

	// Token: 0x04002DA3 RID: 11683
	public Transform localPlayerInterestPointRoot;

	// Token: 0x04002DA4 RID: 11684
	public TeamMemberMapMarker[] teamPositions;

	// Token: 0x04002DA5 RID: 11685
	public List<PointOfInterestMapMarker> PointOfInterestMarkers;

	// Token: 0x04002DA6 RID: 11686
	public List<PointOfInterestMapMarker> TeamPointOfInterestMarkers;

	// Token: 0x04002DA7 RID: 11687
	public List<PointOfInterestMapMarker> LocalPings;

	// Token: 0x04002DA8 RID: 11688
	public List<PointOfInterestMapMarker> TeamPings;

	// Token: 0x04002DA9 RID: 11689
	public GameObject PlayerDeathMarker;

	// Token: 0x04002DAA RID: 11690
	public List<SleepingBagMapMarker> SleepingBagMarkers = new List<SleepingBagMapMarker>();

	// Token: 0x04002DAB RID: 11691
	public List<SleepingBagClusterMapMarker> SleepingBagClusters = new List<SleepingBagClusterMapMarker>();

	// Token: 0x04002DAC RID: 11692
	[FormerlySerializedAs("TrainLayer")]
	public RawImage UndergroundLayer;

	// Token: 0x04002DAD RID: 11693
	public bool ShowGrid;

	// Token: 0x04002DAE RID: 11694
	public bool ShowPointOfInterestMarkers;

	// Token: 0x04002DAF RID: 11695
	public bool ShowDeathMarker = true;

	// Token: 0x04002DB0 RID: 11696
	public bool ShowSleepingBags = true;

	// Token: 0x04002DB1 RID: 11697
	public bool AllowSleepingBagDeletion;

	// Token: 0x04002DB2 RID: 11698
	public bool ShowLocalPlayer = true;

	// Token: 0x04002DB3 RID: 11699
	public bool ShowTeamMembers = true;

	// Token: 0x04002DB4 RID: 11700
	public bool ShowTrainLayer;

	// Token: 0x04002DB5 RID: 11701
	public bool ShowMissions;

	// Token: 0x04002DB6 RID: 11702
	[FormerlySerializedAs("ShowTrainLayer")]
	public bool ShowUndergroundLayers;

	// Token: 0x04002DB7 RID: 11703
	public bool MLRSMarkerMode;

	// Token: 0x04002DB8 RID: 11704
	public RustImageButton LockButton;

	// Token: 0x04002DB9 RID: 11705
	public RustImageButton OverworldButton;

	// Token: 0x04002DBA RID: 11706
	public RustImageButton TrainButton;

	// Token: 0x04002DBB RID: 11707
	public RustImageButton[] UnderwaterButtons;

	// Token: 0x04002DBC RID: 11708
	public RustImageButton DungeonButton;
}

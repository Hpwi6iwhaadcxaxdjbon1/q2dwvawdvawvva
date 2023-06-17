using System;
using UnityEngine;

// Token: 0x0200098D RID: 2445
public class ExplosionDemoGUI : MonoBehaviour
{
	// Token: 0x0400347E RID: 13438
	public GameObject[] Prefabs;

	// Token: 0x0400347F RID: 13439
	public float reactivateTime = 4f;

	// Token: 0x04003480 RID: 13440
	public Light Sun;

	// Token: 0x04003481 RID: 13441
	private int currentNomber;

	// Token: 0x04003482 RID: 13442
	private GameObject currentInstance;

	// Token: 0x04003483 RID: 13443
	private GUIStyle guiStyleHeader = new GUIStyle();

	// Token: 0x04003484 RID: 13444
	private float sunIntensity;

	// Token: 0x04003485 RID: 13445
	private float dpiScale;

	// Token: 0x06003A3A RID: 14906 RVA: 0x00158AC4 File Offset: 0x00156CC4
	private void Start()
	{
		if (Screen.dpi < 1f)
		{
			this.dpiScale = 1f;
		}
		if (Screen.dpi < 200f)
		{
			this.dpiScale = 1f;
		}
		else
		{
			this.dpiScale = Screen.dpi / 200f;
		}
		this.guiStyleHeader.fontSize = (int)(15f * this.dpiScale);
		this.guiStyleHeader.normal.textColor = new Color(0.15f, 0.15f, 0.15f);
		this.currentInstance = UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.currentNomber], base.transform.position, default(Quaternion));
		this.currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = this.reactivateTime;
		this.sunIntensity = this.Sun.intensity;
	}

	// Token: 0x06003A3B RID: 14907 RVA: 0x00158BA4 File Offset: 0x00156DA4
	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "PREVIOUS EFFECT"))
		{
			this.ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(160f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "NEXT EFFECT"))
		{
			this.ChangeCurrent(1);
		}
		this.sunIntensity = GUI.HorizontalSlider(new Rect(10f * this.dpiScale, 70f * this.dpiScale, 285f * this.dpiScale, 15f * this.dpiScale), this.sunIntensity, 0f, 0.6f);
		this.Sun.intensity = this.sunIntensity;
		GUI.Label(new Rect(300f * this.dpiScale, 70f * this.dpiScale, 30f * this.dpiScale, 30f * this.dpiScale), "SUN INTENSITY", this.guiStyleHeader);
		GUI.Label(new Rect(400f * this.dpiScale, 15f * this.dpiScale, 100f * this.dpiScale, 20f * this.dpiScale), "Prefab name is \"" + this.Prefabs[this.currentNomber].name + "\"  \r\nHold any mouse button that would move the camera", this.guiStyleHeader);
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x00158D48 File Offset: 0x00156F48
	private void ChangeCurrent(int delta)
	{
		this.currentNomber += delta;
		if (this.currentNomber > this.Prefabs.Length - 1)
		{
			this.currentNomber = 0;
		}
		else if (this.currentNomber < 0)
		{
			this.currentNomber = this.Prefabs.Length - 1;
		}
		if (this.currentInstance != null)
		{
			UnityEngine.Object.Destroy(this.currentInstance);
		}
		this.currentInstance = UnityEngine.Object.Instantiate<GameObject>(this.Prefabs[this.currentNomber], base.transform.position, default(Quaternion));
		this.currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = this.reactivateTime;
	}
}

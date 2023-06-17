using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200089E RID: 2206
public class UIParticle : BaseMonoBehaviour
{
	// Token: 0x0400316A RID: 12650
	public Vector2 LifeTime;

	// Token: 0x0400316B RID: 12651
	public Vector2 Gravity = new Vector2(1000f, 1000f);

	// Token: 0x0400316C RID: 12652
	public Vector2 InitialX;

	// Token: 0x0400316D RID: 12653
	public Vector2 InitialY;

	// Token: 0x0400316E RID: 12654
	public Vector2 InitialScale = Vector2.one;

	// Token: 0x0400316F RID: 12655
	public Vector2 InitialDelay;

	// Token: 0x04003170 RID: 12656
	public Vector2 ScaleVelocity;

	// Token: 0x04003171 RID: 12657
	public Gradient InitialColor;

	// Token: 0x04003172 RID: 12658
	private float lifetime;

	// Token: 0x04003173 RID: 12659
	private float gravity;

	// Token: 0x04003174 RID: 12660
	private Vector2 velocity;

	// Token: 0x04003175 RID: 12661
	private float scaleVelocity;

	// Token: 0x060036EC RID: 14060 RVA: 0x0014B0B0 File Offset: 0x001492B0
	public static void Add(UIParticle particleSource, RectTransform spawnPosition, RectTransform particleCanvas)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(particleSource.gameObject);
		gameObject.transform.SetParent(spawnPosition, false);
		gameObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(0f, spawnPosition.rect.width) - spawnPosition.rect.width * spawnPosition.pivot.x, UnityEngine.Random.Range(0f, spawnPosition.rect.height) - spawnPosition.rect.height * spawnPosition.pivot.y, 0f);
		gameObject.transform.SetParent(particleCanvas, true);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x0014B17C File Offset: 0x0014937C
	private void Start()
	{
		base.transform.localScale *= UnityEngine.Random.Range(this.InitialScale.x, this.InitialScale.y);
		this.velocity.x = UnityEngine.Random.Range(this.InitialX.x, this.InitialX.y);
		this.velocity.y = UnityEngine.Random.Range(this.InitialY.x, this.InitialY.y);
		this.gravity = UnityEngine.Random.Range(this.Gravity.x, this.Gravity.y);
		this.scaleVelocity = UnityEngine.Random.Range(this.ScaleVelocity.x, this.ScaleVelocity.y);
		Image component = base.GetComponent<Image>();
		if (component)
		{
			component.color = this.InitialColor.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}
		this.lifetime = UnityEngine.Random.Range(this.InitialDelay.x, this.InitialDelay.y) * -1f;
		if (this.lifetime < 0f)
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}
		base.Invoke(new Action(this.Die), UnityEngine.Random.Range(this.LifeTime.x, this.LifeTime.y) + this.lifetime * -1f);
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x0014B2F8 File Offset: 0x001494F8
	private void Update()
	{
		if (this.lifetime < 0f)
		{
			this.lifetime += Time.deltaTime;
			if (this.lifetime < 0f)
			{
				return;
			}
			base.GetComponent<CanvasGroup>().alpha = 1f;
		}
		else
		{
			this.lifetime += Time.deltaTime;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.localScale;
		this.velocity.y = this.velocity.y - this.gravity * Time.deltaTime;
		position.x += this.velocity.x * Time.deltaTime;
		position.y += this.velocity.y * Time.deltaTime;
		vector += Vector3.one * this.scaleVelocity * Time.deltaTime;
		if (vector.x <= 0f || vector.y <= 0f)
		{
			this.Die();
			return;
		}
		base.transform.position = position;
		base.transform.localScale = vector;
	}

	// Token: 0x060036EF RID: 14063 RVA: 0x0014B41B File Offset: 0x0014961B
	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}

using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable {

	[SerializeField]
	private int damage = 10;
	[SerializeField]
	private float lifetime = 3f;
	[SerializeField]
	private LayerMask layerMask;
	private bool isHitted = false;

	[SerializeField]
	private string poolID = "Bullet";
	public string PoolID { get { return poolID; } }
	[SerializeField]
	private int objectsCount = 100;
	public int ObjectsCount { get { return objectsCount; } }
	public bool IsActive { get { return gameObject.activeSelf; } }

	public void Initialize(Transform firepoint) {
		transform.position = firepoint.position;
		transform.rotation = firepoint.rotation;
		CancelInvoke();
		isHitted = false;
		Invoke("Disable", lifetime);
		gameObject.SetActive(true);
	}

	private void OnCollisionEnter(Collision other) {
		if (isHitted) return;
		bool isNeededLayer = (1 << other.gameObject.layer & layerMask.value) != 0;
		if (!isNeededLayer) return;
		isHitted = true;
		var obj = other.collider.GetComponent<IGetDamage>();
		if (obj != null) obj.GetDamage(damage);
		Disable();
	}

	private void Disable() {
		gameObject.SetActive(false);
	}
}

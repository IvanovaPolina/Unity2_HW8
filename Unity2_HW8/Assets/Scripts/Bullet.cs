using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable {

	[SerializeField]
	private int damage = 10;
	[SerializeField]
	private float lifetime = 2f;
	[SerializeField]
	private LayerMask layerMask;
	private float speed;
	private bool isHitted = false;

	[SerializeField]
	private string poolID = "Bullet";
	public string PoolID { get { return poolID; } }
	[SerializeField]
	private int objectsCount = 100;
	public int ObjectsCount { get { return objectsCount; } }
	public bool IsActive { get { return gameObject.activeSelf; } }

	public void Initialize(Transform firepoint, float speed) {
		transform.position = firepoint.position;
		transform.rotation = firepoint.rotation;
		CancelInvoke();
		isHitted = false;
		this.speed = speed;
		Invoke("Disable", lifetime);
		gameObject.SetActive(true);
	}

	private void FixedUpdate() {
		if (isHitted) return;
		Vector3 finalPos = transform.position + transform.forward * speed * Time.fixedDeltaTime;
		RaycastHit hit;
		if (Physics.Linecast(transform.position, finalPos, out hit, layerMask)) {
			isHitted = true;
			transform.position = hit.point;
			SetDamage(hit);
			Disable();
		} else transform.position = finalPos;
	}

	private void SetDamage(RaycastHit hit) {
		var obj = hit.collider.GetComponent<IGetDamage>();
		if (obj == null) return;
		obj.GetDamage(damage);
	}

	private void Disable() {
		gameObject.SetActive(false);
	}
}

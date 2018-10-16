using UnityEngine;

public class Bullet : MonoBehaviour {

	[SerializeField]
	private int damage = 10;
	[SerializeField]
	private float lifetime = 2f;

	private void Start() {
		Destroy(gameObject, lifetime);
	}

	private void OnCollisionEnter(Collision other) {
		var obj = other.gameObject.GetComponent<IGetDamage>();
		if (obj != null) obj.GetDamage(damage);
		Destroy(gameObject);
	}
}

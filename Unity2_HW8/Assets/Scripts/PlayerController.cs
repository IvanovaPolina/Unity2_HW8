using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	[SerializeField]
	private float moveSpeed = 1.5f;
	[SerializeField]
	private float rotationSpeed = 30f;

	[SerializeField]
	private Rigidbody bullet;
	[SerializeField]
	private float bulletSpeed = 10f;
	[SerializeField]
	private Transform firepoint;

	[SerializeField]
	private Material playerMaterial;
	[SerializeField]
	private Material enemyMaterial;

	[SerializeField]
	private Transform cameraPos;

	private void Start() {
		if (!isLocalPlayer) {
			var renderers = GetComponentsInChildren<Renderer>();
			foreach (var rend in renderers)
				for (int i = 0; i < rend.materials.Length; i++)
					if (rend.materials[i].color == playerMaterial.color)
						rend.materials[i].color = enemyMaterial.color;
		} else {
			Camera.main.transform.position = Vector3.zero;
			Camera.main.transform.rotation = new Quaternion(0, 0, 0, 1);
			Camera.main.transform.SetParent(cameraPos, false);
		}
	}

	private void Update () {
		if (!isLocalPlayer) return;

		float y = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
		float z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

		transform.Rotate(0, y, 0);
		transform.Translate(0, 0, z);

		if (Input.GetButtonDown("Fire1")) CmdFire();
	}

	[Command]
	private void CmdFire() {
		var tempBullet = Instantiate(bullet, firepoint.position, firepoint.rotation);
		tempBullet.velocity = tempBullet.transform.forward * bulletSpeed;
		NetworkServer.Spawn(tempBullet.gameObject);
	}
}

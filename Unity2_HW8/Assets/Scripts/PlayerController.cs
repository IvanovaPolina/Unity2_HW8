using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	[SerializeField]
	private float moveSpeed = 1.5f;
	[SerializeField]
	private float rotationSpeed = 30f;
	
	[SerializeField]
	private float bulletSpeed = 10f;
	[SerializeField]
	private Transform firepoint;
	[SerializeField]
	private string bulletID = "Bullet";

	[SerializeField]
	private Material playerMaterial;
	[SerializeField]
	private Material enemyMaterial;

	[SerializeField]
	private Transform turret;
	[SerializeField]
	private float turretRotSpeed = 40f;

	[SerializeField]
	private Transform cam;

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
			Camera.main.transform.SetParent(cam, false);
		}
	}

	private void Update () {
		if (!isLocalPlayer) return;

		float y = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
		float z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

		transform.Rotate(0, y, 0);
		transform.Translate(0, 0, z);
		RotateTurret();
		
		if (Input.GetButtonDown("Fire1")) CmdFire();
	}

	private void RotateTurret() {
		float y = Input.GetAxis("Turret") * Time.deltaTime * turretRotSpeed;
		turret.Rotate(0, y, 0);
	}

	[Command]
	private void CmdFire() {
		Bullet tempBullet = ObjectsPool.Instance.GetObject(bulletID) as Bullet;
		tempBullet.Initialize(firepoint);
		tempBullet.GetComponent<Rigidbody>().velocity = tempBullet.transform.forward * bulletSpeed;
		NetworkServer.Spawn(tempBullet.gameObject);
	}
}

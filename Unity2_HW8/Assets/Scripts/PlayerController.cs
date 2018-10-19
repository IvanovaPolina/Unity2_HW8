using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

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
	private Transform playerCam;

	[SerializeField]
	private GameObject dualTouchControls;

	private void Start() {
		if (!isLocalPlayer) {
			var renderers = GetComponentsInChildren<Renderer>();
			foreach (var rend in renderers)
				for (int i = 0; i < rend.materials.Length; i++)
					if (rend.materials[i].color == playerMaterial.color)
						rend.materials[i].color = enemyMaterial.color;
		} else {
#if !UNITY_ANDROID
			if (dualTouchControls)
				Destroy(dualTouchControls);
#endif
			Camera c = Instantiate(Camera.main, Vector3.zero, new Quaternion(0, 0, 0, 1));
			c.transform.SetParent(playerCam, false);
			c.depth = 0;
		}
	}

	private void Update () {
		if (!isLocalPlayer) return;
		MoveAndRotate();
		RotateTurret();
		Fire();
	}

	private void MoveAndRotate() {
#if UNITY_ANDROID
		float y = CrossPlatformInputManager.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
		float z = CrossPlatformInputManager.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
#else
		float y = Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed;
		float z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
#endif
		transform.Rotate(0, y, 0);
		transform.Translate(0, 0, z);
	}

	private void RotateTurret() {
#if UNITY_ANDROID
		float y = CrossPlatformInputManager.GetAxis("Turret") * Time.deltaTime * turretRotSpeed;
#else
		float y = Input.GetAxis("Turret") * Time.deltaTime * turretRotSpeed;
#endif
		turret.Rotate(0, y, 0);
	}

	private void Fire() {
#if UNITY_ANDROID
		bool isFire = CrossPlatformInputManager.GetButtonDown("Fire1");
#else
		bool isFire = Input.GetButtonDown("Fire1");
#endif
		if (isFire) CmdFire();
	}

	[Command]
	private void CmdFire() {
		Bullet tempBullet = ObjectsPool.Instance.GetObject(bulletID) as Bullet;
		tempBullet.Initialize(firepoint);
		tempBullet.GetComponent<Rigidbody>().velocity = tempBullet.transform.forward * bulletSpeed;
		NetworkServer.Spawn(tempBullet.gameObject);
	}
}

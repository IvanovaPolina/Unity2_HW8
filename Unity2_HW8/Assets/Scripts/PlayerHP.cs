using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHP : NetworkBehaviour, IGetDamage {

	[SerializeField]
	private Transform cameraTransform;
	[SerializeField]
	private Transform canvas;
	[SerializeField]
	private Image fillImage;
	[SerializeField]
	private int maxHealth = 100;
	[SyncVar(hook = "ChangeHeath")]
	private int currentHealth = 100;

	private void LateUpdate() {
		canvas.LookAt(cameraTransform, Vector3.up);
	}

	public void GetDamage(int damage) {
		if (!isServer) return;
		if (currentHealth <= 0) return;
		currentHealth -= damage;
		if (currentHealth <= 0) RpcRespawn();
	}

	private void ChangeHeath(int health) {
		currentHealth = health;
		fillImage.fillAmount = (float)currentHealth / maxHealth;
	}

	[ClientRpc]
	private void RpcRespawn() {
		transform.position = NetworkManager.singleton.GetStartPosition().position;
		currentHealth = maxHealth;
	}
}

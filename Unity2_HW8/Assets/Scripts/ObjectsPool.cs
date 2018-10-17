using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
	public static ObjectsPool Instance { get; private set; }

	[SerializeField]
	private GameObject[] prefabs;   // виды объектов, которые будут храниться в пуле
	private Dictionary<string, Queue<IPoolable>> objectsDict = new Dictionary<string, Queue<IPoolable>>(); // для хранения всех объектов данного вида. Dictoinary<PoolID, Queue<кто реализует IPoolable>>

	private void Awake() {
		if (Instance) DestroyImmediate(this);
		else Instance = this;
	}

	private void Start() {
		foreach (var obj in prefabs) {  // заполняем пул
			IPoolable poolable = obj.GetComponent<IPoolable>();
			if (poolable == null) continue;     // если префаб реализует интерфейс IPoolable
			Queue<IPoolable> queue = new Queue<IPoolable>();    // создаем для него очередь объектов
			for (int i = 0; i < poolable.ObjectsCount; i++) {   // определенного количества
				ApplyInPool(obj, queue);
			}
			objectsDict.Add(poolable.PoolID, queue);    // добавляем в словарь очередь с ID-шником
		}
	}

	void ApplyInPool(GameObject obj, Queue<IPoolable> queue) {
		GameObject go = Instantiate(obj);
		go.SetActive(false);
		queue.Enqueue(go.GetComponent<IPoolable>());
	}

	/// <summary>
	/// Возвращает объект из пула по ID этого пула
	/// </summary>
	public IPoolable GetObject(string poolID) {
		if (string.IsNullOrEmpty(poolID)) return null;
		if (!objectsDict.ContainsKey(poolID)) return null;
		IPoolable p = objectsDict[poolID].Dequeue();    // убираем объект из начала очереди
		objectsDict[poolID].Enqueue(p);     // и ставим в конец
		return p;
	}
}
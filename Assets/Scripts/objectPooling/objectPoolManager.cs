using UnityEngine;
using System.Collections.Generic;

public class objectPoolManager : MonoBehaviour
{

    private static objectPoolManager _instance;
    public List<string> objectPoolName = new List<string>();
    private List<GameObject> objectParents = new List<GameObject>();
    public List<GameObject> objectPrefabs = new List<GameObject>();
    private List<GameObject> freeObjects = new List<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();
    
    
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        makeParent();
    }

    
    
    public int makeParent()
    {
        foreach (string name in objectPoolName)
        {
            GameObject parent = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            parent.name = name + "s";
            objectParents.Add(parent);
        }
        if (objectParents.Count == objectPoolName.Count)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public int spawn(string objectName, Vector3 position, Quaternion rotation)
    {
        foreach (GameObject obj in freeObjects)
        {
            if (obj.name == objectName)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                activeObjects.Add(obj);
                freeObjects.Remove(obj);
                return 1;
            }
        }
        foreach (GameObject prefab in objectPrefabs)
        {
            if (prefab.name == objectName)
            {
                GameObject newObj = Instantiate(prefab, position, rotation);
                newObj.name = objectName;
                activeObjects.Add(newObj);
                foreach (GameObject parent in objectParents)
                {
                    if (parent.name == objectName + "s")
                    {
                        newObj.transform.parent = parent.transform;
                        break;
                    }
                }
            }
        }
        return 1;
    }
    public void despawn(GameObject obj)
    {
        obj.SetActive(false);
        activeObjects.Remove(obj);
        freeObjects.Add(obj);
    }

}

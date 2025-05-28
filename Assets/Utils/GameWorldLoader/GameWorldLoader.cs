using UnityEngine;

public class GameWorldLoader : MonoBehaviour
{
    [SerializeField]
    public GameObject GameWorld;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var background = new GameObject("Background");
        background.transform.SetParent(GameWorld.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

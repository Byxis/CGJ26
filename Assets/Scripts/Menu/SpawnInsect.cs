using UnityEngine;

public class SpawnInsect : MonoBehaviour
{

    private float timer = 0f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private GameObject insectPrefab;
    [SerializeField] private Transform startPosition;
    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            GameObject insect = Instantiate(insectPrefab, startPosition.position, Quaternion.identity);
            insect.transform.SetParent(this.transform);
        }
    }
}

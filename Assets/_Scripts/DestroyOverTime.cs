using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    public float lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject,lifeTime);
    }

   
}

using UnityEngine;

public class PlanetController : MonoBehaviour

{

    public float speed;
    public bool invert;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!invert)
            transform.Rotate(new Vector3(0, 0, speed) * Time.deltaTime);
        else
            transform.Rotate(new Vector3(0, 0, speed * (-1)) * Time.deltaTime);

    }
}
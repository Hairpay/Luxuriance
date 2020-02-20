using UnityEngine;

public class Analyzable : MonoBehaviour
{
    [TextArea]
    public string _message;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayMessage()
    {
        Debug.Log(_message);
    }
}
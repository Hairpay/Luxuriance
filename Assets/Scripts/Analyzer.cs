using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analyzer : MonoBehaviour
{
    private float _analysisDeltaTime;
    private const float _analysisTime = 1.0f;
    private Avatar _avatar;

    // Start is called before the first frame update
    private void Start()
    {
        _avatar = GetComponentInParent<Avatar>();
    }

    // Update is called once per frame
    private void Update()
    {
        if( Input.GetButtonDown( "Scan" ) )
        {

        }
    }

    private void CastRay()
    {

    }
}
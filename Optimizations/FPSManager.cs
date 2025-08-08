using UnityEngine;

public class FPSManager : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = -1;
    }
}

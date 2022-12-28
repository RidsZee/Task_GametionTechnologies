/// <Sumery>
/// This class is responsible for:
/// 1. Disabling logs in devices and only showing Erros to boost the performance
/// </Summery>

using UnityEngine;

public class DisableLogging : MonoBehaviour
{
    public bool DisableLogs;

    private void Start()
    {
        if(DisableLogs)
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
  Debug.unityLogger.logEnabled = false;
#endif
        }
    }
}

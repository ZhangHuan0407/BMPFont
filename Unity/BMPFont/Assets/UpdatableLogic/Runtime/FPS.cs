using UnityEngine;

namespace Encoder
{
    public class FPS : MonoBehaviour
    {
        private void OnApplicationFocus(bool focus)
        {
#if UNITY_EDITOR
            Application.targetFrameRate = 60;
#else
            // 我曾经手残在 UnityEditor 内设置帧率为1...
            Application.targetFrameRate = focus ? 60 : 1;
#endif
        }
    }
}
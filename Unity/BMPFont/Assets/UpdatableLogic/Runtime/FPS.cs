using UnityEngine;

namespace Encoder
{
    public class FPS : MonoBehaviour
    {
        private void OnApplicationFocus(bool focus)
        {
            Debug.Log($"OnApplicationFocus, focus : {focus}");
            Application.targetFrameRate = focus ? 60 : 1;
        }
    }
}
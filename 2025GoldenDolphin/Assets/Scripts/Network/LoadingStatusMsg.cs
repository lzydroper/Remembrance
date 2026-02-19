using SKCell;
using UnityEngine;

namespace Network
{
    public class LoadingStatusMsg : MonoBehaviour
    {
        [SerializeField] private SKText loginStatusText;

        public void SetLoginStatusMsg(string msg)
        {
            if (loginStatusText)
            {
                loginStatusText.text = msg;
            }
            // else
            // {
            //     Debug.LogError("Script_LoginStatusMsg: loginStatusText is null");
            // }
        }
    }
}
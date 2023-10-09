using HoshinoLabs.IwaSync3.Udon;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Praecipua.Udon.VideoURL
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class player_iwaSync3 : VideoControllerEventListener
    {
        public VideoCore videoCore;
        public InputField inputField;
        public bool useMessage;

        private void Start()
        {
            if (videoCore == null)
            {
                Debug.LogError("iwaSync3 VideoCore is not set.");
                return;
            }

            videoCore.AddListener(this);
        }

        public override void OnChangeURL()
        {
            string playingURL = videoCore.url.Get();
            // Debug.Log("URL changed. Currently playing URL: " + playingURL);

            inputField.text = playingURL;

            if (useMessage)
            {
                videoCore.Message = playingURL;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Eloi.TextureToServer {
    public class SetServerOfAllImageToFlask: MonoBehaviour
    {
        public string m_serverUrl = "http://localhost:80";
        public GameObject m_childrenToAffect;

        public bool m_serverUrlSetOnAwake = true;

        private void Reset()
        {
            m_childrenToAffect = this.gameObject;
        }
        private void Awake()
        {
            SetServerWithInspectorValue();
        }


        [ContextMenu("Set Server Url in children")]
        private void SetServerWithInspectorValue()
        {
            SetServer(m_serverUrl);
        }

        private void SetServer(string m_serverUrl)
        {
            List<I_ImageToFlaskSettableIP> settableIPs =
                new List<I_ImageToFlaskSettableIP>();
            this.m_childrenToAffect.GetComponentsInChildren<I_ImageToFlaskSettableIP>(true, settableIPs);
            foreach (var settableIP in settableIPs)
            {
                if (settableIP == null) continue;
                settableIP.SetServerUrl(m_serverUrl);
            }
        }
    }

}
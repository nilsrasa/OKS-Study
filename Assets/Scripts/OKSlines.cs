
using QRTracking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OKS
{
    public class OKSlines : MonoBehaviour
    {
        private Material _mat;
        public float _cmSec = 8.9f;
        private float _speed;
        private void Awake()
        {
            _mat = GetComponent<Renderer>().material;
            _speed = (_cmSec / 100f) / transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            float offset = Time.time * _speed;
            _mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}

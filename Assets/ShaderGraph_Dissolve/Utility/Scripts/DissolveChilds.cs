using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DissolveExample
{
    public class DissolveChilds : MonoBehaviour
    {
        // Start is called before the first frame update
        List<Material> materials = new List<Material>();
        bool PingPong = false;
        public bool fadeIn = false;
        void Start()
        {
            var renders = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renders.Length; i++)
            {
                materials.AddRange(renders[i].materials);
            }
        }
        private void OnEnable()
        {
            if (fadeIn)
                value = 0;
            else
                value = 1;
        }
        private void Reset()
        {
            Start();
            SetValue(0);
        }
        float value = 1.0f;
        public float lerpSpeed = 2;
        // Update is called once per frame
        void Update()
        {
            if (fadeIn)
            {
                if (value >= 0.85f)
                    value = 1;
                value = Mathf.Lerp(value, 1, Time.deltaTime * lerpSpeed);
            }
            else
            {
                if (value <= 0.15f)
                    value = 0;
                value = Mathf.Lerp(value,0 ,Time.time * lerpSpeed);
            }
            SetValue(value);
        }

        //IEnumerator enumerator()
        //{

        //    //float value =         while (true)
        //    //{
        //    //    Mathf.PingPong(value, 1f);
        //    //    value += Time.deltaTime;
        //    //    SetValue(value);
        //    //    yield return new WaitForEndOfFrame();
        //    //}
        //}

        public void SetValue(float value)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].SetFloat("_Dissolve", value);
            }
        }
    }
}
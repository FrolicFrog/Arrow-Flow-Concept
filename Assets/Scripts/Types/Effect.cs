using System;
using UnityEngine;

namespace ArrowFlow.Types
{
    [Serializable]
    public class Effect
    {
        public string name;
        public ParticleSystem[] Particles;   

        public void Play()
        {
            Array.ForEach(Particles, P => P.Play());
        }    
    }    
}
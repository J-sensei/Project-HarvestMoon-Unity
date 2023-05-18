using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class Tool : MonoBehaviour
    {
        [SerializeField] private Transform attachTransform;
        public Transform AttachTransform { get { return attachTransform; } }
    }
}

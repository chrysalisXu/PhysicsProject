using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ClothPhysics
{
    public enum ObjectType
    {
        GroundPlane, // consider its mass to be very large
        Sphere
    }

    public class PhysicsObject : MonoBehaviour
    {
        
        public ObjectType ObjType;

        public float mass;
        [NonSerialized]
        public Transform trans;

        void Start()
        {
            trans = GetComponent<Transform>();
            PhysicsEngine.Instance.Register(this);
        }
        
        void Update()
        {
            
        }
    }
}


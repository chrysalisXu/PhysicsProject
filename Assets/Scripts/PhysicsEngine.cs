using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ClothPhysics
{
    public class PhysicsEngine : MonoBehaviour
    {
        // singleton
        private static PhysicsEngine _instance = null;
        public static PhysicsEngine Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<PhysicsEngine>();
                if (_instance == null) throw new Exception($"Please add PhysicsEngine Script");
                return _instance;
            }
        }

        public List<PhysicsObject> ObjectList = new List<PhysicsObject>();

        [Header("Base")]
        public float TimeStep;
        public float GravityConstant;
        public float DragConstant;
        [Header("Wind")]
        public Vector3 WindSpeed;

        public void Register(PhysicsObject obj){
            ObjectList.Add(obj);
        }
    }
}


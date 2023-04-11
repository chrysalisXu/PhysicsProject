using UnityEngine;
using System;

namespace ClothPhysics
{
    public struct Spring
    {
        public int V1, V2;
        public float originalLength;
        public Spring(int v1, int v2, float l)
        {
            V1 = v1;
            V2 = v2;
            originalLength = l;
        }
    }

    public enum FixedType
    {
        TopCorners,
        TopEdge,
        LeftCorners,
        UpperLeft,
        None
    }

    public class Cloth : MonoBehaviour
    {
        [Tooltip("spring constant")]
        public float springCoefficient;
        [Tooltip("damping constant")]
        public float dampingCoefficient;
        [Tooltip("Vertices number per edge / 2")]
        public int HalfSideLength;
        [Tooltip("whether fixed")]
        public FixedType FixPos;
        [Tooltip("mass")]
        public float mass;
        [Tooltip("Plasticity")]
        public float plasticity;

        // init
        private Vector3 Center;
        private Vector2 Size;
        private Quaternion OriRotation;
        private float VerticesMass;
        // kept property
        private Mesh ClothMesh;
        private Spring[] Springs;
        private Vector3[] Velocities;
        private Vector3[] Vertices;

        void Start(){
            SetTransform();
            InitMesh();
            InitSpring();
            InitOthers();
        }

        void Update(){
            Vertices = ClothMesh.vertices;
            UpdateVelocity();
            UpdatePosition();
            HandleCollision();
            ClothMesh.vertices = Vertices;
        }

        private void SetTransform(){
            var trans = GetComponent<Transform>();
            Center = trans.position;
            trans.position = new Vector3(0, 0, 0);
            Size = new Vector2 (trans.localScale.x * 5, trans.localScale.z * 5);
            trans.localScale = new Vector3(1, 1, 1);
            OriRotation = trans.rotation;
            trans.rotation = Quaternion.identity;
        }

        private void InitMesh(){
            // Vertices
            Vertices = new Vector3[(2 * HalfSideLength + 1) * (2 * HalfSideLength + 1)];
            for (int i = 0; i <= 2 * HalfSideLength; i++){
                for (int j = 0; j <= 2 * HalfSideLength; j++){
                    var vectorFromCenter = new Vector3(
                        (j - HalfSideLength) * (Size.x / HalfSideLength), 
                        0,
                        (i - HalfSideLength) * (Size.y / HalfSideLength)
                    );
                    Vertices[i * (2 * HalfSideLength + 1) + j] = Center + OriRotation * vectorFromCenter;
                }
            }

            // UVs
            Vector2[] uv = new Vector2[Vertices.Length];
            for (int i = 0; i <= 2 * HalfSideLength; i++){
                for (int j = 0; j <= 2 * HalfSideLength; j++){
                    uv[i * (2 * HalfSideLength + 1) + j] = new Vector2(
                        1 - (float)j / (float)(HalfSideLength * 2),
                        1 - (float)i / (float)(HalfSideLength * 2)
                    );
                }
            }

            // triangles
            int[] triangles = new int[3 * 8 * HalfSideLength * HalfSideLength];
            for (int i = 0; i < 2 * HalfSideLength; i++){
                for (int j = 0; j < 2 * HalfSideLength; j++){
                    int startIndex = (i * 2 * HalfSideLength + j) * 6; 
                    triangles[startIndex] = i * (2 * HalfSideLength + 1) + j;
                    triangles[startIndex + 1] = i * (2 * HalfSideLength + 1) + (j+1);
                    triangles[startIndex + 2] = (i+1) * (2 * HalfSideLength + 1) + j;
                    triangles[startIndex + 3] = i * (2 * HalfSideLength + 1) + (j+1);
                    triangles[startIndex + 4] = (i+1) * (2 * HalfSideLength + 1) + (j+1);
                    triangles[startIndex + 5] = (i+1) * (2 * HalfSideLength + 1) + j;
                }
            }

            // assign mesh
            ClothMesh = GetComponent<MeshFilter>().mesh;
            ClothMesh.Clear();
            ClothMesh.vertices = Vertices;
            ClothMesh.uv = uv;
            ClothMesh.triangles = triangles;
            ClothMesh.RecalculateNormals();
        }

        private void InitSpring(){
            Springs = new Spring[4 * 4 * HalfSideLength * HalfSideLength + 4 * HalfSideLength];
            for (int i = 0; i < 2 * HalfSideLength; i++){
                for (int j = 0; j < 2 * HalfSideLength; j++){
                    int startSpringIndex = (i * 2 * HalfSideLength + j) * 4;
                    Springs[startSpringIndex] = new Spring(
                        i * (2 * HalfSideLength + 1) + j,
                        i * (2 * HalfSideLength + 1) + (j+1),
                        Size.x / HalfSideLength
                    );
                    Springs[startSpringIndex+1] = new Spring(
                        i * (2 * HalfSideLength + 1) + j,
                        (i+1) * (2 * HalfSideLength + 1) + j,
                        Size.y / HalfSideLength
                    );
                    Springs[startSpringIndex+2] = new Spring(
                        i * (2 * HalfSideLength + 1) + j,
                        (i+1) * (2 * HalfSideLength + 1) + (j+1),
                        MathF.Sqrt(Size.x * Size.x + Size.y * Size.y) / HalfSideLength
                    );
                    Springs[startSpringIndex+3] = new Spring(
                        (i+1) * (2 * HalfSideLength + 1) + j,
                        i * (2 * HalfSideLength + 1) + (j+1),
                        MathF.Sqrt(Size.x * Size.x + Size.y * Size.y) / HalfSideLength
                    );
                }
            }

            for (int i = 0; i < HalfSideLength; i++){
                int startSpringIndex = 4 * 4 * HalfSideLength * HalfSideLength + 4 * i;
                // lower edge
                Springs[startSpringIndex] = new Spring(
                    (2 * HalfSideLength + 1) * (2 * HalfSideLength) + i,
                    (2 * HalfSideLength + 1) * (2 * HalfSideLength) + (i+1),
                    Size.x / HalfSideLength
                );
                Springs[startSpringIndex+1] = new Spring(
                    (2 * HalfSideLength + 1) * (2 * HalfSideLength) + HalfSideLength + i,
                    (2 * HalfSideLength + 1) * (2 * HalfSideLength) + HalfSideLength + (i+1),
                    Size.y / HalfSideLength
                );
                // right edge
                Springs[startSpringIndex+2] = new Spring(
                    i * (2 * HalfSideLength + 1) + 2 * HalfSideLength,
                    (i+1) * (2 * HalfSideLength + 1) + 2 * HalfSideLength,
                    Size.y / HalfSideLength
                );
                Springs[startSpringIndex+3] = new Spring(
                    (i+HalfSideLength) * (2 * HalfSideLength + 1) + 2 * HalfSideLength,
                    (i+HalfSideLength+1) * (2 * HalfSideLength + 1) + 2 * HalfSideLength,
                    Size.y / HalfSideLength
                );
            }
        }

        private void InitOthers(){
            // velocities
            Velocities = new Vector3[ClothMesh.vertices.Length];
            for (int i = 0; i < Velocities.Length; i++){
                Velocities[i] = new Vector3(0,0,0);
            }
            VerticesMass = mass / Velocities.Length;
        }

        private void UpdateVelocity(){
            Vector3[] LastVelocities = new Vector3[ClothMesh.vertices.Length];
            Array.Copy(Velocities, LastVelocities, ClothMesh.vertices.Length);

            // add gravity
            for (int i = 0; i < Velocities.Length; i++){
                Velocities[i].y -= PhysicsEngine.Instance.GravityConstant * PhysicsEngine.Instance.TimeStep;
            }

            // spring system
            for (int i=0; i<Springs.Length; i++){
                // for v1 to v2
                Vector3 vector = Vertices[Springs[i].V2] - Vertices[Springs[i].V1];
                Vector3 relativeVelocity = LastVelocities[Springs[i].V2] - LastVelocities[Springs[i].V1];
                float velocityChange = (vector.magnitude - Springs[i].originalLength) * springCoefficient;
                velocityChange += Vector3.Dot(relativeVelocity, vector.normalized) * dampingCoefficient;
                velocityChange = velocityChange / VerticesMass * PhysicsEngine.Instance.TimeStep;
                Velocities[Springs[i].V1] += velocityChange * vector.normalized;
                Velocities[Springs[i].V2] -= velocityChange * vector.normalized;
                Springs[i].originalLength += (vector.magnitude - Springs[i].originalLength) * plasticity;
            }

            // Drag forces
            for (int i = 0; i < Velocities.Length; i++){
                Velocities[i] -= (Velocities[i] - PhysicsEngine.Instance.WindSpeed) * PhysicsEngine.Instance.DragConstant / VerticesMass * PhysicsEngine.Instance.TimeStep;
            }

            // fixed correct
            switch (FixPos){
                case FixedType.TopCorners:
                    Velocities[0].Set(0, 0, 0);
                    Velocities[2 * HalfSideLength].Set(0, 0, 0);
                    break;
                case FixedType.TopEdge:
                    for (int i = 0; i <= 2 * HalfSideLength; i++)
                        Velocities[i].Set(0, 0, 0);
                    break;
                case FixedType.LeftCorners:
                    Velocities[0].Set(0, 0, 0);
                    Velocities[(2 * HalfSideLength) * (2 * HalfSideLength + 1)].Set(0, 0, 0);
                    break;
                case FixedType.UpperLeft:
                    Velocities[0].Set(0, 0, 0);
                    break;
                case FixedType.None:
                    break;
            }
        }

        private void UpdatePosition(){
            for (int i = 0; i < Velocities.Length; i++){
                Vertices[i] += Velocities[i] * PhysicsEngine.Instance.TimeStep;
            }
        }

        private void HandleCollision(){
            foreach (PhysicsObject obj in PhysicsEngine.Instance.ObjectList){
                for (int i = 0; i < Velocities.Length; i++){
                    switch (obj.ObjType){
                        case ObjectType.GroundPlane:
                            float height = Vertices[i].y - obj.trans.position.y;
                            if (height <= 0){
                                Vertices[i].y = obj.trans.position.y;
                                Velocities[i].y = -Velocities[i].y;
                            }
                            break;
                        case ObjectType.Sphere:
                            Vector3 distance = Vertices[i] - obj.trans.position;
                            if (distance.magnitude <= obj.trans.localScale.x / 2){
                                Vertices[i] += (obj.trans.localScale.x / 2 - distance.magnitude) * distance.normalized;
                                var relativeVelocity = Vector3.Dot(Velocities[i], distance.normalized) * distance.normalized;
                                if (Vector3.Dot(Velocities[i], distance.normalized) < 0){
                                    Velocities[i] -= 2 * relativeVelocity;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

}

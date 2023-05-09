
// public class PointInPolyhedronTest : MonoBehaviour
// {
//     // The eight vertices of the polyhedron
//     public Vector3 a, b, c, d, e, f, g, h;

//     // The point to test
//     public Vector3 x;

//     void Start()
//     {
//         // Define the six faces of the polyhedron
//         Vector3[] faceABCD = { a, b, c, d };
//         Vector3[] faceADHE = { a, d, h, e };
//         Vector3[] faceEFGH = { e, f, g, h };
//         Vector3[] faceBFHG = { b, f, h, g };
//         Vector3[] faceACGF = { a, c, g, f };
//         Vector3[] faceBDEC = { b, d, e, c };

//         // Test if the point is inside any of the six faces
//         bool isInside = PointInPolyhedron(x, faceABCD) ||
//                         PointInPolyhedron(x, faceADHE) ||
//                         PointInPolyhedron(x, faceEFGH) ||
//                         PointInPolyhedron(x, faceBFHG) ||
//                         PointInPolyhedron(x, faceACGF) ||
//                         PointInPolyhedron(x, faceBDEC);

//         Debug.Log(isInside);
//     }

//     // Check if a point is inside a convex polyhedron defined by its vertices
//     bool PointInPolyhedron(Vector3 point, Vector3[] vertices)
//     {
//         // Compute the normal of the plane defined by the first three vertices
//         Vector3 normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]);

//         // Compute the signed distance between the point and the plane
//         float signedDistance = Vector3.Dot(point - vertices[0], normal);

//         // If the point is on the "outside" side of the plane, it is not inside the polyhedron
//         if (signedDistance < 0)
//             return false;

//         // Check if the point is inside each triangle formed by the remaining vertices
//         for (int i = 0; i < vertices.Length - 2; i++)
//         {
//             Vector3 v1 = vertices[i + 1] - vertices[0];
//             Vector3 v2 = vertices[i + 2] - vertices[0];
//             Vector3 v3 = point - vertices[0];

//             // Compute the barycentric coordinates of the point with respect to the triangle
//             float dot11 = Vector3.Dot(v1, v1);
//             float dot12 = Vector3.Dot(v1, v2);
//             float dot13 = Vector3.Dot(v1, v3);
//             float dot22 = Vector3.Dot(v2, v2);
//             float dot23 = Vector3.Dot(v2, v3);
//             float invDenom = 1 / (dot11 * dot22 - dot12 * dot12);
//             float u = (dot22 * dot13 - dot12 * dot23) * invDenom;
//             float v = (dot11 * dot23 - dot12 * dot13) * invDenom;

//             // If the barycentric coordinates are both non-negative and their sum is less than 1, 
//             // the point is inside the triangle and thus inside the polyhedron
//             if (u >= 0 && v >= 0 && u + v < 1)
//             return true;
//     }

//     // If the point is not inside any of the triangles, it is not inside the polyhedron
//     return false;
//     }
// }

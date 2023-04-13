// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;

// public class DartThrowing
// {
//     public static List<Vector2> GenerateRandomPoints(float minX, float minY, float maxX, float maxY, float minDistance, int maxIterations)
//     {
//         List<List<Rect>> activeLists = new List<List<Rect>>();
//         activeLists.Add(new List<Rect> { new Rect(minX, minY, maxX - minX, maxY - minY) });
//         List<Vector2> pointSet = new List<Vector2>();
//         System.Random random = new System. Random();

//         for (int i = 0; i < activeLists.Count && i < maxIterations; i++)
//         {
//             List<Rect> activeSquares = activeLists[i];

//             if (activeSquares.Count == 0)
//             {
//                 continue;
//             }

//             float totalArea = 0;
//             foreach(Rect square in activeSquares) totalArea+= square.width * square.height;
//             float randomArea = (float)random.NextDouble() * totalArea;

//             Rect chosenSquare = null;
//             float areaSum = 0;

//             for (int j = 0; j < activeSquares.Count; j++)
//             {
//                 Rect square = activeSquares[j];
//                 areaSum += square.width * square.height;

//                 if (areaSum >= randomArea)
//                 {
//                     chosenSquare = square;
//                     activeSquares.RemoveAt(j);
//                     break;
//                 }
//             }

//             if (chosenSquare == null)
//             {
//                 continue;
//             }

//             bool isCovered = false;

//             foreach (Vector2 point in pointSet)
//             {
//                 if (chosenSquare.Contains(point))
//                 {
//                     isCovered = true;
//                     break;
//                 }
//             }

//             if (!isCovered)
//             {
//                 Vector2 point = new Vector2(
//                     (float)random.NextDouble() * chosenSquare.width + chosenSquare.x,
//                     (float)random.NextDouble() * chosenSquare.height + chosenSquare.y
//                 );

//                 bool isValid = true;

//                 foreach (Vector2 existingPoint in pointSet)
//                 {
//                     float distance = Vector2.Distance(point, existingPoint);

//                     if (distance < minDistance)
//                     {
//                         isValid = false;
//                         break;
//                     }
//                 }

//                 if (isValid)
//                 {
//                     pointSet.Add(point);
//                 }
//                 else
//                 {
//                     float subWidth = chosenSquare.width / 2f;
//                     float subHeight = chosenSquare.height / 2f;

//                     activeLists.AddRange(new List<Rect> {
//                         new Rect(chosenSquare.x, chosenSquare.y, subWidth, subHeight),
//                         new Rect(chosenSquare.x + subWidth, chosenSquare.y, subWidth, subHeight),
//                         new Rect(chosenSquare.x, chosenSquare.y + subHeight, subWidth, subHeight),
//                         new Rect(chosenSquare.x + subWidth, chosenSquare.y + subHeight, subWidth, subHeight)
//                     });
//                 }
//             }
//         }

//         return pointSet;
//     }
// }

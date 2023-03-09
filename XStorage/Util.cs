using System;
using UnityEngine;

namespace XStorage
{
    internal class Util
    {
        #region String to Vector
        // Slightly modified version of Jessespike's answer at:
        // https://answers.unity.com/questions/1134997/string-to-vector3.html?childToView=1135010#answer-1135010
        public static Vector3 StringToVector3(string sVector)
        {
            // Remove the parentheses
            sVector = sVector.Trim("()".ToCharArray());

            // Split the items
            string[] sParts = sVector.Split(',');

            // Parse string[] to float[]
            var fParts = Array.ConvertAll(sParts, float.Parse);

            return new Vector3(fParts[0], fParts[1], fParts[2]);
        }

        public static Vector3Int StringToVector3Int(string sVector)
        {
            // Remove the parentheses
            sVector = sVector.Trim("()".ToCharArray());

            // Split the items
            string[] sParts = sVector.Split(',');

            // Parse string[] to float[]
            var iParts = Array.ConvertAll(sParts, int.Parse);

            return new Vector3Int(iParts[0], iParts[1], iParts[2]);
        }

        public static Vector2 StringToVector2(string sVector)
        {
            // Remove the parentheses
            sVector = sVector.Trim("()".ToCharArray());

            // Split the items
            string[] sParts = sVector.Split(',');

            // Parse string[] to float[]
            var fParts = Array.ConvertAll(sParts, float.Parse);

            return new Vector2(fParts[0], fParts[1]);
        }

        public static Vector2Int StringToVector2Int(string sVector)
        {
            // Remove the parentheses
            sVector = sVector.Trim("()".ToCharArray());

            // Split the items
            string[] sParts = sVector.Split(',');

            // Parse string[] to float[]
            var iParts = Array.ConvertAll(sParts, int.Parse);

            return new Vector2Int(iParts[0], iParts[1]);
        }
        #endregion
    }
}

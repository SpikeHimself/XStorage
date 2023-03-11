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
            // Split the items
            string[] sParts = sVector.Split(", ()".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Parse string[] to float[]
            var fParts = Array.ConvertAll(sParts, float.Parse);

            return new Vector3(fParts[0], fParts[1], fParts[2]);
        }
        #endregion
    }
}

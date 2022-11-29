using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-11-27


public class JsonEntries : MonoBehaviour
{
    public List<string> parents;

    [System.Serializable]
    public struct connectionEntry
    {
        public int fromChild;
        public int fromParent;
        public int toChild;
        public int toParent;

        public connectionEntry(int fromC, int fromP, int toC, int toP)
        {
            this.fromChild = fromC;
            this.fromParent = fromP;
            this.toChild = toC;
            this.toParent = toP;
        }
    }
    public List<connectionEntry> connectionsList;


    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        parents.Clear();   
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

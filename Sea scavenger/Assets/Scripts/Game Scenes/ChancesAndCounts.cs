using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChancesAndCounts
{
    [SerializeField]
    private List<ChanceCount> chancesAndCounts;

    public int GetCount()
    {
        return ((ChanceCount)Roll.GetRandomElement(
            chancesAndCounts, 1f)).Count;
    }

    [System.Serializable]
    private class ChanceCount : IRollable
    {
        [SerializeField]
        private float chance;
        [SerializeField]
        private int count;

        public float Chance => chance;
        public int Count => count;
    }
}

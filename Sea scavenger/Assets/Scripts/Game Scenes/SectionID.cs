using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SectionID
{
    [SerializeField]
    private int section1;
    [SerializeField]
    private int section2;
    [SerializeField]
    private int section3;
    [SerializeField]
    private int section4;

    public int Section1 => section1;
    public int Section2 => section2;
    public int Section3 => section3;
    public int Section4 => section4;

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return section1.ToString() + section2.ToString() +
            section3.ToString() + section4.ToString();
    }

    public static bool operator ==(SectionID lhs, SectionID rhs)
    {
        return lhs.Section1 == rhs.Section1 && lhs.Section2 == rhs.Section2 &&
            lhs.Section3 == rhs.Section3 && lhs.Section4 == rhs.Section4;
    }

    public static bool operator !=(SectionID lhs, SectionID rhs)
    {
        return !(lhs == rhs);
    }

    public static explicit operator int(SectionID sectionID)
    {
        return int.Parse(sectionID.ToString());
    }
}

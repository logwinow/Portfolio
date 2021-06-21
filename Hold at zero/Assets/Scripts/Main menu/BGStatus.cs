using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BGStatus
{
    public enum Status
    {
        Closed = 0,
        Opened = 1
    }

    public static Status status = Status.Opened;
}

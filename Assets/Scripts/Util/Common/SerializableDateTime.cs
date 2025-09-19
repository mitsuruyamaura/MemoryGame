using System;
using UnityEngine;

[Serializable]
public class SerializableDateTime {
    [SerializeField]
    private string dateTimeString;

    public DateTime DateTimeValue {
        get => DateTime.TryParse(dateTimeString, out var result) ? result : System.DateTime.MinValue;
        set => dateTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
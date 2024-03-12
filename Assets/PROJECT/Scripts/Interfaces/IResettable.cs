using System;
using System.Collections.Generic;

public interface IResettable
{
    List<Action> ResetActions { get; set; }

    public void ResetValue<T>(T initialValue, ref T reference);

    public void ResetObject();
}
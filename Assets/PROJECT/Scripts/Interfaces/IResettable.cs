using System;
using System.Collections.Generic;

public interface IResettable
{
    List<Action> ResetActions { get; set; }

    public void ResetObject();
}
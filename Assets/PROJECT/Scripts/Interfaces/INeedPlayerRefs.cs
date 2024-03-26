using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INeedPlayerRefs 
{
 void FetchPlayerRefs(Rigidbody rb, GrindController _gc = null);
}

using BzKovSoft.ObjectSlicer;
using UnityEngine;

public class SliceTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        var sliceableA = transform.GetComponentInParent<IBzMeshSlicer>();

        Plane plane = new Plane(Vector3.zero, Vector3.up);

        if (sliceableA != null)
        {
            await sliceableA.SliceAsync(plane);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

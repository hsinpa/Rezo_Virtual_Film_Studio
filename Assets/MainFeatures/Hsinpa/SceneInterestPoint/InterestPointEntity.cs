using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Entity
{
    public class InterestPointEntity : MonoBehaviour
    {

        private Vector3 _line_start_pos;
        private Vector3 _line_end_pos;

        private void Start()
        {
            float line_scale = 2;
            _line_start_pos = transform.position;
            _line_end_pos = transform.position + (transform.forward * line_scale);
        }
    }
}

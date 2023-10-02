using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Entity
{
    public class InterestPointManager : MonoBehaviour
    {
        InterestPointEntity[] _interestPointEntities;
        public int Length => (_interestPointEntities == null) ? 0 : _interestPointEntities.Length;

        public void SetUp()
        {
            _interestPointEntities = transform.GetComponentsInChildren<InterestPointEntity>();  
        }

        public Transform GetTrasformPoint(int i)
        {
            if (i >= _interestPointEntities.Length || i < 0) return null;

            return _interestPointEntities[i].transform;
        }
    }
}
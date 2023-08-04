using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        // Start is called before the first frame update
        void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = target.transform.position;
        }
    }
}

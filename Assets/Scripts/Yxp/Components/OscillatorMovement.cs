using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yxp.Behaviours
{
    public class OscillatorMovement : MonoBehaviour
    {
        const float TAU = 2 * Mathf.PI;     // constant value of 6.283

        [SerializeField] Vector3 _movementVector;
        [SerializeField] [Range(1, 10)] float _periodInSeconds = 3f;

        Vector3 _startingCenterPosition;
        float _movementFactor;


        void Start()
        {
            _startingCenterPosition = transform.position;
        }


        void Update()
        {
            Oscillate();
        }

        void Oscillate()
        {
            float secondsSinceGameStarted = Time.time;

            // constantly growing over time
            float cycles = secondsSinceGameStarted / _periodInSeconds;

            // going from -1 ot 1
            _movementFactor = Mathf.Sin(cycles * TAU);

            // movement offset
            Vector3 offset = _movementVector * _movementFactor;

            // apply movement
            transform.position = _startingCenterPosition + offset;
        }
    }
}
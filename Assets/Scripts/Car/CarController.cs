using UnityEngine;

namespace Car
{
    public abstract class CarController: MonoBehaviour
    {

        public abstract bool IsBraking { get; }
        public abstract Vector2 Controls { get; }

    }
}
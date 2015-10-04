using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.logic.Domain
{
    public interface iPhysics
    {
        // A contract with any class that implements this interface
        // on which methods it MUST implement itself
        void Move();
        void ApplyFriction();
        void ApplyGravity();
        void AddVelocity(Vector2 velocity);
    }
}

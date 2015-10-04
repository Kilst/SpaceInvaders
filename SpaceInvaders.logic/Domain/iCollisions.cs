using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders.logic.Domain
{
    public interface iCollisions
    {
        void CollisionCheck(List<GameObject> list);
    }
}

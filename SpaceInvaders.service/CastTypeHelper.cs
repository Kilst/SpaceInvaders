using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//..
using System.Drawing;
using SpaceInvaders.logic.Domain;

namespace SpaceInvaders.service
{
    public class CastTypeHelper
    {
        public static Bitmap EnemyNPCFlip(GameObject obj, Bitmap b)
        {
            Enemy enemy = (Enemy)obj;
            return enemy.FlipNPCImage(b);
        }

        public static string CheckType(GameObject obj)
        {
            if (obj.GetType() == typeof(BulletBill))
            {
                return "BulletBill";
            }
            if (obj.GetType() == typeof(Goomba))
            {
                return "Goomba";
            }
            if (obj.GetType() == typeof(KoopaGreen))
            {
                return "KoopaGreen";
            }

            return "GameObject";
        }
    }
}

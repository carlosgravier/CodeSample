using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class HitState
    {
        public enum HitValue
        {
            Empty,
            Hit,
            Missed
        }

        public HitValue WasHit { get; set; }

        public HitState()
        {
            this.WasHit = HitValue.Empty;
        }

    }
}

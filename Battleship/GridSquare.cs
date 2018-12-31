using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Battleship
{
    public class GridSquare
    {
        public HitState.HitValue WasHit { get; set; }
        public Rectangle _rectangle { get; set; }

        public GridSquare(Rectangle rectangle, HitState.HitValue wasHit)
        {
            this.WasHit = HitState.HitValue.Empty;
            this._rectangle = rectangle;
        }

    }
}

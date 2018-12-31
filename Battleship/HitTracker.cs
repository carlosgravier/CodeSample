using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class HitTracker
    {
        public enum TrackingDirection
        {
            Neutral,
            Up,
            Down,
            Left,
            Right
        }

        public GridSquare gridSquare;
        public bool hasTriedLeft;
        public bool hasTriedRight;
        public bool hasTriedUp;
        public bool hasTriedDown;
        public bool lastTryWasSuccessful;
        public TrackingDirection direction;

        public HitTracker(GridSquare gridSquare)
        {
            this.gridSquare = gridSquare;
            hasTriedLeft = false;
            hasTriedRight = false;
            hasTriedUp = false;
            hasTriedDown = false;
            lastTryWasSuccessful = false;
            direction = TrackingDirection.Neutral;
        }

    }
}

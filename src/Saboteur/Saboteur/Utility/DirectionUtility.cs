using log4net;
using Saboteur.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Saboteur.Utility
{
    class DirectionUtility
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                default:
                    throw new ArgumentException(
                        new StringBuilder().Append(nameof(Direction)).Append(".").Append(nameof(GetOppositeDirection)).ToString(),
                        nameof(direction));
            }
        }
    }
}

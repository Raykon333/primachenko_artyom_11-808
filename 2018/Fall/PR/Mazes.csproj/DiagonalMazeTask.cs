using System;

namespace Mazes
{
    public static class DiagonalMazeTask
    {
        public static void MoveOut(Robot robot, int width, int height)
        {
            Direction primaryDirection;
            if (width > height) //в какую сторону делать первый шаг
                primaryDirection = Direction.Right;
            else
                primaryDirection = Direction.Down;
            Direction secondaryDirection = (Direction)Math.Abs((int)primaryDirection - 4);
            DoStep(robot, Math.Min(width, height), Math.Max(width, height), primaryDirection);
            for (int i = 0; i < Math.Min(width, height) - 3; i++) //L-образные переходы
            {
                DoStep(robot, Math.Max(width, height), Math.Min(width, height), secondaryDirection);
                DoStep(robot, Math.Min(width, height), Math.Max(width, height), primaryDirection);
            }
        }

        //при шаге вниз и поиске шага вниз, primaryDimension - height; 
        //при шаге вправо и поиске шага вправо, primaryDimension - width

        public static int FindStep(int primaryDimension, int secondDimension)
        {
            int step = 1;
            if (secondDimension > primaryDimension)
            {
                while (secondDimension - 2 > step * (primaryDimension - 2) + 1)
                    step++;
            }
            return step;
        }

        public static void DoStep(Robot robot, int primaryDimension, int secondaryDimension, Direction direction)
        {
            for (int i = 0; i < FindStep(primaryDimension, secondaryDimension); i++)
                robot.MoveTo(direction);
        }
    }
}
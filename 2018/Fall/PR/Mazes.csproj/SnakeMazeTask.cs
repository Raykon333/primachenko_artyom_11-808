namespace Mazes
{
	public static class SnakeMazeTask
	{
		public static void MoveOut(Robot robot, int width, int height)
		{
            Direction y = Direction.Right; //переменная, поочерёдно меняющая движение влево и вправо
            MoveToSide(robot, y, width);
            for (int i = 1; i < (height - 1) / 2; i++)
            {
                y = (Direction)System.Math.Abs((int)y - 5); //изменение направления
                robot.MoveTo(Direction.Down);
                robot.MoveTo(Direction.Down);
                MoveToSide(robot, y, width);
            }
		}

        public static void MoveToSide(Robot robot, Mazes.Direction direction, int width)
        {
            for(int i = 1; i < width - 2; i++)
            {
                robot.MoveTo(direction);
            }
        }
	}
}
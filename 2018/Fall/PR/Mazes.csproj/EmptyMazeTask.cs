namespace Mazes
{
	public static class EmptyMazeTask
	{
		public static void MoveOut(Robot robot, int width, int height)
		{
            MoveToWall(robot, width, Direction.Right);
            MoveToWall(robot, height, Direction.Down);
        }

        public static void MoveToWall(Robot robot, int dimension, Direction direction)
        {
            for (int i = 1; i < dimension - 2; i++)
                robot.MoveTo(direction);
        }
	}
}
namespace Pluralize
{
	public static class PluralizeTask
	{
		public static string PluralizeRubles(int count)
		{
            int x = count % 10;
            if ((count % 100 >= 11) && (count % 100 <= 14)) x = 0;
			switch (x)
            {
                case 1:
                    return "рубль";
                case 2:
                    return "рубля";
                case 3:
                    return "рубля";
                case 4:
                    return "рубля";
                default:
                    return "рублей";
            }
		}
	}
}
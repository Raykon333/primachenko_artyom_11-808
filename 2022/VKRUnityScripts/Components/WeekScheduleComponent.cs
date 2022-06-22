using Unity.Entities;

public struct WeekScheduleComponent : IComponentData
{
    public TimeInterval MondaySchedule;
    public TimeInterval TuesdaySchedule;
    public TimeInterval WednesdaySchedule;
    public TimeInterval ThursdaySchedule;
    public TimeInterval FridaySchedule;
    public TimeInterval SaturdaySchedule;
    public TimeInterval SundaySchedule;

    public TimeInterval this[int day]
    {
        get
        {
            TimeInterval returnSchedule = default;
            switch (day % 7)
            {
                case 0:
                    returnSchedule = SundaySchedule;
                    break;
                case 1:
                    returnSchedule = MondaySchedule;
                    break;
                case 2:
                    returnSchedule = TuesdaySchedule;
                    break;
                case 3:
                    returnSchedule = WednesdaySchedule;
                    break;
                case 4:
                    returnSchedule = ThursdaySchedule;
                    break;
                case 5:
                    returnSchedule = FridaySchedule;
                    break;
                case 6:
                    returnSchedule = SaturdaySchedule;
                    break;
            }
            return returnSchedule;
        }
        set
        {
            switch (day % 7)
            {
                case 0:
                    SundaySchedule = value;
                    break;
                case 1:
                    MondaySchedule = value;
                    break;
                case 2:
                    TuesdaySchedule = value;
                    break;
                case 3:
                    WednesdaySchedule = value;
                    break;
                case 4:
                    ThursdaySchedule = value;
                    break;
                case 5:
                    FridaySchedule = value;
                    break;
                case 6:
                    SaturdaySchedule = value;
                    break;
            }
        }
    }
}

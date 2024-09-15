using System;
using System.Collections.Generic;
using System.Linq;
using congestion.calculator;

/**
     * Calculate the total toll fee for one day
     *
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total congestion tax for that day
     */


#region my change source
public class CongestionTaxCalculator
{

    private static readonly Dictionary<(int hour, int minute), int> TollRates = new()
    {
        {(6, 0), 8}, {(6, 30), 13}, {(7, 0), 18}, {(8, 0), 13}, {(8, 30), 8},
        {(15, 0), 13}, {(15, 30), 18}, {(17, 0), 13}, {(18, 0), 8}
    };

    private static readonly HashSet<DateTime> PublicHolidays = new()
    {
        new DateTime(2013, 1, 1), new DateTime(2013, 3, 28), new DateTime(2013, 3, 29),
        new DateTime(2013, 4, 1), new DateTime(2013, 5, 1), new DateTime(2013, 5, 8),
        new DateTime(2013, 6, 5), new DateTime(2013, 6, 6), new DateTime(2013, 6, 21),
        new DateTime(2013, 11, 1), new DateTime(2013, 12, 24), new DateTime(2013, 12, 25),
        new DateTime(2013, 12, 26), new DateTime(2013, 12, 31)
    };

    private static readonly HashSet<DayOfWeek> Weekends = new()
    {
        DayOfWeek.Saturday, DayOfWeek.Sunday
    };

    public int GetTax(Vehicle vehicle, DateTime[] dates)
    {
        if (IsTollFreeVehicle(vehicle))
            return 0;

        var sortedDates = dates.OrderBy(d => d).ToArray();
        int totalFee = 0;
        DateTime intervalStart = sortedDates[0];
        int dailyFee = 0;

        foreach (var date in sortedDates)
        {
            int currentFee = GetTollFee(date);

            if ((date - intervalStart).TotalMinutes <= 60)
            {
                dailyFee = Math.Max(dailyFee, currentFee);
            }
            else
            {
                totalFee += dailyFee;
                intervalStart = date;
                dailyFee = currentFee;
            }
        }
        totalFee += dailyFee;
        return Math.Min(totalFee, 60);
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
            return false;

        var tollFreeVehicleTypes = new HashSet<string>
        {
            "Motorbike", "Tractor", "Emergency", "Diplomat", "Foreign", "Military"
        };

        return tollFreeVehicleTypes.Contains(vehicle.GetVehicleType());
    }

    private int GetTollFee(DateTime date)
    {
        if (IsTollFreeDate(date))
            return 0;

        var hour = date.Hour;
        var minute = date.Minute;

        return TollRates
            .Where(rate => hour == rate.Key.hour && minute >= rate.Key.minute)
            .Select(rate => rate.Value)
            .DefaultIfEmpty(0)
            .Max();
    }

    private bool IsTollFreeDate(DateTime date)
    {
        if (Weekends.Contains(date.DayOfWeek))
            return true;

        if (date.Month == 7)
            return true;

        if (PublicHolidays.Contains(date.Date))
            return true;

        return PublicHolidays.Contains(date.AddDays(-1).Date);
    }
}
#endregion


#region source you
//public int GetTax(Vehicle vehicle, DateTime[] dates)
//{
//    DateTime intervalStart = dates[0];
//    int totalFee = 0;
//    foreach (DateTime date in dates)
//    {
//        int nextFee = GetTollFee(date, vehicle);
//        int tempFee = GetTollFee(intervalStart, vehicle);

//        long diffInMillies = date.Millisecond - intervalStart.Millisecond;
//        long minutes = diffInMillies / 1000 / 60;

//        if (minutes <= 60)
//        {
//            if (totalFee > 0) totalFee -= tempFee;
//            if (nextFee >= tempFee) tempFee = nextFee;
//            totalFee += tempFee;
//        }
//        else
//        {
//            totalFee += nextFee;
//        }
//    }
//    if (totalFee > 60) totalFee = 60;
//    return totalFee;
//}

//private bool IsTollFreeVehicle(Vehicle vehicle)
//{
//    if (vehicle == null) return false;
//    String vehicleType = vehicle.GetVehicleType();
//    return vehicleType.Equals(TollFreeVehicles.Motorcycle.ToString()) ||
//           vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
//           vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
//           vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
//           vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
//           vehicleType.Equals(TollFreeVehicles.Military.ToString());
//}

//public int GetTollFee(DateTime date, Vehicle vehicle)
//{
//    if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

//    int hour = date.Hour;
//    int minute = date.Minute;

//    if (hour == 6 && minute >= 0 && minute <= 29) return 8;
//    else if (hour == 6 && minute >= 30 && minute <= 59) return 13;
//    else if (hour == 7 && minute >= 0 && minute <= 59) return 18;
//    else if (hour == 8 && minute >= 0 && minute <= 29) return 13;
//    else if (hour >= 8 && hour <= 14 && minute >= 30 && minute <= 59) return 8;
//    else if (hour == 15 && minute >= 0 && minute <= 29) return 13;
//    else if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
//    else if (hour == 17 && minute >= 0 && minute <= 59) return 13;
//    else if (hour == 18 && minute >= 0 && minute <= 29) return 8;
//    else return 0;
//}

//private Boolean IsTollFreeDate(DateTime date)
//{
//    int year = date.Year;
//    int month = date.Month;
//    int day = date.Day;

//    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

//    if (year == 2013)
//    {
//        if (month == 1 && day == 1 ||
//            month == 3 && (day == 28 || day == 29) ||
//            month == 4 && (day == 1 || day == 30) ||
//            month == 5 && (day == 1 || day == 8 || day == 9) ||
//            month == 6 && (day == 5 || day == 6 || day == 21) ||
//            month == 7 ||
//            month == 11 && day == 1 ||
//            month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
//        {
//            return true;
//        }
//    }
//    return false;
//}

//private enum TollFreeVehicles
//{
//    Motorcycle = 0,
//    Tractor = 1,
//    Emergency = 2,
//    Diplomat = 3,
//    Foreign = 4,
//    Military = 5
//}
#endregion

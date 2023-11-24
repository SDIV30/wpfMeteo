using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for Class1
/// </summary>
public class MeteoProcessing
{
    public MeteoProcessing()
    {
    }
    public List<Pogoda> AveragingBegin(int timeInterval, List<Pogoda> receivedList)
    {
        List<Pogoda> sortedList = receivedList.OrderBy(res => res._date).ToList();
        List<Task<Pogoda>> tasks = new List<Task<Pogoda>>();
        List<Pogoda> results = new List<Pogoda>();
        
        Stopwatch sw = new Stopwatch();

        sw.Start();

        for (int iStart = 0; iStart < sortedList.Count;)
        {
            DateTime dateEnd = sortedList[iStart]._date.AddSeconds(timeInterval);

            int iEnd = sortedList.Count - 1;
            for (int c = iStart + 1; c < sortedList.Count; c++)
            {
                if (sortedList[c]._date > dateEnd && sortedList[c - 1]._date < dateEnd)
                {
                    iEnd = c - 1;
                    break;
                }
                if (sortedList[c]._date == dateEnd)
                {
                    iEnd = c;
                    break;
                }
            }
            if (iStart < iEnd)
            {
                tasks.Add(IntervalSumCalcC(sortedList, iStart, iEnd));
            }
            iStart = iEnd + 1;
        }

        Task.WaitAll(tasks.ToArray());
        var aa =tasks.Select(ts => ts.Result).OrderBy(res=>res._date);
        sw.Stop();
        System.Diagnostics.Debug.WriteLine("Execution took " + sw.ElapsedMilliseconds + "ms");
        return results=aa.ToList();
    }

    private Pogoda IntervalSumCalc(List<Pogoda> sortedList, int beginIndex, int endIndex) //расчёт по интервалам С ПО
    {
        int count = 0;
        double temperature = 0;
        double windDirection = 0;
        
            for (int i = beginIndex; i < endIndex; i++)
            {
                    count++;
                    temperature += sortedList[i].Temp;
                    windDirection += sortedList[i].WindDir;
            }
                double averageTemperature = temperature / count;
                double averageWindDirection = windDirection / count;

                //average temp and wind direction
                Pogoda resultPogoda = new Pogoda
                {
                    _date = sortedList[beginIndex]._date,
                    Temp = Math.Round(averageTemperature, 1),
                    WindDir = Math.Round(averageWindDirection, 1)
                };
                return resultPogoda;
    }

    private Task<Pogoda> IntervalSumCalcAsync(List<Pogoda> sortedList, int beginIndex, int endIndex) {
        var res =Task.Run(() =>  IntervalSumCalc(sortedList, beginIndex , endIndex));
        return res;
    }
    private async Task<Pogoda> IntervalSumCalcC(List<Pogoda> sortedList, int beginIndex, int endIndex) {
        var res =await IntervalSumCalcAsync(sortedList, beginIndex, endIndex);
        return res;
    }

}
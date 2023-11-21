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
    
    public List<Pogoda> AveragingBegin(int timeInterval, List<Pogoda> sortedResult)
    {
        List<Task<Pogoda>> tasks = new List<Task<Pogoda>>();
        List<Pogoda> results = new List<Pogoda>();
        var sortBegin = sortedResult[0].Dat;
        DateTime sortEnd = sortBegin.AddSeconds(timeInterval);  

        Stopwatch sw = new Stopwatch();

        sw.Start();
        
        while (sortBegin < sortedResult[sortedResult.Count-1].Dat)
        {
            //Debug.Print("begun");
            tasks.Add(IntervalSumCalcC(sortedResult, sortBegin, sortEnd));
            sortBegin = sortEnd;
            sortEnd = sortEnd.AddSeconds(timeInterval);
        }

        Task.WaitAll(tasks.ToArray());
        sw.Stop();
        foreach (Task<Pogoda> ts in tasks)
        { 
            results.Add(ts.Result);
        }
        System.Diagnostics.Debug.WriteLine("Execution took "+sw.ElapsedMilliseconds+"ms");
        return results.OrderBy(res => res.Dat).Where(res => res.Dat >= sortedResult[0].Dat).ToList();
    }
    
    
    private Pogoda IntervalSumCalc(List<Pogoda> sortedResult, DateTime sortBegin, DateTime sortEnd) //расчёт по интервалам С ПО
    {
        int beginIndex = sortedResult.FindIndex(x => x.Dat == sortBegin);
        int endIndex = sortedResult.FindIndex(x => x.Dat == sortEnd);
        DateTime newSortBegin = sortBegin.AddSeconds(10); 
        while(beginIndex == -1) {
            beginIndex = sortedResult.FindIndex(x => x.Dat == newSortBegin);
            newSortBegin.AddSeconds(10);
        }
        int count = 0;
        double temperature = 0;
        double windDirection = 0;
        Pogoda pog = new Pogoda();
        try
        {
            for (int i = beginIndex; i < endIndex; i++)
            {
                if (sortedResult[i].WindDir != -1)
                {
                    count++;
                    temperature += sortedResult[i].Temp;
                    windDirection += sortedResult[i].WindDir;
                }
            }
            if (count != 0)
            {
                double averageTemperature = temperature / count;
                double averageWindDirection = windDirection / count;

                //average temp and wind direction
                Pogoda resultPogoda = new Pogoda
                {
                    Dat = sortBegin,
                    Temp = Math.Round(averageTemperature, 1),
                    WindDir = Math.Round(averageWindDirection, 1)
                };
                //Debug.Print("finished");
                return resultPogoda;
            }
        }
        catch (System.ArgumentOutOfRangeException){}
        return pog;
    }

    private Task<Pogoda> IntervalSumCalcAsync(List<Pogoda> sortedResult, DateTime sortBegin, DateTime sortEnd) {
        var res =Task.Run(() =>  IntervalSumCalc(sortedResult, sortBegin, sortEnd));
        return res;
    }
    private async Task<Pogoda> IntervalSumCalcC(List<Pogoda> sortedResult, DateTime sortBegin, DateTime sortEnd) {
        var res =await IntervalSumCalcAsync(sortedResult, sortBegin, sortEnd);
        return res;
    }

}
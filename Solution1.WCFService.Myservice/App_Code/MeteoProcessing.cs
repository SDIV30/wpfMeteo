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
public delegate void SendPogoda(Pogoda pog);
public class MeteoProcessing
{
    
    private delegate Task<Pogoda> SendToFunc(double temperature, double windDirection, int count, DateTime sortBegin, int i);

    public MeteoProcessing()
    {
    }


    public SendPogoda sendResult;
    private SendToFunc sendToFunc;

    public void AveragingBegin(int timeInterval, List<Pogoda> sortedResult)
    {
        List<Task<bool>> tasks = new List<Task<bool>>();
        var sortBegin = sortedResult[0].Dat;
        DateTime sortEnd = sortBegin.AddSeconds(timeInterval);  

        Stopwatch sw = new Stopwatch();

        sw.Start();
       
        sendToFunc += PogodaCalcAsync;
        
        while (sortBegin < sortedResult[sortedResult.Count-1].Dat)
        {
            tasks.Add(IntervalSumCalcAsync(sortedResult, sortBegin, sortEnd));
            sortBegin = sortEnd;
            sortEnd = sortEnd.AddSeconds(timeInterval);
        }

        Task.WaitAll(tasks.ToArray());
        sw.Stop();

        System.Diagnostics.Debug.WriteLine("Execution took "+sw.ElapsedMilliseconds+"ms");
    }
    
    private Pogoda PogodaCalc(double temperature,double windDirection,int count,DateTime sortBegin) {
        double averageTemperature = temperature / count;
        double averageWindDirection = windDirection / count;

        //average temp and wind direction
        Pogoda resultPogoda =new Pogoda
        {
            Dat = sortBegin,
            Temp = Math.Round(averageTemperature, 1),
            WindDir = Math.Round(averageWindDirection, 1)
        };
        sendResult.Invoke(resultPogoda);
        System.Diagnostics.Debug.WriteLine(" task done ");
        return resultPogoda;
    }

    private Task<Pogoda> PogodaCalcTask(double temperature, double windDirection, int count, DateTime sortBegin)
    {
        var result = Task.Run(() => PogodaCalc(temperature, windDirection, count, sortBegin));
        return result;
    }

    private async Task<Pogoda> PogodaCalcAsync(double temperature, double windDirection, int count, DateTime sortBegin, int id)
    {
        System.Diagnostics.Debug.WriteLine("task started " + id);
        var res = await PogodaCalcTask(temperature, windDirection, count, sortBegin);
        System.Diagnostics.Debug.WriteLine("starting next " + id);
        return res;
    }
    
    private bool IntervalSumCalc(List<Pogoda> sortedResult, DateTime sortBegin, DateTime sortEnd) //расчёт по интервалам С ПО
    {
        int beginIndex = sortedResult.FindIndex(x => x.Dat == sortBegin);
        int endIndex = sortedResult.FindIndex(x => x.Dat == sortEnd);
        int count = 0;
        double temperature = 0;
        double windDirection = 0;

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
                sendToFunc.Invoke(temperature, windDirection, count, sortBegin, beginIndex);
        }
        catch (System.ArgumentOutOfRangeException){}
        return true;
    }

    private Task<bool> IntervalSumCalcTask(List<Pogoda> sortedResult, DateTime sortBegin, DateTime sortEnd) {
        var res =Task.Run(() =>  IntervalSumCalc(sortedResult, sortBegin, sortEnd));
        return res;
    }
    private async Task<bool> IntervalSumCalcAsync(List<Pogoda> sortedResult, DateTime sortBegin, DateTime sortEnd) {
        var res =await IntervalSumCalcTask(sortedResult, sortBegin, sortEnd);
        return res;
    }

}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for Class1
/// </summary>
public delegate void SendPogoda(Pogoda pog);
public class MeteoProcessing
{
    public MeteoProcessing()
    {
    }
    public SendPogoda sendResult;
    
    public void AveragingBegin(int timeInterval, List<Pogoda> sortedResult) 
    {
        List<Task<Pogoda>> tasks = new List<Task<Pogoda>>();
        List<Pogoda> averagedData = new List<Pogoda>();
        var sortEnd = sortedResult[0].Dat.AddSeconds(timeInterval);
        var sortBegin = sortedResult[0].Dat;
        var count = 0;
        double temperature = 0;
        double windDirection = 0;

        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < sortedResult.Count; i++)
        {
            if (sortedResult[i].Dat < sortEnd)
            {
                count++;
                temperature += sortedResult[i].Temp;
                windDirection += sortedResult[i].WindDir;
            }
            
            else
            {
                //System.Diagnostics.Debug.WriteLine("task launched "+ i);
                //var pog = PogodaCalcTask(temperature,windDirection,count,sortBegin);
                //averagedData.Add(pog.Result);
                System.Diagnostics.Debug.WriteLine("task launched "+ i);
                tasks.Add(PogodaCalcAsync(temperature, windDirection, count, sortBegin,i));//
                //Task.Run(async ()=>await PogodaCalcAsync(temperature, windDirection, count, sortBegin, i));
                System.Diagnostics.Debug.WriteLine("step over");
                temperature =0;
                windDirection=0;
                count = 0;
                sortBegin = sortEnd;
                sortEnd = sortEnd.AddSeconds(timeInterval);
            }
        }
        sw.Stop();
        Task.WaitAll(tasks.ToArray());

        System.Diagnostics.Debug.WriteLine("Execution took "+sw.ElapsedMilliseconds+"ms");
    }
    //придумать что то для асинка
    private async Task<Pogoda> PogodaCalcAsync(double temperature, double windDirection, int count, DateTime sortBegin,int id) {
        System.Diagnostics.Debug.WriteLine("task started " + id);
        var res = await PogodaCalcTask(temperature, windDirection, count, sortBegin);
        sendResult.Invoke(res);
        System.Diagnostics.Debug.WriteLine("task finished "+id);
        return res;
    }

    private Task<Pogoda> PogodaCalcTask(double temperature, double windDirection, int count, DateTime sortBegin) {

        var result = Task.Run(()=>PogodaCalc(temperature,windDirection,count,sortBegin));
        return result;
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
        return resultPogoda;
    }
}
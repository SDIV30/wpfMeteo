using System;
using System.Collections.Generic;
using System.Linq;
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
        List<Pogoda> averagedData = new List<Pogoda>();
        var sortEnd = sortedResult[0].Dat.AddSeconds(timeInterval);
        var sortBegin = sortedResult[0].Dat;
        var count = 0;
        double temperature = 0;
        double windDirection = 0;

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
                var pog = PogodaCalcTask(temperature,windDirection,count,sortBegin);
                // pog.Wait();
                averagedData.Add(pog.Result);//task POGODA
                temperature=0;
                windDirection=0;
                count = 0;
                sortBegin = sortEnd;
                sortEnd = sortEnd.AddSeconds(timeInterval);
            }
        }
        return averagedData;
    }
    //придумать что то для асинка
    private async Task<Pogoda> PogodaCalcAsync(double temperature, double windDirection, int count, DateTime sortBegin) {
        var res = await PogodaCalcTask(temperature, windDirection, count, sortBegin);
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
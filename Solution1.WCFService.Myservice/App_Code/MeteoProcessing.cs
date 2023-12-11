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
    delegate Pogoda MethodDelegate(List<Pogoda> sortedList, int beginIndex, int endIndex);

    Dictionary<string, MethodDelegate> functionName = new Dictionary<string, MethodDelegate>();

    public MeteoProcessing()
    {
        functionName["AverageArithmetic"] = AverageArithmetic;
        functionName["AverageWeighted"] = AverageWeighted;
        functionName["AverageMoving"] = AverageMoving;
    }

    public List<string> GetAlgorithms()
    {
        return functionName.Keys.ToList();
    }

    public List<Pogoda> AveragingBegin(long timeInterval, List<Pogoda> receivedList, string algorithm)
    {
        List<Pogoda> sortedList = receivedList.OrderBy(res => res._date).ToList();
        List<Task<Pogoda>> tasks = new List<Task<Pogoda>>();
        List<Pogoda> results = new List<Pogoda>();

        for (int iStart = 0; iStart < sortedList.Count;)
        {
            DateTime dateEnd = sortedList[iStart]._date.AddMilliseconds(timeInterval);
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
            if (iStart == iEnd)
            {
                results.Add(sortedList[iStart]);
            }
            if (iStart < iEnd)
            {
                tasks.Add(AverageRunAsynchronously(sortedList, iStart, iEnd,algorithm));
            }
            iStart = iEnd+ 1;
        }

        Task.WaitAll(tasks.ToArray());
        var aa = tasks.Select(ts => ts.Result);
        results.AddRange(aa);
        return results.OrderBy(res=>res._date).ToList();
    }

    private Pogoda AverageWeighted(List<Pogoda> sortedList, int beginIndex, int endIndex) {
        double weight = (100.0 / (double)(endIndex - beginIndex))/100.0;
        
        double temperature = 0;
        double windDirection = 0;

        if (endIndex-beginIndex==0)
            weight=1;
        for (int i = beginIndex; i < endIndex; i++)
        {
            temperature += sortedList[i].Temp*weight;
            windDirection += sortedList[i].WindDir*weight;
        }
        Pogoda output = new Pogoda {
            _date = sortedList[beginIndex]._date,
            Temp = Math.Round(temperature, 1),
            WindDir = Math.Round(windDirection, 1)
        };

        return output;
    }

    private Pogoda AverageArithmetic(List<Pogoda> sortedList, int beginIndex, int endIndex) 
    {
        long count = 0;
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
    //добавить новый алгоритм

    private Pogoda AverageMoving(List<Pogoda> sortedList, int beginIndex, int endIndex) {
        long count = endIndex - beginIndex;
        double temperature = 0;
        double windDirection = 0;
        if(count==0)
            count = 1;
        long divideBy = 0;

        for (int i = beginIndex; i < endIndex; i++)
        {
            temperature += sortedList[i].Temp*count;
            windDirection += sortedList[i].WindDir*count;
            divideBy += count;
            count--;
        }
        double averageTemperature = temperature / divideBy;
        double averageWindDirection = windDirection / divideBy;

        //average temp and wind direction
        Pogoda resultPogoda = new Pogoda
        {
            _date = sortedList[beginIndex]._date,
            Temp = Math.Round(averageTemperature, 1),
            WindDir = Math.Round(averageWindDirection, 1)
        };
        return resultPogoda;
    }

    private Task<Pogoda> CalculateAverageAsync(List<Pogoda> sortedList, int beginIndex, int endIndex, string demandedAlgorithm) {
        var res =Task.Run(() => this.functionName[demandedAlgorithm](sortedList, beginIndex, endIndex) );
        return res;
    }
    private async Task<Pogoda> AverageRunAsynchronously(List<Pogoda> sortedList, int beginIndex, int endIndex, string demandedAlgorithm) {
        var res = await CalculateAverageAsync(sortedList, beginIndex, endIndex, demandedAlgorithm);
        return res;
    }

}
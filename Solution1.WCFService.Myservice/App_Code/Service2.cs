using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Service2
{
    [WebInvoke(Method = "GET", UriTemplate = "/algoritms")]
    public List<string> GetAlgoritms()
    {
        return new List<string> { "10min", "30min", "1Hr" };
    }

    [WebInvoke(Method = "GET", UriTemplate = "/dan?s={dataS}&po={dataPo}&algoritm={algoritm}")]
    public List<Pogoda> GetData(string dataPo, string dataS, string algoritm = "alg1")
    {
        var receivedResult = new List<Pogoda>();
        //var resultSort = new List<Pogoda>();
        var averagedResult = new List<Pogoda>();

        try
        {
            var start = DateTime.Parse(dataS);
            var stop = DateTime.Parse(dataPo);
            var timeInterval = 600;
            switch (algoritm)
            {
                case "10min":
                    timeInterval = 600;
                    break;
                case "30min":
                    timeInterval = 1800;
                    break;
                case "1Hr":
                    timeInterval = 3600;
                    break;
                default:
                    break;
            }

            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\stepa\Downloads\meteo.csv");//path to file meteo.csv
            for (int c = 1; c < lines.Length; c++)
            {
                string[] spl = lines[c].Split(';');
                DateTime date = Convert.ToDateTime(spl[0]);
 
                if (start <= date && date <= stop/* && spl[2] != " - 1"*/)
                {
                    receivedResult.Add(
                        new Pogoda
                        {
                            Dat = date,
                            Temp = Convert.ToDouble(spl[1], CultureInfo.InvariantCulture),
                            WindDir = Convert.ToDouble(spl[2], CultureInfo.InvariantCulture)
                        }
                        );
                }
            }
            List<Pogoda> sortedResult = receivedResult.OrderBy(res => res.Dat).ToList();//отсорт список

            //SORTIROVKA SNIZU  

            var sortEnd = sortedResult[0].Dat.AddSeconds(timeInterval);
            var sortBegin = sortedResult[0].Dat;

            var counter = 0;
            double _temp = 0;
            double _wind = 0;

            var limitedPogoda = new List<Pogoda>();
            for (int i = 0; i < sortedResult.Count; i++)
            {
                if (sortedResult[i].Dat < sortEnd)
                {            
                    counter++;
                    _temp += sortedResult[i].Temp; 
                    _wind += sortedResult[i].WindDir;
                }
                else
                {
                    double newTemp = 0;
                    double newWind = 0;

                    newTemp = _temp / counter;
                    newWind = _wind / counter;
                    _temp = 0;
                    _wind = 0;

                    //записали среднюю температуру и ветер
                    averagedResult.Add(new Pogoda
                    {
                        Dat = sortBegin,
                        Temp = Math.Round(newTemp, 1),
                        WindDir = Math.Round(newWind, 1)
                    });
                    
                    counter = 0;
                    sortBegin = sortEnd;
                    sortEnd = sortEnd.AddSeconds(timeInterval);
                }
            }
        }
        catch (Exception ex)
        {
            throw new WebFaultException<string>(ex.Message, System.Net.HttpStatusCode.InternalServerError);
        }

        return averagedResult;


    }




}

[DataContract]
public class Pogoda
{
    [IgnoreDataMember]
    public DateTime Dat { get; set; }
    //[DataMember]
    //public int Index { get; set; }//!!!!!!!!!!!!!
    [DataMember]
    public string Date { get { return Dat.ToUniversalTime().ToString("o"); } set { } }

    [DataMember]
    public double Temp { get; set; }
    [DataMember]
    public double WindDir { get; set; }
}


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class Service2
{
    [WebInvoke(Method = "GET", UriTemplate = "/algoritms")]
    public List<string> GetAlgoritms()
    {
        return new List<string> { "10min", "30min", "1Hr" };
    }

    [WebInvoke(Method = "GET", UriTemplate = "/dan?s={dataS}&po={dataPo}&algoritm={algoritm}")]//request
    public List<Pogoda> GetData(string dataPo, string dataS, string algoritm = "alg1")
    {
        MeteoProcessing met = new MeteoProcessing();
        var receivedList = new List<Pogoda>();
        var averagedResult = new List<Pogoda>();
        
        try//time intervals
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
            string path =Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\meteo.csv");
            string[] lines = System.IO.File.ReadAllLines(path);
            for (int c = 1; c < lines.Length; c++)
            {
                string[] splited = lines[c].Split(';');
                DateTime date = Convert.ToDateTime(splited[0]);

                if (start <= date && date <= stop)
                {
                    receivedList.Add(
                        new Pogoda
                        {
                            _date = date,
                            Temp = Convert.ToDouble(splited[1], CultureInfo.InvariantCulture),
                            WindDir = Convert.ToDouble(splited[2], CultureInfo.InvariantCulture)
                        }
                        );
                }
            }
            
            
            averagedResult = met.AveragingBegin(timeInterval, receivedList);

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
    public DateTime _date;

    [DataMember]
    public string Date { get { return _date.ToUniversalTime().ToString("o"); } set { } }

    [DataMember]
    public double Temp { get; set; }
    [DataMember]
    public double WindDir { get; set; }
}


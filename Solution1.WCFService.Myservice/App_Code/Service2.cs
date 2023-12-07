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
    MeteoProcessing met = new MeteoProcessing();

    [WebInvoke(Method = "GET", UriTemplate = "/algorithms")]
    public List<string> GetAlgorithms()
    {
        return met.GetAlgorithms();
    }

    [WebInvoke(Method = "GET", UriTemplate = "/dan?dateBegin={dateBegin}&dateEnd={dateEnd}&algorithm={algorithm}&scale={scale}")]//request
    public List<Pogoda> GetData(string dateEnd, string dateBegin, string algorithm, string scale)
    {
        
        var receivedList = new List<Pogoda>();
        var averagedResult = new List<Pogoda>();
        
        try//time intervals
        {
            var start = DateTime.Parse(dateBegin);
            var stop = DateTime.Parse(dateEnd);

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

            if (met.GetAlgorithms().Contains(algorithm))
            {
                long timeInterval = Convert.ToInt64(scale);
                averagedResult = met.AveragingBegin(timeInterval, receivedList, algorithm);
            }
            else
            {
                averagedResult = receivedList.OrderBy(x => x._date).ToList();
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
    public DateTime _date;

    [DataMember]
    public string Date { get { return _date.ToUniversalTime().ToString("o"); } set { } }

    [DataMember]
    public double Temp { get; set; }
    [DataMember]
    public double WindDir { get; set; }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CircuiBrakerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TemperatureService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TemperatureService.svc or TemperatureService.svc.cs at the Solution Explorer and start debugging.
    public class TemperatureService : ITemperatureService
    {
        public int GetTemperature()
        {
            //System.Threading.Thread.Sleep(10 * 1000);
            return 25;
        }
    }
}

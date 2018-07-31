using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CircuitBrakerClient.Controllers
{
    public class HomeController : Controller
    {

        public static CircuitBreaker CircuitBreaker = new CircuitBreaker(5,TimeSpan.FromSeconds(30));

        public ActionResult Index()
        {
            string temperature = "uknown";
            try
            {
                using (var service = new TemperatureService.TemperatureServiceClient())
                {
                     temperature = CircuitBreaker.Invoke(() => service.GetTemperature().ToString());
                }
            }
            catch (TimeoutException)
            {

            }

            ViewBag.Temperature = temperature;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.State = CircuitBreaker.State.ToString();
            ViewBag.OpenSince = CircuitBreaker.State == State.Open ? CircuitBreaker.OpenSince.ToString() : "";
            ViewBag.TimeOutCount = CircuitBreaker.State == State.Closed ? CircuitBreaker.CurrentTimeoutCount.ToString() : "";


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }        
    }

    public enum State
    {
        Closed = 0,
        Open = 1,
        HalfOpen = 2
    }

    public class CircuitBreaker
    {

        public Result Invoke<Result>(Func<Result> action)
        {
            if (State == State.Open)
            {
                throw new TimeoutException("CircuitBreaker keeps your dependen services");
            }

            try
            {
                var res =  action();

                lock (this)
                {
                    if (State == State.HalfOpen)
                    {
                        //CloseCircuit
                        State = State.Closed;
                        CurrentTimeoutCount = 0;
                    }
                }

                return res;
            }
            catch (TimeoutException)
            {
                lock (this)
                {
                    CurrentTimeoutCount++;
                    if (State == State.HalfOpen
                        || (State == State.Closed && CurrentTimeoutCount >= TimeoutThresholdCount))
                    {
                        //OpenCircuit
                        State = State.Open;
                        OpenSince = DateTime.Now;
                    }
                }

                throw;
            }
        }


        public CircuitBreaker(int timeoutThresholdCount, TimeSpan retryAfter)
        {
            TimeoutThresholdCount = timeoutThresholdCount;
            RetryAfter = retryAfter;
        }


        private State _state;
        public State State
        {
            get
            {
                if (_state == State.Open && (DateTime.Now - OpenSince) >= RetryAfter)
                {
                    _state = State.HalfOpen;
                }
                return _state;
            }
            private set { _state = value; }
        }

        //State
        public int CurrentTimeoutCount { get; private set; }

        public DateTime OpenSince { get; private set; }


        //Settings
        public int TimeoutThresholdCount { get; private set; }

        public TimeSpan RetryAfter { get; private set; }




    }
}
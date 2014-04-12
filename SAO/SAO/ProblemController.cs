using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class ProblemController
    {
        public ProblemInstance Instance { get; private set; }

        public int CurrentMoment { get; private set; }

        public List<Car> ArchivedCarData { get; private set; } 

        public ProblemController(ProblemInstance instance)
        {
            this.Instance = instance;
            this.ArchivedCarData = new List<Car>();
        }

        //sekunda - najmniejsza niepodzielna jednostka czasu
        public void Start(int durationInSeconds = 1000)
        {
            for (CurrentMoment = 0; CurrentMoment < durationInSeconds; CurrentMoment++)
            {
               ProgressInstance();
            }
        }

        public void ProgressInstance()
        {
            GenerateCars();
            foreach (var crossroad in Instance.Crossroads)
            {
                SwitchLights(crossroad);
                MoveCars(crossroad);
            }
        }

        private void GenerateCars()
        {
            
        }

        private void SwitchLights(Crossroad crossroad)
        {
           
            var lights = crossroad.Lights;
            var moment = CurrentMoment;
            while (moment > lights.CycleDuration)
            {
                moment -= lights.CycleDuration;
            }
       
            var secondDuration = lights.WestEastDuration;
            TrafficLightsState computed = crossroad.LightsState;
            switch (lights.StartingState)
            {
                case TrafficLightsState.NorthSouth:
                    secondDuration = lights.WestEastDuration;
                    break;
                case TrafficLightsState.WestEast:
                    secondDuration = lights.NorthSouthDuration;
                    break;
            }
            if (moment < lights.TimeShift || moment >= lights.TimeShift + secondDuration)
            {
                if (computed != lights.StartingState)
                    crossroad.SwitchLightState();
            }
            else
            {
                if (computed == lights.StartingState)
                    crossroad.SwitchLightState();
            }
            
        }

        public void SetTrafficLightsConfiguration(Dictionary<int, TrafficLights> configuration)
        {
            foreach (var crossroad in Instance.Crossroads)
            {
                if (configuration.ContainsKey(crossroad.Id))
                {
                    crossroad.Lights = configuration[crossroad.Id];
                }
            }
        }

        public void MoveCars(Crossroad crossroad)
        {
            //if end route, add to ArchivedCarData
        }

        public double ComputeResult()
        {
            //zwraca wartosc funkcji minimalizujacej na podstawie ArchivedCarData z czasami
            return 1.0;
        }
    }
}

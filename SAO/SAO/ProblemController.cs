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
            }
            MoveCars();
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

        public void MoveCars()
        {
            foreach (var road in Instance.Roads)
            {
                for (int i = 0; i < road.IncreasingLaneCount; i++)
                {
                    List<CarDistance> toDelete = new List<CarDistance>();
                    foreach (var carDistance in road.IncreasingLanes[i])
                    {
                        if (carDistance.Distance > 0 && carDistance.Distance > ProblemInstance.CarSpeed)
                            //check if exists empty space to move a car
                            if (road.IncreasingLanes[i].Any(x => x.Distance + Car.Length > carDistance.Distance +
                                                                 ProblemInstance.CarSpeed &&
                                                                 x.Distance < carDistance.Distance))
                            {
                                //korek,wyznaczyc nowy distance
                            }
                            else
                            {
                                carDistance.Distance -= ProblemInstance.CarSpeed;
                            }
                        else
                        {
                            var leftoverSpeed = ProblemInstance.CarSpeed - carDistance.Distance;
                            //todo: zmniejsz distance do 0 lub najmniejszej mozliwej wartosci
                            carDistance.Distance = 0;
                            //result == true if car left road after movement
                            var result = MoveDependingOnLights(road.Second, carDistance, leftoverSpeed);

                            if (result)
                                toDelete.Add(carDistance);                           
                        }                
                    }

                    foreach (var element in toDelete)
                    {
                        road.IncreasingLanes[i].Remove(element);
                    }
                }

                for (int i = 0; i < road.DecreasingLaneCount; i++)
                {
                    List<CarDistance> toDelete = new List<CarDistance>();
                    foreach (var carDistance in road.DecreasingLanes[i])
                    {
                        if (carDistance.Distance > 0 && carDistance.Distance > ProblemInstance.CarSpeed)
                            //check if exists empty space to move a car
                            if (road.DecreasingLanes[i].Any(x => x.Distance + Car.Length > carDistance.Distance +
                                                                 ProblemInstance.CarSpeed &&
                                                                 x.Distance < carDistance.Distance))
                            {
                                //korek,wyznaczyc nowy distance
                            }
                            else
                            {
                                carDistance.Distance -= ProblemInstance.CarSpeed;
                            }
                        else
                        {
                            var leftoverSpeed = ProblemInstance.CarSpeed - carDistance.Distance;
                            //todo: zmniejsz distance do 0 lub najmniejszej mozliwej wartosci
                            carDistance.Distance = 0;
                            //result == true if car left road after movement
                            var result = MoveDependingOnLights(road.First, carDistance, leftoverSpeed);

                            if (result)
                                toDelete.Add(carDistance);
                        }
                    }

                    foreach (var element in toDelete)
                    {
                        road.DecreasingLanes[i].Remove(element);
                    }
                }
            }
            //if end route, add to ArchivedCarData
        }

        private bool MoveDependingOnLights(Crossroad crossroad, CarDistance carDistance, int leftoverSpeed)
        {
            //check lights and move car if green and no delay
            //determine here where car should ride
            //add carDistance to new road if car left crossroad
            //true if car left road
            return true;
        }

        public double ComputeResult()
        {
            //zwraca wartosc funkcji minimalizujacej na podstawie ArchivedCarData z czasami
            return 1.0;
        }
    }
}

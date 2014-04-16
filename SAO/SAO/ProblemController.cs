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

        public static int CurrentMoment { get; private set; }

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
            //place car on road just before lights?
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
            Orientation computed = crossroad.LightsState;
            switch (lights.StartingLightingState)
            {
                case Orientation.NorthSouth:
                    secondDuration = lights.WestEastDuration;
                    break;
                case Orientation.EastWest:
                    secondDuration = lights.NorthSouthDuration;
                    break;
            }
            if (moment < lights.TimeShift || moment >= lights.TimeShift + secondDuration)
            {
                if (computed != lights.StartingLightingState)
                    crossroad.SwitchLightState();
            }
            else
            {
                if (computed == lights.StartingLightingState)
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
            var finishedCars = new List<Car>();
            foreach (var road in Instance.Roads)
            {
                for (int i = 0; i < road.IncreasingLaneCount; i++)
                {
                    ProcessCars(road.IncreasingLanes[i], road.Second, finishedCars);
                }

                for (int i = 0; i < road.DecreasingLaneCount; i++)
                {
                   ProcessCars(road.DecreasingLanes[i], road.First, finishedCars);
                }
            }

            foreach (var car in finishedCars)
            {
                car.IsFinished = true;
                ArchivedCarData.Add(car);
            }
        }

        private void ProcessCars(LinkedList<CarDistance> lane,Crossroad crossroad, List<Car> finishedCars )
        {
            List<CarDistance> toDelete = new List<CarDistance>();
            foreach (var carDistance in lane)
            {
                if (carDistance.Distance > 0 && carDistance.Distance > ProblemInstance.CarSpeed)
                    //check if exists empty space to move a car
                    if (lane.Any(x => x.Distance + Car.Length > carDistance.Distance +
                                                         ProblemInstance.CarSpeed &&
                                                         x.Distance < carDistance.Distance))
                    {
                        carDistance.Stopped = true;
                        //korek,wyznaczyc nowy distance
                    }
                    else
                    {
                        if (carDistance.Stopped)
                            //w tej fazie jedynie ruszamy
                            carDistance.Stopped = false;
                        else
                            //samochod sie porusza i konczy na tej samej drodze
                            carDistance.Distance -= ProblemInstance.CarSpeed;
                    }
                else
                {
                    var leftoverSpeed = ProblemInstance.CarSpeed - carDistance.Distance;
                    // zmniejsz distance do 0 lub todo:najmniejszej mozliwej wartosci
                    carDistance.Distance = 0;
                    //result == true if car left road after movement
                    var result = MoveDependingOnLights(crossroad, carDistance, leftoverSpeed, finishedCars);

                    if (result)
                        toDelete.Add(carDistance);
                }
            }

            foreach (var element in toDelete)
            {
                lane.Remove(element);
            }
        }

        private bool MoveDependingOnLights(Crossroad crossroad, CarDistance carDistance, int leftoverSpeed, List<Car> finishedCars )
        {
            //koniec mapy
            if (crossroad == null)
            {
                finishedCars.Add(carDistance.Car);
                return true;
            }

            //czerwone
            if (crossroad.LightsState != carDistance.Road.Orientation)
            {
                return false;
            }

            if (carDistance.Delay > 0)
            {
                carDistance.Delay--;
                return false;
            }

            //koniec, samochod dojechal do konca skrzyzowania
            if (carDistance.Car.NextRoad == null)
            {
                finishedCars.Add(carDistance.Car);
                return true;
            }
                


            //check if there is space on next road 
            //check lights and move car if green and no delay
            //determine here where car should ride
            //add carDistance to new road if car left crossroad and set next delay!
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

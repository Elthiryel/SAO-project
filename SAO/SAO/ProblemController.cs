using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class ProblemController
    {
        private readonly Random random;
        public ProblemInstance Instance { get; private set; }

        public static int CurrentMoment { get; private set; }

        public List<Car> ArchivedCarData { get; private set; } 

        public ProblemController(ProblemInstance instance)
        {
            this.Instance = instance;
            this.ArchivedCarData = new List<Car>();
            random = new Random(ProblemInstance.Seed);
        }

        //sekunda - najmniejsza niepodzielna jednostka czasu
        public void Start(int durationInSeconds = 10000)
        {
            Instance.CleanInstance();
            Console.WriteLine("Starting Instance for "+durationInSeconds+" seconds");
            for (CurrentMoment = 0; CurrentMoment < durationInSeconds; CurrentMoment++)
            {
               ProgressInstance();
            }
        }

        private void ProgressInstance()
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
            //place car on road at the beginning?
            foreach (var route in Instance.Routes)
            {
                var number = random.NextDouble() * 100.0 * (100.0 / route.Priority);
                if (number < ProblemInstance.ChanceToGenerate)
                {
                    var road = route.Roads[0];
                    var nextRoad = route.Roads[1];
                    Crossroad crossroad = null;
                    if (road.First == null)
                        crossroad = road.Second;
                    if (road.Second == null)
                        crossroad = road.First;
                    if (nextRoad.First == null)
                        crossroad = nextRoad.Second;
                    if (nextRoad.Second == null)
                        crossroad = nextRoad.First;
                    if (crossroad == null && (road.First.Id == nextRoad.First.Id || road.First.Id == nextRoad.Second.Id))
                        crossroad = road.First;
                    else if(crossroad == null)
                        crossroad = road.Second;
                    var direction = DetermineDirection(crossroad, road);
                    var delayDirection = DetermineDirection(crossroad, nextRoad);
                    switch (direction)
                    {
                        case Direction.East:
                            road.AddCarToDecreasingLane(new Car(CurrentMoment, route), road.Length - Car.Length,
                                ComputeDelay(Direction.West, delayDirection));
                            break;
                        case Direction.North:
                            road.AddCarToIncreasingLane(new Car(CurrentMoment, route), road.Length - Car.Length,
                                ComputeDelay(Direction.South, delayDirection));
                            break;
                        case Direction.West:
                            road.AddCarToIncreasingLane(new Car(CurrentMoment, route), road.Length - Car.Length,
                                ComputeDelay(Direction.East, delayDirection));
                            break;
                        case Direction.South:
                            road.AddCarToDecreasingLane(new Car(CurrentMoment, route), road.Length - Car.Length,
                                ComputeDelay(Direction.North, delayDirection));
                            break;

                    }
                }
            }
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

        private void MoveCars()
        {
            var finishedCars = new List<Car>();
            foreach (var road in Instance.Roads)
            {
                for (int i = 0; i < road.IncreasingLaneCount; i++)
                {
                    ProcessCars(road.IncreasingLanes[i], road.Second, finishedCars, true);
                }

                for (int i = 0; i < road.DecreasingLaneCount; i++)
                {
                   ProcessCars(road.DecreasingLanes[i], road.First, finishedCars, false);
                }
            }

            foreach (var car in finishedCars)
            {
                car.IsFinished = true;
                ArchivedCarData.Add(car);
            }
        }

        private void ProcessCars(LinkedList<CarDistance> lane,Crossroad crossroad, List<Car> finishedCars, bool fromIncreasingLane )
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
                    var result = MoveDependingOnLights(crossroad, carDistance, leftoverSpeed, finishedCars, fromIncreasingLane);

                    if (result)
                        toDelete.Add(carDistance);
                }
            }

            foreach (var element in toDelete)
            {
                lane.Remove(element);
            }
        }

        private bool MoveDependingOnLights(Crossroad crossroad, CarDistance carDistance, int leftoverSpeed, List<Car> finishedCars, bool fromIncreasingLane )
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

            var road = carDistance.Car.NextRoad;

            //koniec, samochod dojechal do konca skrzyzowania
            if (road == null)
            {
                finishedCars.Add(carDistance.Car);
                return true;
            }

            Direction direction = DetermineDirection(crossroad,road);
            
            //setDelay
            int delay = 0;
            if (carDistance.Car.Route.Roads.Count >= carDistance.Car.RoadProgress + 2)
            {
                var delayRoad = carDistance.Car.Route.Roads[carDistance.Car.RoadProgress + 1];
                if (delayRoad != null)
                {
                    var delayCrossroad = road.First;
                    if (road.First != null && road.First.Id == crossroad.Id)
                        delayCrossroad = road.Second;

                    if (delayCrossroad != null)
                    {
                        var delayDirection = DetermineDirection(delayCrossroad, delayRoad);
                        delay = ComputeDelay(direction, delayDirection);
                    }
                }
            }
            
            switch (direction)
            {
                case Direction.North:
                    return road.AddCarToDecreasingLane(carDistance.Car, road.Length - leftoverSpeed - Car.Length, delay);
                case Direction.East:
                    return road.AddCarToIncreasingLane(carDistance.Car, road.Length - leftoverSpeed - Car.Length, delay);
                case Direction.South:
                    return road.AddCarToIncreasingLane(carDistance.Car, road.Length - leftoverSpeed - Car.Length, delay);
                case Direction.West:
                    return road.AddCarToDecreasingLane(carDistance.Car, road.Length - leftoverSpeed - Car.Length, delay);
            }

            return false;
        }

        private int ComputeDelay(Direction direction, Direction delayDirection)
        {
            //jazda prosto
            if (direction == delayDirection)
                return 0;
            if (direction == Direction.East && delayDirection == Direction.North)
                return ProblemInstance.LeftTurnDelay;
            if (direction == Direction.North && delayDirection == Direction.West)
                return ProblemInstance.LeftTurnDelay;
            if (direction == Direction.West && delayDirection == Direction.South)
                return ProblemInstance.LeftTurnDelay;
            if (direction == Direction.South && delayDirection == Direction.East)
                return ProblemInstance.LeftTurnDelay;
            if (direction == Direction.East && delayDirection == Direction.South)
                return ProblemInstance.RightTurnDelay;
            if (direction == Direction.North && delayDirection == Direction.East)
                return ProblemInstance.RightTurnDelay;
            if (direction == Direction.West && delayDirection == Direction.North)
                return ProblemInstance.RightTurnDelay;
            if (direction == Direction.South && delayDirection == Direction.West)
                return ProblemInstance.RightTurnDelay;
            throw new Exception("Could not compute delay");

        }

        private Direction DetermineDirection(Crossroad crossroad, Road road)
        {
            if (crossroad.North != null && crossroad.North.Id == road.Id)
                return Direction.North;
            if (crossroad.South != null && crossroad.South.Id == road.Id)
                return Direction.South;
            if (crossroad.East != null && crossroad.East.Id == road.Id)
                return Direction.East;
            if (crossroad.West != null && crossroad.West.Id == road.Id)
                return Direction.West;
            throw new Exception("Cannot determine direction");
        }

        public double ComputeResult()
        {
            int sumTime = 0;
            foreach (var car in ArchivedCarData)
                sumTime += car.TimeSinceDeparture;
            //zwraca wartosc funkcji minimalizujacej na podstawie ArchivedCarData z czasami
            return (double) sumTime /  ArchivedCarData.Count;
        }

        public Dictionary<Route, double> ComputeEachRoute(out Dictionary<Route, int> carCount)
        {
            var dict = new Dictionary<Route, double>();
            var countersdict = new Dictionary<Route, int>();
            foreach (var car in ArchivedCarData)
            {
                if (dict.ContainsKey(car.Route))
                    dict[car.Route] = dict[car.Route] + car.TimeSinceDeparture;
                else
                    dict[car.Route] = car.TimeSinceDeparture;
                if (countersdict.ContainsKey(car.Route))
                    countersdict[car.Route]++;
                else
                    countersdict[car.Route] = 1;
            }
            foreach (var route in countersdict.Keys)
            {
                dict[route] = dict[route]/countersdict[route];
            }
			carCount = countersdict;
            return dict;
        }

    }
}

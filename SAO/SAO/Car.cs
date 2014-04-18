using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class Car
    {
        private int _createTime;
        private int _finishedTime;

        public static int Length = 2;

		public Route Route { get; set; }

        public int TimeSinceDeparture
        {
            get { return IsFinished ? _finishedTime - _createTime : ProblemController.CurrentMoment - _createTime; } 
        }

        private bool _isFinished;
        public bool IsFinished
        {
            get { return _isFinished; }
            set
            {
                _isFinished = value;
                if (_isFinished)
                    _finishedTime = ProblemController.CurrentMoment;
            }
        }

        public int RoadProgress { get; set; }

        public Car(int createTime,Route route)
        {
            this._createTime = createTime;
            this.RoadProgress = 0;
            this.Route = route;
        }

        public Road NextRoad 
        {
            get
            {
                if (RoadProgress < Route.Roads.Count)
                    return this.Route.Roads[RoadProgress];
                return null;
            }
        }
    }
}

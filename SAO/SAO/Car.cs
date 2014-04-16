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
            get { return IsFinished ? _finishedTime : ProblemController.CurrentMoment - _createTime; } 
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

        public Car(int createTime)
        {
            this._createTime = createTime;
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

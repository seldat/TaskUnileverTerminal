using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskUnilever
{
    public class RobotTask
    {
        long taskId = -1;
        string robotId = "Unknow";
        string stationId = "Unknow";//1,2
        string type = "Unknow";//buffer,return,free
        string status = "Pending";
        string description = "Unknow";

        public RobotTask(long taskId)
        {
            setId(taskId);
        }
        public RobotTask()
        {
            setId(taskId);
        }

        public void setId(long taskId)
        {
            this.taskId = taskId;
        }
        public long getId()
        {
            return taskId;
        }

        public void setDescription(string descr)
        {
            this.description = descr;
        }
        public string getDescription()
        {
            return description;
        }

        public void setRobotId(string rbi)
        {
            this.robotId = rbi;
        }
        public string getRobotId()
        {
            return robotId;
        }

        public void setType(string sty)
        {
            this.type = sty;
        }
        public string getType()
        {
            return type;
        }

        public void setStation(string sta)
        {
            this.stationId = sta;
        }
        public string getStation()
        {
            return stationId;
        }

        public void setStatus(string stat)
        {
            this.status = stat;
        }
        public string getStatus()
        {
            return status;
        }

    }
}

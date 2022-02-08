using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace OperatingSystemsProject
{
    public class Process
    {
        // atirbutes of "Process" typ object
        public int processId, burstTime, priorty, arrivalTime, pWaitingTime, pTurnAroundTime, status, compeletionTime, timeUsed, currentQueue;
        public float responseRatio;
        public static Random rand = new Random();
        public Color processColor = new Color();

        public Process(int id, int b_time, int a_time)
        {
            processId = id;
            burstTime = b_time;
            arrivalTime = a_time;
            status = 0;
            pTurnAroundTime = 0;
            pWaitingTime = 0;
            compeletionTime = 0;
            processColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        public Process(int id, int b_time, int a_time, int pri)
        {
            processId = id;
            burstTime = b_time;
            arrivalTime = a_time;
            priorty = pri;
            status = 0;
            pTurnAroundTime = 0;
            pWaitingTime = 0;
            compeletionTime = 0;
            responseRatio = 0;
            timeUsed = 0;
            currentQueue = 0;
            processColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
    }
}

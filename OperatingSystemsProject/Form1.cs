using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace OperatingSystemsProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radioButtonSRT.CheckedChanged += new EventHandler(RadioButtonCheckChange);
            radioButtonCFS.CheckedChanged += new EventHandler(RadioButtonCheckChange);
            radioButtonMLFQ.CheckedChanged += new EventHandler(RadioButtonCheckChange);
            radioButtonHRRNP.CheckedChanged += new EventHandler(RadioButtonCheckChange);
            radioButtonHRRNNP.CheckedChanged += new EventHandler(RadioButtonCheckChange);
            radioButtonRR.CheckedChanged += new EventHandler(RadioButtonCheckChange);

        }

        #region declerations and initials
        List<Process> processList = new List<Process>();
        List<Process> arrivedList = new List<Process>();
        List<Process> timeLine = new List<Process>();
        List<Process> waitingQueue = new List<Process>();

        List<String> queueNames = new List<String> { "RR", "HRRN", "SRT", "CFS"};

        List<int> mlfqQueue = new List<int>();
        List<int> mlfqQuantums = new List<int>();
        List<int> arrivalTimeList = new List<int>();
        List<int> burstTimeList = new List<int>();

        int processId = 1;
        int gPosition = 10;
        int tPosition = 10;
        int currentTime = 0;
        int completeCheck = 0;
        int mcompleteCheck = 0;
        int currentProcess = 0;
        int finishTime;
        int processBurst;
        int processPriority;
        int processArrival;
        int processQuantum;
        int minimumBurstTime;
        float totalWaitingTime = 0f;
        float totalTurnAroundTime = 0f;
        string[] row = new string[20];
        #endregion

        #region ComboBox1SelectedIndexChanged
        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            addQueueButton.Enabled = true;
            if (comboBox1.SelectedIndex == 3 || comboBox1.SelectedIndex == 0)
                numericUpDown1.Enabled = true;
            else
                numericUpDown1.Enabled = false;
            
        }
        #endregion

        #region AddQueueButtonClick
        private void AddQueueButtonClick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 | comboBox1.SelectedIndex == 3)
            {
                mlfqQueue.Add(comboBox1.SelectedIndex);
                mlfqQuantums.Add(Convert.ToInt32(numericUpDown1.Text));
            }
            else
            {
                mlfqQueue.Add(comboBox1.SelectedIndex);
                mlfqQuantums.Add(0);
            }

            label10.Text = String.Empty;
            for(int i = 0; i < mlfqQueue.Count; i++)
            {
                if(queueNames[mlfqQueue[i]] == "RR")
                    label10.Text += queueNames[mlfqQueue[i]] + " quantum : " + mlfqQuantums[i] + "\n";
                else
                    label10.Text += queueNames[mlfqQueue[i]] + "\n";
            }
                
            
        }
        #endregion

        #region RadioButtonCheckChange
        private void RadioButtonCheckChange(object sender, EventArgs e)
        {
            runButton.Enabled = true;
            addQueueButton.Enabled = false;
            numericUpDown5.Enabled = false;
            RadioButton rb1 = groupBox4.Controls.OfType<RadioButton>().FirstOrDefault(n => n.Checked);
            RadioButton radioButton = sender as RadioButton;
            if (radioButtonRR == rb1 || radioButtonCFS == rb1)
                numericUpDown5.Enabled = true;
            if (radioButtonMLFQ == rb1)
                comboBox1.Enabled = true;
            
        }
        #endregion

        #region MakeCleaning
        private void MakeCleaning()
        {
            gPosition = 10;
            tPosition = 10;
            currentTime = 0;
            completeCheck = 0;
            mcompleteCheck = 0;
            currentProcess = 0;
            minimumBurstTime = 0;
            finishTime = 0;
            processBurst = 0;
            totalTurnAroundTime = 0;
            totalWaitingTime = 0;
            textBox3.Text = "";
            textBox4.Text = "";
            timeLine.Clear();
            burstTimeList.Clear();
            arrivalTimeList.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            panel1.Controls.Clear();
        }
        #endregion

        #region InsertButtonClick
        //INSERT button is pressed
        private void InsertButtonClick(object sender, EventArgs e)
        {
            processBurst = Convert.ToInt32(numericUpDown3.Text);
            processArrival = Convert.ToInt32(numericUpDown2.Text);
            processPriority = Convert.ToInt32(numericUpDown4.Text);


            processList.Add(new Process(processId, processBurst, processArrival, processPriority));
            row = new[]
                        {
                            Convert.ToString(processList[processList.Count - 1].processId),
                            Convert.ToString(processList[processList.Count - 1].arrivalTime),
                            Convert.ToString(processList[processList.Count - 1].burstTime),
                            Convert.ToString(processList[processList.Count - 1].priorty),
                            "-",
                            "-"
                        };

            dataGridView1.Rows.Add(row);
            processId++;
        }
        #endregion

        #region RunButtonClick
        //RUN button is pressed 
        private void RunButtonClick(object sender, EventArgs e)
        {
            //runButton.Enabled = false;  
            if (processList.Count == 0)
            {
                MessageBox.Show("Please! Insert data first.", "Error");
                return;
            }

            if (radioButtonSRT.Checked)//RUN SRT
            {
                MakeCleaning();
                SRT(processList);
            }
            else if (radioButtonCFS.Checked)//RUN CFS
            {
                processQuantum = Convert.ToInt32(numericUpDown5.Text);
                if (processQuantum == 0)
                {
                    MessageBox.Show(null, "QUANTUM = 0 !\nEnter a QUANTUM value and press RUN.", "Invalid Input");
                    return;
                }
                else
                {
                    MakeCleaning();
                    CFS(processList, processQuantum);
                    foreach (Process process in processList)
                    {
                        process.timeUsed = 0;
                    }
                }
            }
            else if (radioButtonMLFQ.Checked)//RUN MLFQ
            {
                MakeCleaning();
                MLFQ(processList);
                foreach (Process process in processList)
                {
                    process.responseRatio = 0;
                    process.currentQueue = 0;
                }

            }
            else if (radioButtonHRRNP.Checked)//RUN HRRNP
            {
                MakeCleaning();
                HRRNP(processList);
                foreach (Process process in processList)
                {
                    process.responseRatio = 0;
                }
            }
            else if (radioButtonHRRNNP.Checked)//RUN HRRNNP
            {
                MakeCleaning();
                HRRNNP(processList);         
                foreach(Process process in processList)
                {
                    process.responseRatio = 0;
                }
            }
            else if (radioButtonRR.Checked)//RUN ROUND ROBIN
            {
                processQuantum = Convert.ToInt32(numericUpDown5.Text);
                if (processQuantum == 0)
                {
                    MessageBox.Show(null, "QUANTUM = 0 !\nEnter a QUANTUM value and press RUN.", "Invalid Input");
                    return;
                }
                else
                {
                    MakeCleaning();
                    RR(processList, processQuantum);
                }

            }
            else
            {
                MessageBox.Show(null, "Something Happened","Something Happened");
                return;
            }

            for (int i = 0; i < processList.Count; i++)
            {
                totalWaitingTime += processList[i].pWaitingTime;
                totalTurnAroundTime += processList[i].pTurnAroundTime;
            }

            dataGridView1.Rows.Clear();
            dataGridView1.Visible = true;
            processList = processList.OrderBy(i => i.processId).ToList();
            for (int i = 0; i < processList.Count; i++)
            {
                row = new[]
                        {
                            Convert.ToString(processList[i].processId),
                            Convert.ToString(processList[i].arrivalTime),
                            Convert.ToString(processList[i].burstTime),
                            Convert.ToString(processList[i].priorty),
                            Convert.ToString(processList[i].pTurnAroundTime),
                            Convert.ToString(processList[i].pWaitingTime)
                        };

                dataGridView1.Rows.Add(row);
            }

            if ((totalTurnAroundTime / processList.Count) == 0)
                textBox3.Text = "0.00";
            else
                textBox3.Text = (totalTurnAroundTime / processList.Count).ToString("#.##");

            if ((totalWaitingTime / processList.Count) == 0)
                textBox4.Text = "0.00";
            else
                textBox4.Text = (totalWaitingTime / processList.Count).ToString("#.##");
            foreach(Process process in processList)
            {
                process.pTurnAroundTime = 0;
                process.pWaitingTime = 0;
            }
        }
        #endregion

        #region ClearButtonClick
        //CLEAR ALL PROCESSES
        private void ClearButtonClick(object sender, EventArgs e)
        {
            runButton.Enabled = true;
            processList.Clear();
            MakeCleaning();
            processId = 1;
        }
        #endregion

        #region AddRandomButtonClick
        //ADD RANDOM button is pressed
        private void AddRandomButtonClick(object sender, EventArgs e)
        {
            Random random = new Random();
            processBurst = random.Next(1,10);
            processArrival = random.Next(10);
            processPriority = random.Next(10);

            processList.Add(new Process(processId, processBurst, processArrival, processPriority));
            row = new[]
                        {
                            Convert.ToString(processList[processList.Count - 1].processId),
                            Convert.ToString(processList[processList.Count - 1].arrivalTime),
                            Convert.ToString(processList[processList.Count - 1].burstTime),
                            Convert.ToString(processList[processList.Count - 1].priorty),
                            "-",
                            "-"
                        };
            dataGridView1.Rows.Add(row);
            processId++;
        }
        #endregion

        #region DrawGanttPart
        private void DrawGanttPart(int processIndex)
        {
            TextBox textBox1 = new TextBox();
            panel1.Controls.Add(textBox1);
            textBox1.Size = new System.Drawing.Size(25, 25);
            textBox1.Text = "P" + processList[processIndex].processId;
            textBox1.Location = new System.Drawing.Point(gPosition, 110);
            textBox1.BackColor = processList[processIndex].processColor;
            textBox1.ReadOnly = true;
            gPosition += textBox1.Width;
        }
        #endregion

        #region mlfqDrawGanttPart
        private void mlfqDrawGanttPart(int processIndex,int levelofqueue)
        {
            TextBox textBox1 = new TextBox();
            panel1.Controls.Add(textBox1);
            textBox1.Size = new System.Drawing.Size(25, 25);
            textBox1.Text = "P" + processList[processIndex].processId;
            textBox1.Location = new System.Drawing.Point(gPosition, 110-(25*levelofqueue));
            textBox1.BackColor = processList[processIndex].processColor;
            textBox1.ReadOnly = true;
            gPosition += textBox1.Width;
        }
        #endregion

        #region PrintTimeLine
        private void PrintTimeline(int endTime)
        {
            int num = 0;
            tPosition -= 25;
            for (int i = 0; i<=endTime; i++)
            {
                
                //ending times of each process except last one
                TextBox textBox5 = new TextBox();
                panel1.Controls.Add(textBox5);
                textBox5.ReadOnly = true;
                textBox5.Size = new System.Drawing.Size(25, 25);
                textBox5.Location = new System.Drawing.Point((tPosition + textBox5.Width), 140);
                textBox5.Text = (num.ToString());
                textBox5.BorderStyle = BorderStyle.None;                
                tPosition += 25;
                num++;


            }
            
        }
        #endregion

        #region EmptyTimeDetect
        private int EmptyTimeDetect(int helper)
        {
            if (helper == 0)
            {
                TextBox textBox1 = new TextBox();
                panel1.Controls.Add(textBox1);
                textBox1.Size = new System.Drawing.Size(25, 25);
                textBox1.Text = " -";
                textBox1.Location = new System.Drawing.Point(gPosition, 110);
                textBox1.ReadOnly = true;
                gPosition += textBox1.Width;
                currentTime++;
                return 1;
            }
            return 0;
        }
        #endregion

        #region FindResponseRatio
        private void FindResponseRatio(int currentProcess)
        {
            Process p = arrivedList[currentProcess];
            if (burstTimeList[p.processId - 1] > 0)
            {
                p.pWaitingTime = currentTime - p.arrivalTime - (p.burstTime-burstTimeList[p.processId-1]);
                if (!(p.pWaitingTime < 0))
                    p.responseRatio = 1.0f*(p.pWaitingTime + burstTimeList[p.processId - 1]) / burstTimeList[p.processId - 1];
            }

        }
        #endregion

        #region MLFQ
        private void MLFQ(List<Process> processList)
        {
            
            int queueCount = mlfqQueue.Count;
            int processCount = processList.Count;
            for (int i = 0; i < processCount; i++)
            {
                burstTimeList.Add(processList[i].burstTime);
            }
            while (mcompleteCheck != processCount)
            {
                
                Process p;
                for (int i = 0; i < processCount; i++)
                {
                    
                    if ((processList[i].arrivalTime <= currentTime) && burstTimeList[i] > 0)
                    {
                        p = processList[i];
                        arrivedList.Add(p);
                    }
                }

                arrivedList = arrivedList.OrderBy(si => si.currentQueue).ToList();

                if (EmptyTimeDetect(arrivedList.Count) == 1)
                    continue;
                else
                {

                    Process currentProcess;
                        
                    for(int j = 0; j < queueCount; j++)
                    {
                        if (queueNames[mlfqQueue[j]] == "RR")
                        {
                            currentProcess = arrivedList[0];
                            if(currentProcess.currentQueue == j || currentProcess.currentQueue >= queueCount)
                            {
                                mlfqRR(currentProcess, mlfqQuantums[j]);
                                break;
                            }
                            
                        }
                        else if (queueNames[mlfqQueue[j]] == "HRRN" )
                        {
                            
                            for(int a = 0; a < arrivedList.Count; a++)                            
                                FindResponseRatio(a);

                            arrivedList = arrivedList.OrderByDescending(si => si.responseRatio).ToList();
                            currentProcess = arrivedList[0];
                            if (currentProcess.currentQueue == j)
                            {
                                mlfqHRRN(currentProcess);
                                break;
                            }
                                
                        }
                        else if (queueNames[mlfqQueue[j]] == "SRT")
                        {

                            arrivedList = arrivedList.OrderBy(si => burstTimeList[si.processId-1]).ToList();
                            currentProcess = arrivedList[0];
                            if (currentProcess.currentQueue == j)
                            {
                                mlfqSRT(currentProcess);
                                break;
                            }
                        }
                        }                            
                        
                    
                }

                arrivedList.Clear();
            }
            PrintTimeline(currentTime);
        }
        #endregion

        #region mlfqSRT

        private void mlfqSRT(Process p)
        {
            int currentId = p.processId - 1;
            for (int i = 0; i < p.burstTime; i++)
            {
                currentTime++;
                mlfqDrawGanttPart(currentId,p.currentQueue);
            }
            finishTime = currentTime;
            burstTimeList[currentId] = 0;
            processList[currentId].pTurnAroundTime = finishTime - processList[currentId].arrivalTime;
            processList[currentId].pWaitingTime = processList[currentId].pTurnAroundTime - processList[currentId].burstTime;
            mcompleteCheck++;
        }
        #endregion

        #region SRT
        private void SRT(List<Process> processList)
        {
            int processCount = processList.Count;
            minimumBurstTime = int.MaxValue;
            for (int i = 0; i < processCount; i++)
            {
                burstTimeList.Add(processList[i].burstTime); 
            }
            while (completeCheck != processCount)
            {
                int emptyCheck = 0;
                for (int i = 0; i < processCount; i++)
                {
                    if ((processList[i].arrivalTime <= currentTime) && (burstTimeList[i] < minimumBurstTime) && burstTimeList[i] > 0)
                    {
                        emptyCheck++;
                        minimumBurstTime = burstTimeList[i];
                        currentProcess = i;
                    }
                }

                if (EmptyTimeDetect(emptyCheck) ==1)
                    continue;

                timeLine.Add(processList[currentProcess]);
                burstTimeList[currentProcess]--;

                DrawGanttPart(currentProcess);

                if (burstTimeList[currentProcess] == 0)
                {
                    minimumBurstTime = int.MaxValue;                  
                    completeCheck++;
                    finishTime = currentTime + 1;

                    processList[currentProcess].pTurnAroundTime = finishTime - processList[currentProcess].arrivalTime;
                    processList[currentProcess].pWaitingTime = processList[currentProcess].pTurnAroundTime - processList[currentProcess].burstTime;



                }
                currentTime++;
            }

            PrintTimeline(currentTime);            
        }
        #endregion

        #region CFS
        private void CFS(List<Process> processList, int quantum)
        {
            int processCount = processList.Count;
            for (int i = 0; i < processCount; i++)
            {
                burstTimeList.Add(processList[i].burstTime);
            }
            while (completeCheck != processCount)
            {
                Process p;
                for (int i = 0; i < processCount; i++)
                {
                    if ((processList[i].arrivalTime <= currentTime) && burstTimeList[i] > 0)
                    {
                        p = processList[i];
                        arrivedList.Add(p);
                    }
                }

                if (EmptyTimeDetect(arrivedList.Count) == 1)
                    continue;
                else
                {
                    var sortedArray = arrivedList.OrderBy(si => si.timeUsed).ToList();
                    Process currentProcess = sortedArray[0];
                    int currentId = currentProcess.processId - 1;
                    if (burstTimeList[currentId] > quantum)
                    {
                        currentTime += quantum;
                        for(int i = 0; i<quantum; i++)
                            DrawGanttPart(currentId);
                        currentProcess.timeUsed += quantum;
                        burstTimeList[currentId] -= quantum;
                        waitingQueue.Add(currentProcess);
                    }
                    else
                    {
                        currentTime += burstTimeList[currentId];
                        finishTime = currentTime;
                        for (int i = 0; i < burstTimeList[currentId]; i++)
                            DrawGanttPart(currentId);
                        currentProcess.timeUsed += burstTimeList[currentId];
                        burstTimeList[currentId] = 0;
                        processList[currentId].pTurnAroundTime = finishTime - processList[currentId].arrivalTime;
                        processList[currentId].pWaitingTime = processList[currentId].pTurnAroundTime - processList[currentId].burstTime;

                        completeCheck++;
                    }
                }

                finishTime = 0;
                arrivedList.Clear();
            }
            PrintTimeline(currentTime);

        }
        #endregion

        #region mlfqHRRN

        private void mlfqHRRN(Process p)
        {
            int currentId = p.processId - 1;
            for(int i = 0; i < burstTimeList[currentId]; i++) {
                currentTime++;
                mlfqDrawGanttPart(currentId, p.currentQueue);
            }
            finishTime = currentTime;
            burstTimeList[currentId] = 0;
            processList[currentId].pTurnAroundTime = finishTime - processList[currentId].arrivalTime;
            processList[currentId].pWaitingTime = processList[currentId].pTurnAroundTime - processList[currentId].burstTime;
            mcompleteCheck++;
        }
        #endregion

        #region HRRNP
        private void HRRNP(List<Process> processList)
        {
            int processCount = processList.Count;


            for (int i = 0; i < processCount; i++)
            {
                burstTimeList.Add(processList[i].burstTime);
                processList[i].responseRatio = 0;
            }

            while (completeCheck != processCount)
            {
                Process p;
                for (int i = 0; i < processCount; i++)
                {
                    if ((processList[i].arrivalTime <= currentTime) && burstTimeList[i] > 0)
                    {
                        p = processList[i];
                        arrivedList.Add(p);
                    }
                }

                if (EmptyTimeDetect(arrivedList.Count) == 1)
                    continue;
                else
                {
                    for (int i = 0; i < arrivedList.Count; i++)
                    {
                        FindResponseRatio(i);
                    }

                }

                // sort process list according to their response ratio https://stackoverflow.com/questions/1301822/how-to-sort-a-list-of-objects-by-a-specific-field-in-c
                var sortedArray = processList.OrderByDescending(si => si.responseRatio).ToList();
                int tempCurrent = sortedArray[0].processId - 1;


                int tempBurst = burstTimeList[tempCurrent];
                
                burstTimeList[tempCurrent]--;
                timeLine.Add(processList[tempCurrent]);
                DrawGanttPart(tempCurrent);
                currentTime++;

                

                if (burstTimeList[tempCurrent] == 0)
                {

                    finishTime = currentTime;


                    processList[tempCurrent].pTurnAroundTime = finishTime - processList[tempCurrent].arrivalTime;
                    processList[tempCurrent].pWaitingTime = processList[tempCurrent].pTurnAroundTime - tempBurst;
                    processList[tempCurrent].responseRatio = 0;
                    completeCheck++;
                }
                finishTime = 0;
                arrivedList.Clear();
            }
            PrintTimeline(currentTime);

        }
        #endregion

        #region HRRNNP
        private void HRRNNP(List<Process> processList)
        {

            int processCount = processList.Count;
            

            for (int i = 0; i < processCount; i++)
            {
                burstTimeList.Add(processList[i].burstTime);
            }

            while (completeCheck != processCount)
            {
                Process p;
                for (int i = 0; i < processCount; i++)
                {
                    if ((processList[i].arrivalTime <= currentTime) && burstTimeList[i] > 0)
                    {
                        p = processList[i];
                        arrivedList.Add(p);
                    }
                }

                if (EmptyTimeDetect(arrivedList.Count) == 1)                
                    continue;                
                else
                {
                    for (int i = 0; i < arrivedList.Count; i++){

                        FindResponseRatio(i);
                    }

                }
                
                
               
                
                // sort process list according to their response ratio https://stackoverflow.com/questions/1301822/how-to-sort-a-list-of-objects-by-a-specific-field-in-c
                var sortedArray= processList.OrderByDescending(si => si.responseRatio).ToList();
                int tempCurrent = sortedArray[0].processId-1;
                

                int tempBurst = burstTimeList[tempCurrent];
                for(int i = 0; i < tempBurst; i++)
                {
                    burstTimeList[tempCurrent]--;
                    timeLine.Add(processList[tempCurrent]);
                    DrawGanttPart(tempCurrent);
                    currentTime++;

                }

                processList[tempCurrent].responseRatio = 0;
                if (burstTimeList[tempCurrent] == 0)
                {

                    finishTime = currentTime;


                    processList[tempCurrent].pTurnAroundTime = finishTime - processList[tempCurrent].arrivalTime;
                    processList[tempCurrent].pWaitingTime = processList[tempCurrent].pTurnAroundTime - tempBurst;
                    processList[tempCurrent].responseRatio = 0;
                    completeCheck++;
                }
                finishTime = 0;
                arrivedList.Clear();
            }
            PrintTimeline(currentTime);

        }
        #endregion

        #region mlfqRR
        private void mlfqRR(Process p, int quantum)
        {
            int currentId = p.processId - 1;
            if (burstTimeList[currentId] > quantum)
            {
                for (int i = 0; i < quantum; i++)
                {
                    currentTime++;
                    mlfqDrawGanttPart(currentId, p.currentQueue);

                }
                burstTimeList[currentId] -= quantum;
            }
            else
            {
                currentTime += burstTimeList[currentId];
                finishTime = currentTime;
                for (int i = 0; i < burstTimeList[currentId]; i++)
                    mlfqDrawGanttPart(currentId, p.currentQueue);
                burstTimeList[currentId] = 0;
                processList[currentId].pTurnAroundTime = finishTime - processList[currentId].arrivalTime;
                processList[currentId].pWaitingTime = processList[currentId].pTurnAroundTime - processList[currentId].burstTime;
                mcompleteCheck++;
                
            }

            if(processList[currentId].currentQueue != mlfqQueue.Count-1)
                processList[currentId].currentQueue++;
        }
        #endregion

        #region RR
        private void RR(List<Process> processList, int quantum)
        {             
            int processCount = processList.Count;
            List<Process> tempList = new List<Process>();
            for (int i = 0; i < processCount; i++)
            {
                burstTimeList.Add(processList[i].burstTime);
                tempList.Add(processList[i]);   
            }
            while (completeCheck != processCount)
            {
                Process p;
                for (int i = 0; i < processCount; i++)
                {
                    if ((tempList[i].arrivalTime <= currentTime) && burstTimeList[i] > 0)
                    {
                        p = tempList[i];
                        arrivedList.Add(p);
                        if (tempList[i].arrivalTime == currentTime)
                        {
                            if(waitingQueue.Count > 0)
                            {
                                int helper = 0;
                                for(int h = 0; h<waitingQueue.Count;h++)
                                {
                                    if (waitingQueue[h].processId != p.processId)
                                        helper++;
                                }
                                if(helper == waitingQueue.Count)
                                    waitingQueue.Add(p);

                            }
                            else
                            {
                                waitingQueue.Add(p);
                            }

                        }
                    }
                }

                if (EmptyTimeDetect(arrivedList.Count) == 1)
                    continue;
                else
                {
                    Process currentProcess = waitingQueue[0];
                    int currentId = currentProcess.processId - 1;
                    if(burstTimeList[currentId] > quantum)
                    {
                        for(int i = 0; i<quantum; i++)
                        {
                            currentTime++;
                            for (int j = 0; j< processCount; j++)
                            {
                                if ((tempList[j].arrivalTime <= currentTime) && burstTimeList[j] > 0)
                                {                                    
                                    if (tempList[j].arrivalTime == currentTime)
                                    {
                                        p = tempList[j];
                                        waitingQueue.Add(p);

                                    }
                                }

                            }
                            DrawGanttPart(currentId);

                        }
                        burstTimeList[currentId] -= quantum;
                        waitingQueue.RemoveAt(0);
                        waitingQueue.Add(currentProcess);
                    }
                    else
                    {
                        currentTime += burstTimeList[currentId];
                        finishTime = currentTime;
                        for (int i = 0; i < burstTimeList[currentId]; i++)
                            DrawGanttPart(currentId);
                        burstTimeList[currentId] = 0;
                        processList[currentId].pTurnAroundTime = finishTime - processList[currentId].arrivalTime;
                        processList[currentId].pWaitingTime = processList[currentId].pTurnAroundTime - processList[currentId].burstTime;

                        waitingQueue.RemoveAt(0);
                        completeCheck++;
                    }
                    finishTime = 0;
                    arrivedList.Clear();

                }
                
            }
            waitingQueue.Clear();
            PrintTimeline(currentTime);

        }


        #endregion

        #region clearQueues
        private void button1_Click(object sender, EventArgs e)
        {
            mlfqQueue.Clear();
            mlfqQuantums.Clear();
            label10.Text = String.Empty;
        }
        #endregion
    }
}

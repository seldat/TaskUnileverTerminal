using SeldatMRMS.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskUnilever
{
    public partial class Form1 : Form
    {

        Serial pserial;
        private int rowIndex = 0;
        private long taskCounter = 0;
        //Orders RobotRequest;
        public List<RobotTask> taskList;
        bool connected = false;
        Timer timer;
        List<waitTask> waitList;
        bool usage = false;
        class waitTask
        {
            public RobotTask rbT;
            public int live;
        }

        public Form1()
        {
            InitializeComponent();
            taskList = new List<RobotTask>();
            waitList = new List<waitTask>();
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("1");
            try
            {
                if (waitList.Count != 0)
                {
                    List<waitTask> needDel = new List<waitTask>();
                    foreach (waitTask item in waitList)
                    {
                        if (item.live != (-1))
                        {
                            if (item.live > 0)
                            {
                                item.live = item.live - 1;
                            }
                            if (item.live == 0)
                            {
                                if (PushTask(item.rbT))//popwaittask(item.rbT)
                                {
                                    taskCounter++;
                                    //waitList.Remove(item);
                                    needDel.Add(item);
                                    AddRowToDGV(new string[] {
                                    item.rbT.getId().ToString(),
                                    item.rbT.getRobotId(), item.rbT.getStation(),
                                    item.rbT.getType(), item.rbT.getStatus(),
                                    item.rbT.getDescription() });
                                }
                                else
                                {
                                    needDel.Add(item);
                                }
                            }
                        }
                        else
                        {
                            needDel.Add(item);
                        }
                    }
                    if (!usage)
                    {
                        foreach (waitTask item in needDel)
                        {
                            waitList.Remove(item);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error in timer_Tick");
            }
        }
        public bool PushTask(RobotTask rbT)
        {
            bool newItem = true;
            if (taskList.Count != 0)
            {
                foreach (RobotTask item in taskList)
                {
                    if ((item.getStation() == rbT.getStation()) &&
                        (item.getType() == rbT.getType()))
                    {
                        newItem = false;
                    }
                }
                if (newItem)
                {
                    rbT.setId(taskCounter);
                    taskList.Add(rbT);
                    return true;
                }
                return false;
            }
            rbT.setId(taskCounter);
            taskList.Add(rbT);
            return true;
        }

        public bool PopTask(string station, string type)
        {
            if (taskList.Count != 0)
            {
                foreach (RobotTask item in taskList)
                {
                    if ((item.getStation() == station) &&
                        (item.getType() == type) &&
                        (item.getStatus() != "working"))
                    {
                        taskList.Remove(item);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            //this.Hide();
            RemoveTask();
        }

        private void TaskUnilever_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
        }




        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!connected)
                {
                    connected = !connected;
                    pserial = new Serial("COM6");
                    pserial.Open();
                    pserial.serialMessenger += serialMessenger;
                    if (pserial._serialPort.IsOpen)
                    {
                        btn_Connect.Text = "Connected";
                        btn_Connect.BackColor = Color.LimeGreen;
                        btn_Connect.ForeColor = Color.White;
                    }
                }
                else
                {
                    connected = !connected;
                    pserial.Close();
                    btn_Connect.Text = "Disconnected";
                    btn_Connect.BackColor = SystemColors.Control;
                    btn_Connect.ForeColor = Color.Black;
                }
            }
            catch
            {
                MessageBox.Show("Error ComPort Connect !");
            }

        }

        public void serialMessenger(string st)
        {
            RobotTask rbT = new RobotTask();
            rbT.setStation(st.ToCharArray()[1].ToString() + st.ToCharArray()[2].ToString());
            rbT.setType(st.ToCharArray()[3].ToString() + st.ToCharArray()[4].ToString());
            try
            {
                if (Int32.Parse(rbT.getType()) != 3)
                {
                    PushWaitTask(rbT);
                }
                else
                {
                    usage = true;
                    foreach (waitTask item in waitList)
                    {
                        if (item.rbT.getStation() == rbT.getStation())
                        {
                            item.live = -1;
                        }
                    }
                    usage = false;
                }
            }
            catch
            {
                Console.WriteLine("Error in serialMessenger");
            }

        }


        public bool PushWaitTask(RobotTask rbT)
        {
            bool newItem = true;
            waitTask tempTask = new waitTask();
            tempTask.live = 10;
            tempTask.rbT = rbT;
            if (waitList.Count != 0)
            {
                foreach (waitTask item in waitList)
                {
                    if ((item.rbT.getStation() == rbT.getStation()) &&
                        (item.rbT.getType() == rbT.getType()))
                    {
                        newItem = false;
                    }
                }
                if (newItem)
                {
                    waitList.Add(tempTask);
                    return true;
                }
                return false;
            }
            waitList.Add(tempTask);
            return true;
        }

        public void AddRowToDGV(string[] data)
        {
            this.Invoke((MethodInvoker)delegate
            {
                dataGridView1.Rows.Add(data);
            });
        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    this.dataGridView1.Rows[e.RowIndex].Selected = true;
                    this.rowIndex = e.RowIndex;
                    this.dataGridView1.CurrentCell = this.dataGridView1.Rows[e.RowIndex].Cells[1];
                    this.contextMenuStrip1.Show(this.dataGridView1, e.Location);
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
            catch { }
        }

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            if (!this.dataGridView1.Rows[this.rowIndex].IsNewRow)
            {
                if (PopTask(this.dataGridView1.Rows[this.rowIndex].Cells["taskstation"].Value.ToString(), this.dataGridView1.Rows[this.rowIndex].Cells["tasktype"].Value.ToString()))
                {
                    this.dataGridView1.Rows.RemoveAt(this.rowIndex);
                }
            }
        }

        public void RemoveTask()
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (this.dataGridView1.RowCount > 0)
                {
                    if (PopTask(this.dataGridView1.Rows[0].Cells["taskstation"].Value.ToString(), this.dataGridView1.Rows[0].Cells["tasktype"].Value.ToString()))
                    {
                        this.dataGridView1.Rows.RemoveAt(0);
                    }
                }
            });
        }
        public void RequestTaskDocking(int area, string robotID, string camID)
        {
            //RobotRequest.RequestDockingOrderLine(area, robotID, camID);
        }

        public void RequestTaskPutAway(int area, string robotID, string camID)
        {
            //RobotRequest.RequestPutAwayOrderLine(area, robotID, camID);
        }

        private void TaskUnilever_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}


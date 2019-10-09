using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SetProxy;

namespace YTViewBot
{
    public partial class Form1 : Form
    {
        private static List<WeakReference> __ENCList;
        static Form1()
        {
            Form1.__ENCList = new List<WeakReference>();
        }

        public Form1()
        {
            InitializeComponent();
            Form1.__ENCAddToList(this);

        }
        private static void __ENCAddToList(object value)
        {
            lock (Form1.__ENCList)
            {
                if (Form1.__ENCList.Count == Form1.__ENCList.Capacity)
                {
                    int item = 0;
                    int count = checked(Form1.__ENCList.Count - 1);
                    for (int i = 0; i <= count; i = checked(i + 1))
                    {
                        if (Form1.__ENCList[i].IsAlive)
                        {
                            if (i != item)
                            {
                                Form1.__ENCList[item] = Form1.__ENCList[i];
                            }
                            item = checked(item + 1);
                        }
                    }
                    Form1.__ENCList.RemoveRange(item, checked(Form1.__ENCList.Count - item));
                    Form1.__ENCList.Capacity = Form1.__ENCList.Count;
                }
                Form1.__ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            _ReadDataGrid();
            webBrowser1.Refresh();
        }
        private void _ReadDataGrid()
        {
            if (dataGridView1.Rows.Count >= 1)
            {
                int cROW = dataGridView1.SelectedRows[0].Index;
                if (cROW < dataGridView1.RowCount)
                {
                    dataGridView1.Rows[++cROW].Selected = true;
                    lblproxy.Text = dataGridView1.Rows[cROW].Cells[0].Value.ToString();
                    WinInetInterop.SetConnectionProxy(lblproxy.Text);
                    webBrowser1.Navigate(new Uri(txturl.Text));
                }
            }
            else
            {
                MessageBox.Show("no proxy");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog opFile = new OpenFileDialog();
            opFile.Title = "Select TXT with proxy list";
            opFile.Filter = "TXT files (*.txt) | *.txt;";

            if (opFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtfile.Text = opFile.FileName;

                    //Read the data from text file
                    string[] textData = System.IO.File.ReadAllLines(@"" + txtfile.Text);
                    string[] headers = textData[0].Split(',');

                    //Create and populate DataTable
                    DataTable dataTable1 = new DataTable();
                    foreach (string header in headers)
                        dataTable1.Columns.Add(header, typeof(string), null);
                    for (int i = 0; i < textData.Length; i++)
                        dataTable1.Rows.Add(textData[i]);

                    //Set the DataSource of DataGridView to the DataTable
                    dataGridView1.DataSource = dataTable1;

                    lblproxy.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();

                    MessageBox.Show("NUMBER OF PROXY:" + dataGridView1.Rows.Count.ToString());

                }
                catch (Exception exp)
                {
                    MessageBox.Show("Unable to open file " + exp.Message);
                }
            }
            else
            {
                opFile.Dispose();
            }
        }

        private void Txttime_TextChanged(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txttime.Text))
            {
                if (Convert.ToInt32(txttime.Text) <= 29999)
                {
                    MessageBox.Show("Required 30000ms");
                }
            }
            else
            {
                timer1.Interval = Convert.ToInt32(txttime.Text);
            }

        }

        private void Txttime_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txturl.Text))
            {
                webBrowser1.Navigate(new Uri(txturl.Text));
            }
            else
            {
                MessageBox.Show("Please input your url");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txturl.Text))
            {
                if(dataGridView1.Rows.Count >= 1)
                {
                    WinInetInterop.SetConnectionProxy(lblproxy.Text);
                    webBrowser1.Navigate(new Uri(txturl.Text));
                    timer1.Start();
                    timer2.Start();
                }
                else
                {
                    MessageBox.Show("Please input your proxy");
                }
            }
            else
            {
                MessageBox.Show("Please input your url");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
        }
        int sec = 0;
        int min = 0;
        int hour = 0;
        private void Timer2_Tick(object sender, EventArgs e)
        {

            sec++;
            if(sec >= 60)
            {
                min++;
                sec = 0;
            }
            if(min >= 60)
            {
                hour++;
                min = 0;
            }
            lbls.Text = sec.ToString() + "s";
            lblm.Text = min.ToString() + "m";
            lblh.Text = hour.ToString() + "h";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessTokenGet
{
    public partial class Form1 : Form
    {
        public List<Process> processes = new List<Process>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            btnCopy.Enabled = false;
            bool seach = false;

            processes.Clear();
            listBox1.Items.Clear();

            Process[] processlist = Process.GetProcesses();
            for (int i = 0; i < processlist.Length; i++)
                processes.Add(processlist[i]);

            for (int i = 0; i < processes.Count; i++)
            {
                label1.Text = i.ToString();
                Process process = processes[i];
                listBox1.Items.Add($"PID: {process.Id} Name: {process.ProcessName} SessionId: {process.SessionId}");
                if (process.ProcessName == "PointBlank")
                {
                    MessageBox.Show("Point Blank found successfully.", "ProcessTokenGet", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OnlyPb();
                    seach = true;
                    break;
                }
            }
            if(!seach)
               MessageBox.Show("Point Blank not found!", "ProcessTokenGet", MessageBoxButtons.OK, MessageBoxIcon.Error);
            label1.Text = "-";
        }
        private void OnlyPb()
        {
            for (int j = listBox1.Items.Count - 1; j > -1; j--)
            {
                bool seach = listBox1.Items[j].ToString().Contains("PointBlank");
                if (!seach)
                {
                    listBox1.Items.RemoveAt(j);
                    listBox1.Refresh();
                }
            }
            for (int j = processes.Count - 1; j > -1; j--)
            {
                Process process = processes[j];
                if (process.ProcessName != "PointBlank") //PointBlank
                {
                    processes.RemoveAt(j);
                }
            }
            btnCopy.Enabled = true;
        }
        private static string GetCommandLine(int id)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + id))
            using (ManagementObjectCollection objects = searcher.Get())
            {
                return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < processes.Count; i++)
            {
                Process process = processes[i];
                string[] copy = GetCommandLine(process.Id).Split('/');
                if(copy != null)
                {
                    Clipboard.SetText(copy[2]);
                    MessageBox.Show("Token Key Successfully copied.", "ProcessTokenGet", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Guide to help you quickly find the constant string \r\n (Token Key) \r\n " +
                "By: Wesley Vale, Frank Lucas.", "ProcessKeyGet", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

#region |Name Space|
using System;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace ProcessForking
{
    /// <summary>
    /// Class Process Start
    /// </summary>
    internal class ProcessStart
    {
        #region |Private Variables Declartion|
        private readonly List<int> lstActiveProcessId = new List<int>();
        private readonly List<int> lstInActiveProcessId = new List<int>();
        private int counterActive;
        private int counterCreate;
        private int counterName;
        private bool isActiveProcessExists;
        private Process notePad;
        private int totalprocess = 1000;
        private int totalActiveProcess = 100;
        #endregion

        /// <summary>
        /// Main Method 
        /// </summary>
        /// <param name="args">string Command Line Argument</param>
        private static void Main(string[] args)
        {
            var processStart = new ProcessStart();
            processStart.fork();
        }

        /// <summary>
        /// Private Method Fork for Forking the Process.
        /// </summary>
        private void fork()
        {
            if (counterCreate < totalprocess)
            {
                if (counterActive < totalActiveProcess)
                {
                    var processStartInfo = new ProcessStartInfo("notepad.exe", "spawn");
                    notePad = Process.GetCurrentProcess();
                    notePad.StartInfo = processStartInfo;
                    notePad.Start();

                    //***************************
                    //Add Process Id to Active process id List
                    //***************************
                    lstActiveProcessId.Add(notePad.Id);
                    
                    //***************************
                    //Checking for printing the Active Process List
                    //***************************
                    if (counterActive == 2)
                    {
                        //************************
                        //Print the Active Process Id
                        //************************
                        foreach (int procId in lstActiveProcessId)
                        {
                            Console.WriteLine("Active Process Id =" + procId);
                        }
                    }
                    counterActive++;
                }
                else
                {
                    //*********************************
                    //Inactive Process Creations
                    //*********************************
                    var processStartInfo = new ProcessStartInfo("notepad.exe", "spawn");
                    processStartInfo.CreateNoWindow = true;
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.RedirectStandardInput = false;
                    processStartInfo.RedirectStandardOutput = false;
                    notePad = Process.GetCurrentProcess();
                    notePad.StartInfo = processStartInfo;
                    notePad.Start();                    
                    lstInActiveProcessId.Add(notePad.Id);
                    notePad.WaitForExit();
                }

                counterCreate++;
                notePad.EnableRaisingEvents = true;
                //***********************************
                // Add eevent handler on Process exit
                //***********************************
                notePad.Exited += OnProcessExisted;
            }


            if (counterCreate == 10)
            {
                Console.ReadLine();
                return;
            }
            //Process Forking
            fork();
        }

        /// <summary>
        /// On ProcessExit Method
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Eventargs e</param>
        private void OnProcessExisted(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var activeprocess = (Process)sender;
                if (lstActiveProcessId.Contains(activeprocess.Id))
                {
                    //Print a Exit message for this process
                    Console.WriteLine("Process " + activeprocess.Id + " has been completed its work and terminated");
                    //Remove this process from the active list
                    lstActiveProcessId.Remove(activeprocess.Id);

                    //Activate the last waiting process
                    if (lstInActiveProcessId.Count > 0)
                    {
                        Process inactiveProc = Process.GetProcessById(lstInActiveProcessId[0]);

                        //Check if the process is resumed by operating System 
                        if (inactiveProc.Responding)
                        {
                            //add Process Id in the active process list
                            lstActiveProcessId.Add(inactiveProc.Id);

                            //Print the new Active Process Ids
                            foreach (int procId in lstActiveProcessId)
                            {
                                Console.WriteLine("Now the Active Process Id =" + procId);
                            }
                        }
                        else
                        {
                            //Reduce active process number by 1
                            counterActive--;
                        }
                    }
                    else
                    {
                        //Reduce active process number by 1
                        counterActive--;
                    }
                }
                //Reduce total number of process by 1
                counterCreate--;

                fork();
            }
        }
    }
}
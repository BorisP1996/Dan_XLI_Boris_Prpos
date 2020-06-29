using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;

namespace Zadatak_1
{
    class ViewModel : ViewModelBase
    {
        MainWindow main;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public ViewModel(MainWindow mainOpen)
        {
            main = mainOpen;
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            
        }
        private int progres;
        public int Progres
        {
            get
            {
                return progres;
            }
            set
            {
                progres = value;
                OnPropertyChanged("Progres");
            }
        }
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }
        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
        private string runing;
        public string Runing
        {
            get
            {
                return runing;
            }
            set
            {
                runing = value;
                OnPropertyChanged("Runing");
            }
        }
        private int copyNumber;
        public int CopyNumber
        {
            get
            {
                return copyNumber;
            }
            set
            {
                copyNumber = value;
                OnPropertyChanged("CopyNumber");
            }
        }
        private ICommand close;
        public ICommand Close
        {
            get
            {
                if (close==null)
                {
                    close = new RelayCommand(param => CloseExecute(), param => CanCloseExecute());
                }
                return close;
            }
        }
        private void CloseExecute()
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }
        private bool CanCloseExecute()
        {
            if (worker.IsBusy)
            {
                return true;
            }
            if (!worker.IsBusy)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
        private ICommand startPrinting;
        public ICommand StartPrinting
        {
            get
            {
                if (startPrinting == null)
                {
                    startPrinting = new RelayCommand(param => StartPrintingExecute(), param => CanStartPrintingExecute());
                }
                return startPrinting;
            }
        }
        private void StartPrintingExecute()
        {
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
            else
            {
                Runing = "Printing already in progres.";
            }
        }
        private bool CanStartPrintingExecute()
        {
            if (String.IsNullOrEmpty(Text) || CopyNumber.ToString()==null || CopyNumber==0 || String.IsNullOrWhiteSpace(CopyNumber.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            int sum = 0;
            int percentage = 0;
            string path = @"../../";
            string path2 = path + 1 + "." + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + ".txt";
            for (int i = 0; i < CopyNumber; i++)
            {
                percentage = 100 / CopyNumber;
                sum = sum + percentage;
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    worker.ReportProgress(0);
                    return;
                }
                worker.ReportProgress(sum);
                StreamWriter sw = new StreamWriter(path2, true);
                sw.WriteLine(Text);
                int temp = i + 2;
                path2 = path + temp + "." + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + ".txt";
                Thread.Sleep(1000);
                sw.Close();
               
            }
            e.Result = sum;
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Message = "Printing is canceled";
                Runing = "Printing stopped";
              
            }
            else if (e.Error!=null)
            {
                Message="Worker exception:"+e.Error.ToString();
            }
            else if (Progres>90)
            {
                Message = "Printing finished";
               Runing = "";
            }
        }
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Message = "Reached:"+e.ProgressPercentage.ToString()+"%";
            Progres = e.ProgressPercentage;
            
        }


    }
}

using System;
using System.IO;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;

namespace Zadatak_1
{
    class ViewModel : ViewModelBase
    {
        MainWindow main;
        // creating background worker
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public ViewModel(MainWindow mainOpen)
        {
            //subscribing to methods
            main = mainOpen;
            worker.DoWork += DoWork;
            worker.ProgressChanged += ProgressChanged;
            worker.RunWorkerCompleted += RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            
        }
        //properties that will be used to bind elements from xaml
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
        //represents main text that will be printed
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
        //displaying message to user
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
        //displays aditional messages (stopped or finished)
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
        //takes number of copies from text box
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
        //comand for stopping print process
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
            //only if worker is busy=>stop it
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }
        //button is available only if process is alive
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
            //if worker is not already runing => start it
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
            //if it is already running=>display message
            else
            {
                Runing = "Printing already in progres.";
            }
        }
        //button will be available only under these circumstances
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
        //main method for worker=>prints copies to txt file
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            int sum = 0;
            int percentage = 0;
            string path = @"../../";
            //creating path => look into file to see needed form
            string path2 = path + 1 + "." + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + ".txt";
            for (int i = 0; i < CopyNumber; i++)
            {
                //calculating percentage that represents finished iterations
                percentage = 100 / CopyNumber;
                sum = sum + percentage;
                //in case that process is cancelled=> progres goes back to 0
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    worker.ReportProgress(0);
                    return;
                }
                //writing to file 
                worker.ReportProgress(sum);
                StreamWriter sw = new StreamWriter(path2, true);
                sw.WriteLine(Text);
                int temp = i + 2;
                path2 = path + temp + "." + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + ".txt";
                //pause between printing
                Thread.Sleep(1000);
                sw.Close();
               
            }
            //main result
            e.Result = sum;
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //if proces is stoped by user
            if (e.Cancelled)
            {
                Message = "Printing is canceled";
                Runing = "Printing stopped";
              
            }
            //if error is occured
            else if (e.Error!=null)
            {
                Message="Worker exception:"+e.Error.ToString();
            }
            //if progres is completed
            else if (Progres>90)
            {
                Message = "Printing finished";
               Runing = "";
            }
        }
        //displays progress continiously
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Message = "Reached:"+e.ProgressPercentage.ToString()+"%";
            Progres = e.ProgressPercentage;         
        }


    }
}

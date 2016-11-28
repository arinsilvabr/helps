﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Help_Generator
{
    partial class GenerateHelpsForm : Form
    {
        private HelpComponents _helpComponents;
        private bool _generateAndClose;

        private string _projectName;
        private string _outputChmFilename;
        private string _temporaryFilesPath;

        private Dictionary<Preprocessor.TopicPreprocessor,string> _outputTopicFilenames;
        private HashSet<string> _usedImageFilenames;
        private Dictionary<Preprocessor.TopicPreprocessor,List<string>> _contextSensitiveHelps;

        private BackgroundWorker _backgroundThread;

        public GenerateHelpsForm(HelpComponents helpComponents,bool generateAndClose)
        {
            InitializeComponent();

            _helpComponents = helpComponents;
            _generateAndClose = generateAndClose;

            _projectName = new DirectoryInfo(_helpComponents.projectPath).Name;
            _outputChmFilename = Path.Combine(_helpComponents.projectPath,_projectName + ".chm");
            _temporaryFilesPath = Path.Combine(_helpComponents.projectPath,Constants.TemporaryFileDirectoryName);

            _outputTopicFilenames = new Dictionary<Preprocessor.TopicPreprocessor,string>();
            _usedImageFilenames = new HashSet<string>();
            _contextSensitiveHelps = new Dictionary<Preprocessor.TopicPreprocessor,List<string>>();

            _backgroundThread = new BackgroundWorker();
        }

        private enum ThreadUpdateMessage { Cancel, Complete, SettingsComplete, TableOfContentsComplete, IndexComplete, TopicsProgress, TopicsComplete, ChmComplete };

        private void GenerateHelpsForm_Load(object sender,EventArgs e)
        {
            _backgroundThread.WorkerReportsProgress = true;
            _backgroundThread.WorkerSupportsCancellation = true;

            _backgroundThread.DoWork += new DoWorkEventHandler(
                delegate(object o,DoWorkEventArgs args)
                {
                    GenerateHelps();
                });

            _backgroundThread.ProgressChanged += new ProgressChangedEventHandler(
                delegate(object o,ProgressChangedEventArgs args)
                {
                    if( args.UserState is ThreadUpdateMessage )
                    {
                        ThreadUpdateMessage message = (ThreadUpdateMessage)args.UserState;

                        switch( message )
                        {
                            case ThreadUpdateMessage.Cancel:
                                _backgroundThread = null;
                                Close();
                                break;

                            case ThreadUpdateMessage.SettingsComplete:
                                pictureBoxCheckmarkSettings.Visible = true;
                                break;

                            case ThreadUpdateMessage.TableOfContentsComplete:
                                pictureBoxCheckmarkTableOfContents.Visible = true;
                                break;

                            case ThreadUpdateMessage.IndexComplete:
                                pictureBoxCheckmarkIndex.Visible = true;
                                progressBarTopics.Visible = true;
                                break;

                            case ThreadUpdateMessage.TopicsProgress:
                                progressBarTopics.Value = args.ProgressPercentage;
                                break;

                            case ThreadUpdateMessage.TopicsComplete:
                                progressBarTopics.Visible = false;
                                pictureBoxCheckmarkTopics.Visible = true;
                                break;

                            case ThreadUpdateMessage.ChmComplete:
                                pictureBoxCheckmarkChm.Visible = true;
                                break;

                            case ThreadUpdateMessage.Complete:
                                buttonOpenHelps.Enabled = true;
                                buttonCancelClose.Text = "Close";
                                _backgroundThread = null;

                                if( _generateAndClose )
                                    Close();

                                break;
                        }
                    }

                    else if( args.UserState is String )
                        textBoxLog.AppendText((string)args.UserState + "\r\n\r\n");

                    else if( args.UserState is DataReceivedEventArgs )
                        textBoxLog.AppendText(String.Format("HTML Help Compiler: {0}\r\n",(string)((DataReceivedEventArgs)args.UserState).Data));
                });

            _backgroundThread.RunWorkerAsync();
        }

        private void GenerateHelps()
        {
            int processingStep = 0;
            string[] stepStrings = new string[] { "Settings", "Table of Contents", "Index", "Topics", "Microsoft HTML Help" };

            try
            {
                Directory.CreateDirectory(_temporaryFilesPath);

                for( ; processingStep < stepStrings.Length && !_backgroundThread.CancellationPending; processingStep++ )
                {
                    _backgroundThread.ReportProgress(0,String.Format("Processing {0}...",stepStrings[processingStep]));

                    if( processingStep == 0 )
                    {
                        _helpComponents.settings.Compile(_helpComponents.preprocessor);
                        _backgroundThread.ReportProgress(0,ThreadUpdateMessage.SettingsComplete);
                    }
                    
                    else if( processingStep == 1 )
                    {
                        _helpComponents.tableOfContents.Compile(_helpComponents.preprocessor);
                        _backgroundThread.ReportProgress(0,ThreadUpdateMessage.TableOfContentsComplete);
                    }

                    else if( processingStep == 2 )
                    {
                        _helpComponents.index.Compile(_helpComponents.preprocessor);
                        _backgroundThread.ReportProgress(0,ThreadUpdateMessage.IndexComplete);
                    }

                    else if( processingStep == 3 )
                    {
                        // TODO: process topics
                        _backgroundThread.ReportProgress(0,ThreadUpdateMessage.TopicsComplete);
                    }

                    else if( processingStep == 4 )
                    {
                        GenerateChm();
                        _backgroundThread.ReportProgress(0,ThreadUpdateMessage.ChmComplete);
                    }

                    _backgroundThread.ReportProgress(0,"Successfully processed " + stepStrings[processingStep]);
                }

                if( _backgroundThread.CancellationPending )
                    _backgroundThread.ReportProgress(0,ThreadUpdateMessage.Cancel);

                else
                    _backgroundThread.ReportProgress(0,ThreadUpdateMessage.Complete);
            }

            catch( Exception exception )
            {
                _backgroundThread.ReportProgress(0,String.Format("Help Generation Error ({0}):\r\n\r\n{1}",stepStrings[processingStep],exception.Message));
            }

            _backgroundThread = null;
        }

        private void buttonOpenHelps_Click(object sender,EventArgs e)
        {
            Help.ShowHelp(null,_outputChmFilename);
        }

        private void GenerateHelpsForm_FormClosing(object sender,FormClosingEventArgs e)
        {
            if( _backgroundThread != null && _backgroundThread.IsBusy )
            {
                if( MessageBox.Show("Are you sure that you want to cancel the operation?",this.Text,MessageBoxButtons.YesNo) == DialogResult.Yes )
                    _backgroundThread.CancelAsync();

                e.Cancel = true;
            }

            else if( checkBoxDeleteTemporaryFiles.Checked )
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(_temporaryFilesPath);

                    foreach( FileInfo fi in di.GetFiles() )
                        fi.Delete();

                    di.Delete();
                }

                catch( Exception exception )
                {
                    MessageBox.Show("There was an error deleting the temporary files: " + exception.Message);
                }
            }
        }

        private void GenerateChm()
        {
            string settingsFilename = Path.Combine(_temporaryFilesPath,_projectName + ".hhp");
            string tableOfContentsFilename = Path.Combine(_temporaryFilesPath,_projectName + ".hhc");
            string indexFilename = Path.Combine(_temporaryFilesPath,_projectName + ".hhk");

            File.Delete(_outputChmFilename);

            _helpComponents.settings.SaveForChm(settingsFilename,_outputChmFilename,tableOfContentsFilename,indexFilename,
                _outputTopicFilenames,_usedImageFilenames,_contextSensitiveHelps);

            _helpComponents.tableOfContents.SaveForChm(tableOfContentsFilename,_outputTopicFilenames);

            _helpComponents.index.SaveForChm(indexFilename,_outputTopicFilenames);

            // compile the file
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.FileName = _helpComponents.htmlHelpCompilerExecutable;
            processStartInfo.Arguments = "\"" + settingsFilename + "\"";

            int numErrors = 0;

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate(object sender,DataReceivedEventArgs e)
                {
                    string hhcOutput = (string)e.Data;

                    if( !String.IsNullOrWhiteSpace(hhcOutput) )
                    {
                        if( hhcOutput.Contains("HHC") && hhcOutput.Contains("Error") )
                            numErrors++;

                        _backgroundThread.ReportProgress(0,e);
                    }
                }
            );

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();

            if( numErrors > 2 ) // there will be two errors because of false reports about problems compiling the jump buttons
            {
                if( File.Exists(_outputChmFilename) )
                    File.Delete(_outputChmFilename);

                throw new Exception("The HTML Help Compiler reported errors.");
            }
        }
    }
}
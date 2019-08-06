﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace GalaxyZooTouchTable.Lib
{
    public class Log
    {
        public static bool EnableDebugOutput { get; set; }
        public String FileName { get; set; }
        TextWriter log;
        List<String> entryBuffer1;
        List<String> entryBuffer2;
        List<String> activeEntryBuffer;
        List<String> inactiveEntryBuffer;
        int maximumBufferSize = 100;
        object gate;
        bool writingInactiveBufferToFile;
        Thread writerThread;
        bool isOpen;
        DateTime lastCommitted;
        TimeSpan commitInterval = TimeSpan.FromSeconds(10);
        DateTime logStartTime;

        static Log()
        {
            EnableDebugOutput = false; //true
        }

        public Log(String fileNameBase)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(Path.Combine(folderPath, "TouchTable_Logs")))
                Directory.CreateDirectory(Path.Combine(folderPath, "TouchTable_Logs"));

            String availableFileName = "";
            bool fileExists = true;
            int fileNameIndex = 0;
            while (fileExists)
            {
                String potentialFileName = fileNameBase + "_" + fileNameIndex + ".csv";
                string fullPath = Path.Combine(folderPath, "TouchTable_Logs", potentialFileName);

                if (!File.Exists(fullPath))
                {
                    availableFileName = fullPath;
                    fileExists = false;
                }
                fileNameIndex++;
            }
            FileName = availableFileName;
            entryBuffer1 = new List<string>();
            entryBuffer2 = new List<string>();
            activeEntryBuffer = entryBuffer1;
            inactiveEntryBuffer = entryBuffer2;
            gate = new object();
            lastCommitted = DateTime.Now;
            Open();
            logStartTime = DateTime.Now;

            AddHeaders();
            AddEntry(entry: "Log_Started");
            writerThread = new Thread(new ThreadStart(WriteLoop)) { IsBackground = true, Priority = ThreadPriority.BelowNormal };
            writerThread.Start();
        }

        public void Open()
        {
            log = new StreamWriter(FileName, true);
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
            log.Close();
        }

        public void FinalizeLog()
        {
            AddEntry(entry: "Log_Ended");
            while (writingInactiveBufferToFile)
            {
                // wait
            }
            WriteActiveBufferToFile();
            WriteLinesToFile();
            Close();
        }

        public void AddHeaders()
        {
            lock (gate)
            {
                string headers = "time,event,user,subjectId,state,context,peer,answer";
                activeEntryBuffer.Add(headers);
                if (EnableDebugOutput) System.Diagnostics.Debug.WriteLine(headers);
            }
            CheckActiveBuffer();
        }

        public void AddEntry(string entry, string user = null, string subjectId = null, ClassifierViewEnum state = ClassifierViewEnum.None, string context = null, string peer = null, string answer = null)
        {
            lock (gate)
            {
                String entryString = PrintLog(entry, user, subjectId, state, context, peer, answer, DateTime.Now.ToString("HH:mm:ss.ffff"));
                activeEntryBuffer.Add(entryString);
                if (EnableDebugOutput) System.Diagnostics.Debug.WriteLine(entryString);
            }
            CheckActiveBuffer();
        }

        void CheckActiveBuffer()
        {
            if (!writingInactiveBufferToFile && activeEntryBuffer.Count > maximumBufferSize)
            {
                WriteActiveBufferToFile();
            }
        }

        void WriteActiveBufferToFile()
        {
            SwapBuffers();
            WriteInactiveBufferToFile();
        }

        void WriteInactiveBufferToFile()
        {
            writingInactiveBufferToFile = true;
        }

        void SwapBuffers()
        {
            lock (gate)
            {
                List<String> mem = entryBuffer1;
                entryBuffer1 = entryBuffer2;
                entryBuffer2 = mem;
                activeEntryBuffer = entryBuffer1;
                inactiveEntryBuffer = entryBuffer2;
            }
        }

        void WriteLinesToFile()
        {
            if (writingInactiveBufferToFile && isOpen)
            {
                foreach (String entry in inactiveEntryBuffer)
                {
                    log.WriteLine(entry);
                }
                inactiveEntryBuffer.Clear();
                TimeSpan timeSinceLastCommit = DateTime.Now - lastCommitted;
                if (timeSinceLastCommit > commitInterval)
                {
                    Close();
                    Open();
                    lastCommitted = DateTime.Now;
                }
                writingInactiveBufferToFile = false;
            }
        }

        public void WriteLoop()
        {
            while (true)
            {
                WriteLinesToFile();
                Thread.Sleep(10000);
            }
        }

        public string PrintLog(string entry, string user, string subjectId, ClassifierViewEnum state, string context, string peer, string answer, string time)
        {
            string stateString = null;
            if (state == ClassifierViewEnum.SubjectView)
                stateString = "Classifier";
            else if (state == ClassifierViewEnum.SummaryView)
                stateString = "Summary";

            return $"{time},{entry},{user ?? ""},{subjectId ?? ""},{stateString ?? ""},{context ?? ""},{peer ?? ""},{answer ?? ""}";
        }
    }
}

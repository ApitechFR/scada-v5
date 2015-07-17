﻿/*
 * Copyright 2015 Mikhail Shiryaev
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : ScadaCommCommon
 * Summary  : TCP connection with a device
 * 
 * Author   : Mikhail Shiryaev
 * Created  : 2015
 * Modified : 2015
 */

using Scada.Comm.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Scada.Comm.Layers
{
    /// <summary>
    /// TCP connection with a device
    /// <para>TCP-соединение с КП</para>
    /// </summary>
    public class TcpConnection : Connection
    {
        /// <summary>
        /// Таймаут чтения одного байта, мс
        /// </summary>
        protected const int OneByteReadTimeout = 10;
        /// <summary>
        /// Максимальный размер считываемых строк по умолчанию
        /// </summary>
        protected const int DeaultfMaxLineSize = 1000;
        /// <summary>
        /// Периодичность попыток установки TCP-соединения, с
        /// </summary>
        protected const int ConnectPeriod = 5;
        /// <summary>
        /// Обозначение активности для строкового представления соединения
        /// </summary>
        protected static readonly string ActivityStr = 
            Localization.UseRussian ? "активность: " : "activity: ";

        /// <summary>
        /// Клиент TCP-соединения
        /// </summary>
        protected TcpClient tcpClient;
        /// <summary>
        /// Максимальный размер считываемых строк
        /// </summary>
        protected int maxLineSize;
        /// <summary>
        /// Дата и время установки соединения
        /// </summary>
        protected DateTime connectDT;


        /// <summary>
        /// Конструктор, ограничивающий создание объекта без параметров
        /// </summary>
        protected TcpConnection()
        {
        }
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public TcpConnection(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            maxLineSize = DeaultfMaxLineSize;
            connectDT = DateTime.MinValue;
            TcpClient = tcpClient; // в том числе NetStream
            ActivityDT = DateTime.Now;
            JustConnected = true;
            Broken = false;
            RelatedKP = null;
        }


        /// <summary>
        /// Получить признак, что соединение установлено
        /// </summary>
        public override bool Connected
        {
            get
            {
                return TcpClient.Connected;
            }
        }

        /// <summary>
        /// Получить клиента TCP-соединения
        /// </summary>
        public TcpClient TcpClient
        {
            get
            {
                return tcpClient;
            }
            protected set
            {
                tcpClient = value;
                NetStream = tcpClient.GetStream();
                TakeAddresses();
            }
        }

        /// <summary>
        /// Получить поток данных TCP-соединения
        /// </summary>
        public NetworkStream NetStream { get; protected set; }

        /// <summary>
        /// Получить или установить максимальный размер считываемых строк
        /// </summary>
        public int MaxLineSize
        {
            get
            {
                return maxLineSize;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Maximum line size must be positive.", "value");
                maxLineSize = value;
            }
        }

        /// <summary>
        /// Получить или установить дату и время последней активности
        /// </summary>
        public DateTime ActivityDT { get; set; }

        /// <summary>
        /// Получить или установить признак, что соединение только что установлено и обмена данными не было
        /// </summary>
        public bool JustConnected { get; set; }

        /// <summary>
        /// Получить или установить признак, что соединение обрвано и его необходимо закрыть
        /// </summary>
        public bool Broken { get; set; }

        /// <summary>
        /// Получить или установить КП, относящийся к данному соединению
        /// </summary>
        public KPLogic RelatedKP { get; set; }


        /// <summary>
        /// Обновить дату и время последней активности
        /// </summary>
        protected void UpdateActivityDT()
        {
            ActivityDT = DateTime.Now;
        }

        /// <summary>
        /// Определить локальный и удалённый адрес соединения
        /// </summary>
        protected void TakeAddresses()
        {
            try { LocalAddress = ((IPEndPoint)TcpClient.Client.LocalEndPoint).Address.ToString(); }
            catch { LocalAddress = ""; }

            try { RemoteAddress = ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString(); }
            catch { RemoteAddress = ""; }
        }


        /// <summary>
        /// Считать данные
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count, int timeout, 
            CommUtils.ProtocolLogFormats logFormat, out string logText)
        {
            try
            {
                int readCnt = 0;
                DateTime nowDT = DateTime.Now;
                DateTime startDT = nowDT;
                DateTime stopDT = startDT.AddMilliseconds(timeout);
                NetStream.ReadTimeout = timeout; // таймаут не выдерживается, если считаны все доступные данные

                do
                {
                    // считывание данных
                    try { readCnt += NetStream.Read(buffer, readCnt + offset, count - readCnt); }
                    catch (IOException) { }

                    // накопление данных во внутреннем буфере соединения
                    if (readCnt < count)
                        Thread.Sleep(DataAccumThreadDelay);

                    nowDT = DateTime.Now;
                } while (readCnt < count && startDT <= nowDT && nowDT <= stopDT);

                logText = BuildReadLogText(buffer, offset, count, readCnt, logFormat);

                if (readCnt > 0)
                    UpdateActivityDT();

                return readCnt;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(CommPhrases.ReadDataError + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Считать данные с условием остановки чтения
        /// </summary>
        public override int Read(byte[] buffer, int offset, int maxCount, int timeout, BinStopCondition stopCond,
            out bool stopReceived, CommUtils.ProtocolLogFormats logFormat, out string logText)
        {
            try
            {
                int readCnt = 0;
                DateTime nowDT = DateTime.Now;
                DateTime startDT = nowDT;
                DateTime stopDT = startDT.AddMilliseconds(timeout);
                NetStream.ReadTimeout = OneByteReadTimeout;

                stopReceived = false;
                int curOffset = offset;
                byte stopCode = stopCond.StopCode;

                while (readCnt <= maxCount && !stopReceived && startDT <= nowDT && nowDT <= stopDT)
                {
                    // считывание одного байта данных
                    bool readOk;
                    try { readOk = NetStream.Read(buffer, curOffset, 1) > 0; }
                    catch (IOException) { readOk = false; }

                    if (readOk)
                    {
                        stopReceived = buffer[curOffset] == stopCode;
                        curOffset++;
                        readCnt++;
                    }
                    else
                    {
                        // накопление данных во внутреннем буфере соединения
                        Thread.Sleep(DataAccumThreadDelay);
                    }

                    nowDT = DateTime.Now;
                }

                logText = BuildReadLogText(buffer, offset, readCnt, logFormat);

                if (readCnt > 0)
                    UpdateActivityDT();

                return readCnt;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(CommPhrases.ReadDataWithStopCondError + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Считать строки
        /// </summary>
        public override List<string> ReadLines(int timeout, TextStopCondition stopCond,
            out bool stopReceived, out string logText)
        {
            try
            {
                List<string> lines = new List<string>();
                string[] stopEndings = stopCond.StopEndings;
                int endingsLen = stopEndings == null ? 0 : stopEndings.Length;
                stopReceived = false;

                DateTime nowDT = DateTime.Now;
                DateTime startDT = nowDT;
                DateTime stopDT = startDT.AddMilliseconds(timeout);
                NetStream.ReadTimeout = OneByteReadTimeout;

                StringBuilder sbLine = new StringBuilder(maxLineSize);
                byte[] buffer = new byte[1];
                int newLineLen = NewLine.Length;

                while (!stopReceived && startDT <= nowDT && nowDT <= stopDT)
                {
                    // считывание одного байта данных
                    bool readOk;
                    try { readOk = NetStream.Read(buffer, 0, 1) > 0; }
                    catch (IOException) { readOk = false; }

                    if (readOk)
                    {
                        sbLine.Append(Encoding.Default.GetChars(buffer));
                    }
                    else
                    {
                        // накопление данных во внутреннем буфере соединения
                        Thread.Sleep(DataAccumThreadDelay);
                    }

                    int lineEndInd = sbLine.Length - newLineLen;
                    bool newLineFound = lineEndInd >= 0 && sbLine.ToString(lineEndInd, newLineLen) == NewLine;

                    if (newLineFound || sbLine.Length == maxLineSize)
                    {
                        string line = newLineFound ? sbLine.ToString(0, lineEndInd) : sbLine.ToString();
                        lines.Add(line);
                        sbLine.Clear();
                        for (int i = 0; i < endingsLen && !stopReceived; i++)
                            stopReceived = line.EndsWith(stopEndings[i], StringComparison.OrdinalIgnoreCase);
                    }

                    nowDT = DateTime.Now;
                }

                logText = BuildReadLinesLogText(lines);

                if (lines.Count > 0)
                    UpdateActivityDT();

                return lines;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(CommPhrases.ReadLinesError + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Записать данные
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count,
            CommUtils.ProtocolLogFormats logFormat, out string logText)
        {
            try
            {
                NetStream.Write(buffer, offset, count);
                logText = BuildWriteLogText(buffer, offset, count, logFormat);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(CommPhrases.WriteDataError + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Записать строку
        /// </summary>
        public override void WriteLine(string text, out string logText)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(text + NewLine);
                Write(buffer, 0, buffer.Length, CommUtils.ProtocolLogFormats.String, out logText);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(CommPhrases.WriteLineError + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Вернуть строковое представление соединения
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(RemoteAddress))
                sb.Append(RemoteAddress).Append("; ");

            if (RelatedKP != null)
                sb.Append(RelatedKP.Caption).Append("; ");

            sb.Append(ActivityStr).Append(ActivityDT.ToString("T", Localization.Culture));
            return sb.ToString();
        }


        /// <summary>
        /// Установить TCP-соединение
        /// </summary>
        public void Open(IPAddress addr, int port)
        {
            DateTime nowDT = DateTime.Now;

            if ((nowDT - connectDT).TotalSeconds >= ConnectPeriod || nowDT < connectDT /*время переведено назад*/)
            {
                connectDT = nowDT;

                try
                {
                    tcpClient.Connect(addr, port);
                    TakeAddresses();
                }
                catch (Exception ex)
                {
                    WriteToLog((Localization.UseRussian ? 
                        "Ошибка при установке TCP-соединения: " :
                        "Error establishing TCP connection: ") + ex.Message);
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format(Localization.UseRussian ?
                    "Попытка установки TCP-соединения может быть не ранее, чем через {0} с после предыдущей." :
                    "Attempt to establish TCP connection can not be earlier than {0} seconds after the previous.",
                    ConnectPeriod));
            }
        }

        /// <summary>
        /// Закрыть соединение
        /// </summary>
        public void Close()
        {
            try { NetStream.Close(); }
            catch { }

            try { TcpClient.Close(); }
            catch { }
        }

        /// <summary>
        /// Считать данные, доступные на текущий момент
        /// </summary>
        public int ReadAvailable(byte[] buffer, int offset, CommUtils.ProtocolLogFormats logFormat, out string logText)
        {
            try
            {
                int count = tcpClient.Available;
                int readCnt = NetStream.Read(buffer, offset, count);
                logText = BuildReadLogText(buffer, offset, count, readCnt, logFormat);

                if (readCnt > 0)
                    UpdateActivityDT();

                return readCnt;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(CommPhrases.ReadAvailableError + ": " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Очистить поток данных TCP-соединения
        /// </summary>
        public void ClearNetStream(byte[] buffer)
        {
            try
            {
                if (NetStream.DataAvailable)
                    NetStream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(CommPhrases.ClearDataStreamError + ": " + ex.Message, ex);
            }
        }
    }
}

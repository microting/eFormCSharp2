﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2020 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using eFormCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microting.eForm.Dto;
using Microting.eForm.Helpers;
using Microting.eForm.Infrastructure;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;
using Microting.eForm.Infrastructure.Models;
using System.IO;
using eFormSDK.Integration.CoreTests;
using Microting.eForm.Infrastructure.Helpers;

namespace eFormSDK.Integration.Tests
{
    [TestFixture]
    public class CoreTestUploadedData : DbTestFixture
    {
        private Core sut;
        private TestHelpers testHelpers;
        private string path;

        public override async Task DoSetup()
        {
            #region Setup SettingsTableContent

            DbContextHelper dbContextHelper = new DbContextHelper(ConnectionString);
            SqlController sql = new SqlController(dbContextHelper);
            await sql.SettingUpdate(Settings.token, "abc1234567890abc1234567890abcdef");
            await sql.SettingUpdate(Settings.firstRunDone, "true");
            await sql.SettingUpdate(Settings.knownSitesDone, "true");
            #endregion

            sut = new Core();
            sut.HandleCaseCreated += EventCaseCreated;
            sut.HandleCaseRetrived += EventCaseRetrived;
            sut.HandleCaseCompleted += EventCaseCompleted;
            sut.HandleCaseDeleted += EventCaseDeleted;
            sut.HandleFileDownloaded += EventFileDownloaded;
            sut.HandleSiteActivated += EventSiteActivated;
            await sut.StartSqlOnly(ConnectionString);
            path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            path = System.IO.Path.GetDirectoryName(path).Replace(@"file:", "");
            await sut.SetSdkSetting(Settings.fileLocationPicture, Path.Combine(path, "output", "dataFolder", "picture"));
            await sut.SetSdkSetting(Settings.fileLocationPdf, Path.Combine(path, "output", "dataFolder", "pdf"));
            await sut.SetSdkSetting(Settings.fileLocationJasper, Path.Combine(path, "output", "dataFolder", "reports"));
            testHelpers = new TestHelpers();
            //sut.StartLog(new CoreBase());
        }

        #region uploaded_datas
        [Test]
        public async Task Core_UploadedData_UploadedDataRead_DoesReturnOneUploadedDataClass()
        {
            // Arrance
            string checksum = "";
            string extension = "jpg";
            string currentFile = "Hello.jpg";
            int uploaderId = 1;
            string fileLocation = @"c:\here";
            string fileName = "Hello.jpg";

            // Act
            uploaded_data dU = new uploaded_data
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Extension = extension,
                UploaderId = uploaderId,
                UploaderType = Constants.UploaderTypes.System,
                WorkflowState = Constants.WorkflowStates.PreCreated,
                Version = 1,
                Local = 0,
                FileLocation = fileLocation,
                FileName = fileName,
                CurrentFile = currentFile,
                Checksum = checksum
            };


            DbContext.uploaded_data.Add(dU);
            await DbContext.SaveChangesAsync().ConfigureAwait(false);

            UploadedData ud = await sut.Advanced_UploadedDataRead(dU.Id);

            // Assert
            Assert.NotNull(ud);
            Assert.AreEqual(ud.Id, dU.Id);
            Assert.AreEqual(ud.Extension, dU.Extension);
            Assert.AreEqual(ud.UploaderId, dU.UploaderId);
            Assert.AreEqual(ud.UploaderType, dU.UploaderType);
            Assert.AreEqual(ud.FileLocation, dU.FileLocation);
            Assert.AreEqual(ud.FileName, dU.FileName);
            Assert.AreEqual(ud.CurrentFile, dU.CurrentFile);
            Assert.AreEqual(ud.Checksum, dU.Checksum);

        }
        #endregion

        #region eventhandlers
        public void EventCaseCreated(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventCaseRetrived(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventCaseCompleted(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventCaseDeleted(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventFileDownloaded(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }

        public void EventSiteActivated(object sender, EventArgs args)
        {
            // Does nothing for web implementation
        }
        #endregion
    }

}
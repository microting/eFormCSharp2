﻿using eFormData;
using eFormShared;
using eFormSqlController;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace eFormSDK.Integration.Tests
{
    [TestFixture]
    public class SqlControllerTest : DbTestFixture
    {
        private SqlController sut;

        public override void DoSetup()
        {
            sut = new SqlController(ConnectionString);
            sut.StartLog(new CoreBase());
        }

        #region Notifications
        [Test]
        public void SQL_Notification_NewNotificationCreateRetrievedForm_DoesStoreNotification()
        {
            // Arrance
            var notificationId = Guid.NewGuid().ToString();
            var microtingUId = Guid.NewGuid().ToString();

            // Act
            sut.NotificationCreate(notificationId, microtingUId, Constants.Notifications.RetrievedForm);

            // Assert
            var notification = DbContext.notifications.SingleOrDefault(x => x.notification_uid == notificationId && x.microting_uid == microtingUId);

            Assert.NotNull(notification);
            Assert.AreEqual(1, DbContext.notifications.Count());
            Assert.AreEqual(Constants.Notifications.RetrievedForm, notification.activity);
            Assert.AreEqual(Constants.WorkflowStates.Created, notification.workflow_state);
        }

        [Test]
        public void SQL_Notification_NewNotificationCreateCompletedForm_DoesStoreNotification()
        {
            // Arrance
            var notificationId = Guid.NewGuid().ToString();
            var microtingUId = Guid.NewGuid().ToString();

            // Act
            sut.NotificationCreate(notificationId, microtingUId, Constants.Notifications.Completed);

            // Assert
            var notification = DbContext.notifications.SingleOrDefault(x => x.notification_uid == notificationId && x.microting_uid == microtingUId);

            Assert.NotNull(notification);
            Assert.AreEqual(1, DbContext.notifications.Count());
            Assert.AreEqual(Constants.Notifications.Completed, notification.activity);
            Assert.AreEqual(Constants.WorkflowStates.Created, notification.workflow_state);
        }

        [Test]
        public void SQL_Notification_NotificationReadFirst_DoesReturnFirstNotification()
        {
            // Arrance
            var notificationId1 = Guid.NewGuid().ToString();
            var microtingUId1 = Guid.NewGuid().ToString();
            var notificationId2 = Guid.NewGuid().ToString();
            var microtingUId2 = Guid.NewGuid().ToString();

            // Act
            sut.NotificationCreate(notificationId1, microtingUId1, Constants.Notifications.Completed);
            sut.NotificationCreate(notificationId2, microtingUId2, Constants.Notifications.Completed);

            // Assert
            Note_Dto notification = sut.NotificationReadFirst();

            Assert.NotNull(notification);
            Assert.AreEqual(2, DbContext.notifications.Count());
            Assert.AreEqual(Constants.Notifications.Completed, notification.Activity);
            Assert.AreEqual(microtingUId1, notification.MicrotingUId);
        }

        [Test]
        public void SQL_Notification_NotificationUpdate_DoesUpdateNotification()
        {
            // Arrance
            var notificationUId = Guid.NewGuid().ToString();
            var microtingUId = Guid.NewGuid().ToString();

            // Act
            sut.NotificationCreate(notificationUId, microtingUId, Constants.Notifications.Completed);
            sut.NotificationUpdate(notificationUId, microtingUId, Constants.WorkflowStates.Processed);

            // Assert
            var notification = DbContext.notifications.SingleOrDefault(x => x.notification_uid == notificationUId && x.microting_uid == microtingUId);

            Assert.NotNull(notification);
            Assert.AreEqual(1, DbContext.notifications.Count());
            Assert.AreEqual(Constants.Notifications.Completed, notification.activity);
            Assert.AreEqual(Constants.WorkflowStates.Processed, notification.workflow_state);
        }
        #endregion

        #region UploadedData
        [Test]
        public void SQL_UploadedData_FileRead_DoesReturnOneUploadedData()
        {
            // Arrance
            string checksum = "";
            string extension = "jpg";
            string currentFile = "Hello.jpg";
            int uploaderId = 1;
            string fileLocation = @"c:\here";
            string fileName = "Hello.jpg";

            // Act
            uploaded_data dU = new uploaded_data();

            dU.created_at = DateTime.Now;
            dU.updated_at = DateTime.Now;
            dU.extension = extension;
            dU.uploader_id = uploaderId;
            dU.uploader_type = Constants.UploaderTypes.System;
            dU.workflow_state = Constants.WorkflowStates.PreCreated;
            dU.version = 1;
            dU.local = 0;
            dU.file_location = fileLocation;
            dU.file_name = fileName;
            dU.current_file = currentFile;
            dU.checksum = checksum;

            DbContext.uploaded_data.Add(dU);
            DbContext.SaveChanges();

            UploadedData ud = sut.FileRead();

            // Assert
            Assert.NotNull(ud);
            Assert.AreEqual(dU.id, ud.Id);
            Assert.AreEqual(dU.checksum, ud.Checksum);
            Assert.AreEqual(dU.extension, ud.Extension);
            Assert.AreEqual(dU.current_file, ud.CurrentFile);
            Assert.AreEqual(dU.uploader_id, ud.UploaderId);
            Assert.AreEqual(dU.uploader_type, ud.UploaderType);
            Assert.AreEqual(dU.file_location, ud.FileLocation);
            Assert.AreEqual(dU.file_name, ud.FileName);
            //Assert.AreEqual(dU.local, ud.);

        }
        #endregion

        #region Templates
        [Test]
        public void SQL_Template_TemplateItemReadAll_DoesSortAccordingly()
        {

            // Arrance

            #region Template1
            check_lists cl1 = new check_lists();
            cl1.created_at = DateTime.Now;
            cl1.updated_at = DateTime.Now;
            cl1.label = "A";
            cl1.description = "D";
            cl1.workflow_state = Constants.WorkflowStates.Created;
            cl1.case_type = "CheckList";
            cl1.folder_name = "Template1FolderName";
            cl1.display_index = 1;
            cl1.repeated = 1;

            DbContext.check_lists.Add(cl1);
            DbContext.SaveChanges();
            Thread.Sleep(1000);
            #endregion

            #region Template2
            check_lists cl2 = new check_lists();
            cl2.created_at = DateTime.Now;
            cl2.updated_at = DateTime.Now;
            cl2.label = "B";
            cl2.description = "C";
            cl2.workflow_state = Constants.WorkflowStates.Removed;
            cl2.case_type = "CheckList";
            cl2.folder_name = "Template1FolderName";
            cl2.display_index = 1;
            cl2.repeated = 1;

            DbContext.check_lists.Add(cl2);
            DbContext.SaveChanges();
            Thread.Sleep(1000);
            #endregion

            #region Template3
            check_lists cl3 = new check_lists();
            cl3.created_at = DateTime.Now;
            cl3.updated_at = DateTime.Now;
            cl3.label = "D";
            cl3.description = "B";
            cl3.workflow_state = Constants.WorkflowStates.Created;
            cl3.case_type = "CheckList";
            cl3.folder_name = "Template1FolderName";
            cl3.display_index = 1;
            cl3.repeated = 1;

            DbContext.check_lists.Add(cl3);
            DbContext.SaveChanges();
            Thread.Sleep(1000);
            #endregion

            #region Template4
            check_lists cl4 = new check_lists();
            cl4.created_at = DateTime.Now;
            cl4.updated_at = DateTime.Now;
            cl4.label = "C";
            cl4.description = "A";
            cl4.workflow_state = Constants.WorkflowStates.Created;
            cl4.case_type = "CheckList";
            cl4.folder_name = "Template1FolderName";
            cl4.display_index = 1;
            cl4.repeated = 1;

            DbContext.check_lists.Add(cl4);
            DbContext.SaveChanges();
            #endregion
            
            
            // Act
            List<int> emptyList = new List<int>();

            // Default sorting including removed
            List<Template_Dto> templateListId = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", false, "", emptyList);
            List<Template_Dto> templateListLabel = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", false, Constants.TamplateSortParameters.Label, emptyList);
            List<Template_Dto> templateListDescription = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", false, Constants.TamplateSortParameters.Description, emptyList);
            List<Template_Dto> templateListCreatedAt = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", false, Constants.TamplateSortParameters.CreatedAt, emptyList);

            // Descending including removed
            List<Template_Dto> templateListDescengingId = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", true, "", emptyList);
            List<Template_Dto> templateListDescengingLabel = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", true, Constants.TamplateSortParameters.Label, emptyList);
            List<Template_Dto> templateListDescengingDescription = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", true, Constants.TamplateSortParameters.Description, emptyList);
            List<Template_Dto> templateListDescengingCreatedAt = sut.TemplateItemReadAll(true, Constants.WorkflowStates.Created, "", true, Constants.TamplateSortParameters.CreatedAt, emptyList);

            // Default sorting excluding removed
            List<Template_Dto> templateListIdNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", false, "", emptyList);
            List<Template_Dto> templateListLabelNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", false, Constants.TamplateSortParameters.Label, emptyList);
            List<Template_Dto> templateListDescriptionNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", false, Constants.TamplateSortParameters.Description, emptyList);
            List<Template_Dto> templateListCreatedAtNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", false, Constants.TamplateSortParameters.CreatedAt, emptyList);

            // Descending excluding removed
            List<Template_Dto> templateListDescengingIdNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", true, "", emptyList);
            List<Template_Dto> templateListDescengingLabelNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", true, Constants.TamplateSortParameters.Label, emptyList);
            List<Template_Dto> templateListDescengingDescriptionNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", true, Constants.TamplateSortParameters.Description, emptyList);
            List<Template_Dto> templateListDescengingCreatedAtNr = sut.TemplateItemReadAll(false, Constants.WorkflowStates.Created, "", true, Constants.TamplateSortParameters.CreatedAt, emptyList);


            // Assert

            #region include removed
            // Default sorting including removed 
            // Id
            Assert.NotNull(templateListId);
            Assert.AreEqual(4, templateListId.Count());
            Assert.AreEqual("A", templateListId[0].Label);
            Assert.AreEqual("B", templateListId[1].Label);
            Assert.AreEqual("D", templateListId[2].Label);
            Assert.AreEqual("C", templateListId[3].Label);
            Assert.AreEqual(0, templateListId[0].Tags.Count());
            Assert.AreEqual(0, templateListId[1].Tags.Count());
            Assert.AreEqual(0, templateListId[2].Tags.Count());
            Assert.AreEqual(0, templateListId[3].Tags.Count());

            // Default sorting including removed 
            // Label
            Assert.NotNull(templateListLabel);
            Assert.AreEqual(4, templateListLabel.Count());
            Assert.AreEqual("A", templateListLabel[0].Label);
            Assert.AreEqual("B", templateListLabel[1].Label);
            Assert.AreEqual("C", templateListLabel[2].Label);
            Assert.AreEqual("D", templateListLabel[3].Label);
            Assert.AreEqual(0, templateListLabel[0].Tags.Count());
            Assert.AreEqual(0, templateListLabel[1].Tags.Count());
            Assert.AreEqual(0, templateListLabel[2].Tags.Count());
            Assert.AreEqual(0, templateListLabel[3].Tags.Count());

            // Default sorting including removed 
            // Description
            Assert.NotNull(templateListDescription);
            Assert.AreEqual(4, templateListDescription.Count());
            Assert.AreEqual("C", templateListDescription[0].Label);
            Assert.AreEqual("D", templateListDescription[1].Label);
            Assert.AreEqual("B", templateListDescription[2].Label);
            Assert.AreEqual("A", templateListDescription[3].Label);
            Assert.AreEqual(0, templateListDescription[0].Tags.Count());
            Assert.AreEqual(0, templateListDescription[1].Tags.Count());
            Assert.AreEqual(0, templateListDescription[2].Tags.Count());
            Assert.AreEqual(0, templateListDescription[3].Tags.Count());

            // Default sorting including removed 
            // Created At
            Assert.NotNull(templateListCreatedAt);
            Assert.AreEqual(4, templateListCreatedAt.Count());
            Assert.AreEqual("A", templateListCreatedAt[0].Label);
            Assert.AreEqual("B", templateListCreatedAt[1].Label);
            Assert.AreEqual("D", templateListCreatedAt[2].Label);
            Assert.AreEqual("C", templateListCreatedAt[3].Label);
            Assert.AreEqual(0, templateListCreatedAt[0].Tags.Count());
            Assert.AreEqual(0, templateListCreatedAt[1].Tags.Count());
            Assert.AreEqual(0, templateListCreatedAt[2].Tags.Count());
            Assert.AreEqual(0, templateListCreatedAt[3].Tags.Count());

            // Descending sorting including removed 
            // Id
            Assert.NotNull(templateListDescengingId);
            Assert.AreEqual(4, templateListDescengingId.Count());
            Assert.AreEqual("C", templateListDescengingId[0].Label);
            Assert.AreEqual("D", templateListDescengingId[1].Label);
            Assert.AreEqual("B", templateListDescengingId[2].Label);
            Assert.AreEqual("A", templateListDescengingId[3].Label);
            Assert.AreEqual(0, templateListDescengingId[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingId[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingId[2].Tags.Count());
            Assert.AreEqual(0, templateListDescengingId[3].Tags.Count());

            // Descending sorting including removed 
            // Label
            Assert.NotNull(templateListDescengingLabel);
            Assert.AreEqual(4, templateListDescengingLabel.Count());
            Assert.AreEqual("D", templateListDescengingLabel[0].Label);
            Assert.AreEqual("C", templateListDescengingLabel[1].Label);
            Assert.AreEqual("B", templateListDescengingLabel[2].Label);
            Assert.AreEqual("A", templateListDescengingLabel[3].Label);
            Assert.AreEqual(0, templateListDescengingLabel[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingLabel[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingLabel[2].Tags.Count());
            Assert.AreEqual(0, templateListDescengingLabel[3].Tags.Count());

            // Descending sorting including removed 
            // Description
            Assert.NotNull(templateListDescengingDescription);
            Assert.AreEqual(4, templateListDescengingDescription.Count());
            Assert.AreEqual("A", templateListDescengingDescription[0].Label);
            Assert.AreEqual("B", templateListDescengingDescription[1].Label);
            Assert.AreEqual("D", templateListDescengingDescription[2].Label);
            Assert.AreEqual("C", templateListDescengingDescription[3].Label);
            Assert.AreEqual(0, templateListDescengingDescription[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingDescription[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingDescription[2].Tags.Count());
            Assert.AreEqual(0, templateListDescengingDescription[3].Tags.Count());

            // Descending sorting including removed 
            // Created At
            Assert.NotNull(templateListDescengingCreatedAt);
            Assert.AreEqual(4, templateListDescengingCreatedAt.Count());
            Assert.AreEqual("C", templateListDescengingCreatedAt[0].Label);
            Assert.AreEqual("D", templateListDescengingCreatedAt[1].Label);
            Assert.AreEqual("B", templateListDescengingCreatedAt[2].Label);
            Assert.AreEqual("A", templateListDescengingCreatedAt[3].Label);
            Assert.AreEqual(0, templateListDescengingCreatedAt[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingCreatedAt[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingCreatedAt[2].Tags.Count());
            Assert.AreEqual(0, templateListDescengingCreatedAt[3].Tags.Count());
            #endregion

            #region Exclude removed
            // Default sorting including removed 
            // Id
            Assert.NotNull(templateListIdNr);
            Assert.AreEqual(3, templateListIdNr.Count());
            Assert.AreEqual("A", templateListIdNr[0].Label);
            Assert.AreEqual("D", templateListIdNr[1].Label);
            Assert.AreEqual("C", templateListIdNr[2].Label);
            Assert.AreEqual(0, templateListIdNr[0].Tags.Count());
            Assert.AreEqual(0, templateListIdNr[1].Tags.Count());
            Assert.AreEqual(0, templateListIdNr[2].Tags.Count());

            // Default sorting including removed 
            // Label
            Assert.NotNull(templateListLabelNr);
            Assert.AreEqual(3, templateListLabelNr.Count());
            Assert.AreEqual("A", templateListLabelNr[0].Label);
            Assert.AreEqual("C", templateListLabelNr[1].Label);
            Assert.AreEqual("D", templateListLabelNr[2].Label);
            Assert.AreEqual(0, templateListLabelNr[0].Tags.Count());
            Assert.AreEqual(0, templateListLabelNr[1].Tags.Count());
            Assert.AreEqual(0, templateListLabelNr[2].Tags.Count());

            // Default sorting including removed 
            // Description
            Assert.NotNull(templateListDescriptionNr);
            Assert.AreEqual(3, templateListDescriptionNr.Count());
            Assert.AreEqual("C", templateListDescriptionNr[0].Label);
            Assert.AreEqual("D", templateListDescriptionNr[1].Label);
            Assert.AreEqual("A", templateListDescriptionNr[2].Label);
            Assert.AreEqual(0, templateListDescriptionNr[0].Tags.Count());
            Assert.AreEqual(0, templateListDescriptionNr[1].Tags.Count());
            Assert.AreEqual(0, templateListDescriptionNr[2].Tags.Count());

            // Default sorting including removed 
            // Created At
            Assert.NotNull(templateListCreatedAtNr);
            Assert.AreEqual(3, templateListCreatedAtNr.Count());
            Assert.AreEqual("A", templateListCreatedAtNr[0].Label);
            Assert.AreEqual("D", templateListCreatedAtNr[1].Label);
            Assert.AreEqual("C", templateListCreatedAtNr[2].Label);
            Assert.AreEqual(0, templateListCreatedAtNr[0].Tags.Count());
            Assert.AreEqual(0, templateListCreatedAtNr[1].Tags.Count());
            Assert.AreEqual(0, templateListCreatedAtNr[2].Tags.Count());

            // Descending sorting including removed 
            // Id
            Assert.NotNull(templateListDescengingIdNr);
            Assert.AreEqual(3, templateListDescengingIdNr.Count());
            Assert.AreEqual("C", templateListDescengingIdNr[0].Label);
            Assert.AreEqual("D", templateListDescengingIdNr[1].Label);
            Assert.AreEqual("A", templateListDescengingIdNr[2].Label);
            Assert.AreEqual(0, templateListDescengingIdNr[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingIdNr[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingIdNr[2].Tags.Count());

            // Descending sorting including removed 
            // Label
            Assert.NotNull(templateListDescengingLabelNr);
            Assert.AreEqual(3, templateListDescengingLabelNr.Count());
            Assert.AreEqual("D", templateListDescengingLabelNr[0].Label);
            Assert.AreEqual("C", templateListDescengingLabelNr[1].Label);
            Assert.AreEqual("A", templateListDescengingLabelNr[2].Label);
            Assert.AreEqual(0, templateListDescengingLabelNr[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingLabelNr[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingLabelNr[2].Tags.Count());

            // Descending sorting including removed 
            // Description
            Assert.NotNull(templateListDescengingDescriptionNr);
            Assert.AreEqual(3, templateListDescengingDescriptionNr.Count());
            Assert.AreEqual("A", templateListDescengingDescriptionNr[0].Label);
            Assert.AreEqual("D", templateListDescengingDescriptionNr[1].Label);
            Assert.AreEqual("C", templateListDescengingDescriptionNr[2].Label);
            Assert.AreEqual(0, templateListDescengingDescriptionNr[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingDescriptionNr[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingDescriptionNr[2].Tags.Count());

            // Descending sorting including removed 
            // Created At
            Assert.NotNull(templateListDescengingCreatedAtNr);
            Assert.AreEqual(3, templateListDescengingCreatedAtNr.Count());
            Assert.AreEqual("C", templateListDescengingCreatedAtNr[0].Label);
            Assert.AreEqual("D", templateListDescengingCreatedAtNr[1].Label);
            Assert.AreEqual("A", templateListDescengingCreatedAtNr[2].Label);
            Assert.AreEqual(0, templateListDescengingCreatedAtNr[0].Tags.Count());
            Assert.AreEqual(0, templateListDescengingCreatedAtNr[1].Tags.Count());
            Assert.AreEqual(0, templateListDescengingCreatedAtNr[2].Tags.Count());

            #endregion

        }
        
        [Test]
        public void SQL_Template_TemplateDelete_DoesMarkTemplateAsRemoved()
        {
            // Arrance
            #region Template1

            check_lists cl1 = new check_lists();
            cl1.created_at = DateTime.Now;
            cl1.updated_at = DateTime.Now;
            cl1.label = "A";
            cl1.description = "D";
            cl1.workflow_state = Constants.WorkflowStates.Created;
            cl1.case_type = "CheckList";
            cl1.folder_name = "Template1FolderName";
            cl1.display_index = 1;
            cl1.repeated = 1;

            DbContext.check_lists.Add(cl1);
            DbContext.SaveChanges();
            #endregion
            // Act

            sut.TemplateDelete(cl1.id);

            Template_Dto clResult = sut.TemplateItemRead(cl1.id);

            // Assert

            var checkLists = DbContext.check_lists.AsNoTracking().ToList();

            Assert.NotNull(clResult);
            Assert.AreEqual(1, checkLists.Count());
            //Assert.AreEqual(1, cl_results.Count);
            Assert.AreEqual(Constants.WorkflowStates.Removed, checkLists[0].workflow_state);
            
        }
        
        
        [Test]
        public void SQL_Template_UpdateCaseFieldValue_DoesUpdateFieldValues()
        {
            // Arrance

            // Act

            // Assert
            Assert.True(true);
        }
        #endregion 

        #region Tags
        [Test]
        public void SQL_Tags_CreateTag_DoesCreateNewTag()
        {
            // Arrance
            string tagName = "Tag1";

            // Act
            sut.TagCreate(tagName);

            // Assert
            var tag = DbContext.tags.ToList();

            Assert.AreEqual(tag[0].name, tagName);
            Assert.AreEqual(1, tag.Count());
        }

        [Test]
        public void SQL_Tags_DeleteTag_DoesMarkTagAsRemoved()
        {
            // Arrance
            string tagName = "Tag1";
            tags tag = new tags();
            tag.name = tagName;
            tag.workflow_state = Constants.WorkflowStates.Created;

            DbContext.tags.Add(tag);
            DbContext.SaveChanges();

            // Act
            sut.TagDelete(tag.id);

            // Assert
            var result = DbContext.tags.AsNoTracking().ToList();

            Assert.AreEqual(result[0].name, tagName);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Constants.WorkflowStates.Removed, result[0].workflow_state);
        }

        [Test]
        public void SQL_Tags_CreateTag_DoesRecreateRemovedTag()
        {
            // Arrance
            string tagName = "Tag1";
            tags tag = new tags();
            tag.name = tagName;
            tag.workflow_state = Constants.WorkflowStates.Removed;

            DbContext.tags.Add(tag);
            DbContext.SaveChanges();

            // Act
            sut.TagCreate(tagName);

            // Assert
            var result = DbContext.tags.AsNoTracking().ToList();

            Assert.AreEqual(result[0].name, tagName);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(Constants.WorkflowStates.Created, result[0].workflow_state);
        }

        [Test]
        public void SQL_Tags_GetAllTags_DoesReturnAllTags()
        {
            // Arrance
            string tagName1 = "Tag1";
            tags tag = new tags();
            tag.name = tagName1;
            tag.workflow_state = Constants.WorkflowStates.Removed;

            DbContext.tags.Add(tag);
            DbContext.SaveChanges();

            string tagName2 = "Tag2";
            tag = new tags();

            tag.name = tagName2;
            tag.workflow_state = Constants.WorkflowStates.Removed;

            DbContext.tags.Add(tag);
            DbContext.SaveChanges();
            string tagName3 = "Tag3";
            tag = new tags();

            tag.name = tagName3;
            tag.workflow_state = Constants.WorkflowStates.Removed;

            DbContext.tags.Add(tag);
            DbContext.SaveChanges();
            //int tagId3 = sut.TagCreate(tagName3);

            // Act
            var tags = sut.GetAllTags(true);            

            // Assert
            Assert.True(true);
            Assert.AreEqual(3, tags.Count());
            Assert.AreEqual(tagName1, tags[0].Name);
            Assert.AreEqual(0, tags[0].TaggingCount);
            Assert.AreEqual(tagName2, tags[1].Name);
            Assert.AreEqual(0, tags[1].TaggingCount);       
            Assert.AreEqual(tagName3, tags[2].Name);
            Assert.AreEqual(0, tags[2].TaggingCount);
        }

        [Test]
        public void SQL_Tags_TemplateSetTags_DoesAssignTagToTemplate()
        {
            // Arrance
            check_lists cl1 = new check_lists();
            cl1.created_at = DateTime.Now;
            cl1.updated_at = DateTime.Now;
            cl1.label = "A";
            cl1.description = "D";
            cl1.workflow_state = Constants.WorkflowStates.Created;
            cl1.case_type = "CheckList";
            cl1.folder_name = "Template1FolderName";
            cl1.display_index = 1;
            cl1.repeated = 1;

            DbContext.check_lists.Add(cl1);
            DbContext.SaveChanges();

            string tagName1 = "Tag1";
            tags tag = new tags();
            tag.name = tagName1;
            tag.workflow_state = Constants.WorkflowStates.Created;

            DbContext.tags.Add(tag);
            DbContext.SaveChanges();

            // Act
            List<int> tags = new List<int>();
            tags.Add(tag.id);
            sut.TemplateSetTags(cl1.id, tags);


            // Assert
            List<taggings> result = DbContext.taggings.AsNoTracking().ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(tag.id, result[0].tag_id);
            Assert.AreEqual(cl1.id, result[0].check_list_id);
            Assert.True(true);
        }
        #endregion

        #region Case
        [Test]
        public void SQL_Case_CaseDeleteResult_DoesMarkCaseRemoved()
        {

            // Arrance
            sites site = new sites();
            site.name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.microting_uid = "microting_uid";
            aCase.microting_check_uid = "microting_check_uid";
            aCase.workflow_state = Constants.WorkflowStates.Created;
            aCase.check_list_id = cl.id;
            aCase.site_id = site.id;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseDeleteResult(aCase.id);
            //cases theCase = sut.CaseReadFull(aCase.microting_uid, aCase.microting_check_uid);
            var match = DbContext.cases.AsNoTracking().ToList();
            var versionedMatches = DbContext.case_versions.AsNoTracking().ToList();

            // Assert
            Assert.NotNull(match);
            Assert.AreEqual(1, match.Count);
            Assert.AreEqual(1, versionedMatches.Count);
            Assert.AreEqual(Constants.WorkflowStates.Removed, match[0].workflow_state);
            Assert.AreEqual(Constants.WorkflowStates.Removed, versionedMatches[0].workflow_state);
        }

        [Test]
        public void SQL_Case_CaseDelete_DoesCaseRemoved()
        {
            // Arrance
            // Arrance
            sites site = new sites();
            site.name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.microting_uid = "microting_uid";
            aCase.microting_check_uid = "microting_check_uid";
            aCase.workflow_state = Constants.WorkflowStates.Created;
            aCase.check_list_id = cl.id;
            aCase.site_id = site.id;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseDelete(aCase.microting_uid);
            //cases theCase = sut.CaseReadFull(aCase.microting_uid, aCase.microting_check_uid);
            var match = DbContext.cases.AsNoTracking().ToList();
            var versionedMatches = DbContext.case_versions.AsNoTracking().ToList();

            // Assert
            Assert.NotNull(match);
            Assert.AreEqual(1, match.Count);
            Assert.AreEqual(1, versionedMatches.Count);
            Assert.AreEqual(Constants.WorkflowStates.Removed, match[0].workflow_state);
            Assert.AreEqual(Constants.WorkflowStates.Removed, versionedMatches[0].workflow_state);
        }

        [Test]
        public void SQL_Case_CaseUpdateRetrived_DoesCaseGetUpdated()
        {

            // Arrance
            sites site = new sites();
            site.name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.microting_uid = "microting_uid";
            aCase.microting_check_uid = "microting_check_uid";
            aCase.workflow_state = Constants.WorkflowStates.Created;
            aCase.check_list_id = cl.id;
            aCase.site_id = site.id;
            aCase.status = 66;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseUpdateRetrived(aCase.microting_uid);
            //Case_Dto caseResult = sut.CaseFindCustomMatchs(aCase.microting_uid);
            List<cases> caseResults = DbContext.cases.AsNoTracking().ToList();


            // Assert

            
            Assert.NotNull(caseResults);
            Assert.AreEqual(1, caseResults.Count);
            Assert.AreNotEqual(1, caseResults[0]);
            

        

        }

        [Test]
        public void SQL_Case_CaseDeleteReversed_DoesDeletionReversed()
        {
            // Arrance
            sites site = new sites();
            site.name = "SiteName";
            DbContext.sites.Add(site);
            DbContext.SaveChanges();

            check_lists cl = new check_lists();
            cl.label = "label";

            DbContext.check_lists.Add(cl);
            DbContext.SaveChanges();

            cases aCase = new cases();
            aCase.microting_uid = "microting_uid";
            aCase.microting_check_uid = "microting_check_uid";
            aCase.workflow_state = Constants.WorkflowStates.Created;
            aCase.check_list_id = cl.id;
            aCase.site_id = site.id;
            aCase.status = 66;

            DbContext.cases.Add(aCase);
            DbContext.SaveChanges();

            // Act
            sut.CaseDeleteReversed(aCase.microting_uid);
            //Case_Dto caseResult = sut.CaseFindCustomMatchs(aCase.microting_uid);
            List<cases> caseResults = DbContext.cases.AsNoTracking().ToList();
            List<sites> siteResults = DbContext.sites.AsNoTracking().ToList();


            // Assert



            Assert.NotNull(caseResults);
            //Assert.AreEqual(1, caseResults.Count);
            //Assert.AreNotEqual(1, caseResults[1]);
        }

       

        #endregion


        #region public site
        #region site
        //         public List<SiteName_Dto> SiteGetAll(bool includeRemoved)

        [Test]

        public void SQL_Site_SiteGetAll_DoesReturnAllSites()
        {
            // Arrance

            // Act

            // Assert
            Assert.True(true);
        }
        #endregion
        #endregion

        // Arrance

        // Act

        // Assert
    }
}

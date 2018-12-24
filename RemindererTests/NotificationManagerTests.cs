using Moq;
using NUnit.Framework;
using Reminderer;
using Reminderer.Models;
using Reminderer.Repositories;
using System;
using System.Collections.Generic;

namespace RemindererTests
{
    [TestFixture]
    class NotificationManagerTests
    {
        public Mock<IReminderRepository> reminderRepository;
        public Mock<IScheduleRepository> scheduleRepository;
        public DateTime baseTime;
        private NotificationManager notificationManager;

        [SetUp]
        public void SetUp()
        {
            baseTime = new DateTime(2019, 01, 01);

            reminderRepository = new Mock<IReminderRepository>();
            scheduleRepository = new Mock<IScheduleRepository>();
            reminderRepository.Setup(x => x.ReadAll()).Returns(GetSampleReminders());
            scheduleRepository.Setup(x => x.ReadAll()).Returns(GetSampleSchedules());

            notificationManager = new NotificationManager(reminderRepository.Object, scheduleRepository.Object);
        }

        [Test]
        public void NotificationManager_LoadRemindersAndSchedules_ValidCall()
        {
            Assert.True(notificationManager.Schedules.Count == 3);
            Assert.True(notificationManager.Schedules[1].NumDaysBeforeNotify == 3);
            Assert.True(notificationManager.Reminders.Count == 3);
            Assert.True(notificationManager.Reminders[0].Type == Reminder.ReminderType.IsFromSavedTime);
        }



        private List<Schedule> GetSampleSchedules()
        {
            return new List<Schedule>{
                new Schedule
                {
                    Description = "Test Schedule 1",
                    ExtraDetail = "No notification option set",
                    DesiredDateTime = baseTime.AddDays(5),
                    Id = 1,
                    NumDaysBeforeNotify = -1,
                    ShouldRemind = false
                },
                new Schedule
                {
                    Description = "Test Schedule 2",
                    ExtraDetail = "Notify 3 days before Due date",
                    DesiredDateTime = baseTime.AddDays(3),
                    Id = 2,
                    NumDaysBeforeNotify = 3,
                    ShouldRemind = true
                },
                new Schedule
                {
                    Description = "Test Schedule 3",
                    ExtraDetail = "Notify 5 days before Due date",
                    DesiredDateTime = baseTime.AddDays(3),
                    Id = 3,
                    NumDaysBeforeNotify = 5,
                    ShouldRemind = true
                },
            };
        }
        private List<Reminder> GetSampleReminders()
        {
            return new List<Reminder>{
                new Reminder
                {
                    Description = "Test Reminder 1",
                    ExtraDetail = "Is From Save Time, 5 minutes from now",
                    DesiredDateTime = baseTime.AddMinutes(5),
                    Type = Reminder.ReminderType.IsFromSavedTime,
                    Id = 1,
                    ShouldRemind = true,
                    ShouldRepeat = false,
                    RepeatingDays = { },
                },
                new Reminder
                {
                    Description = "Test Reminder 2",
                    ExtraDetail = "Is At Set Interval, Every 3 minutes",
                    DesiredDateTime = baseTime.AddMinutes(3),
                    Type = Reminder.ReminderType.IsAtSetInterval,
                    Id = 2,
                    ShouldRemind = true,
                    ShouldRepeat = true,
                    RepeatingDays = { },
                },
                new Reminder
                {
                    Description = "Test Reminder 3",
                    ExtraDetail = "Is At Specific Time, 7PM Mon,Wed,Fri",
                    DesiredDateTime = baseTime.AddHours(19),
                    Type = Reminder.ReminderType.IsAtSpecificTime,
                    Id = 3,
                    ShouldRemind = true,
                    ShouldRepeat = true,
                    RepeatingDays = {Reminder.Days.Monday, Reminder.Days.Wednesday, Reminder.Days.Friday },
                },
            };

        }
    }
}

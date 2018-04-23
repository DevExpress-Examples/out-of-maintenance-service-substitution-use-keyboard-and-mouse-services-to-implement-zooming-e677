using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
using System.IO;
using DevExpress.Utils.Controls;
using DevExpress.Services;
using DevExpress.XtraEditors;

namespace ServicesMouseZoom {
    public partial class Form1 : XtraForm {

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

            IMouseHandlerService oldMouseHandler = (IMouseHandlerService)schedulerControl1.GetService(typeof(IMouseHandlerService));
            if (oldMouseHandler != null) {
                MyMouseHandlerService newMouseHandler = new MyMouseHandlerService(schedulerControl1, oldMouseHandler);
                schedulerControl1.RemoveService(typeof(IMouseHandlerService));
                schedulerControl1.AddService(typeof(IMouseHandlerService), newMouseHandler);
            }

            IKeyboardHandlerService oldKeyboardHandler = (IKeyboardHandlerService)schedulerControl1.GetService(typeof(IKeyboardHandlerService));
            if (oldKeyboardHandler != null) {
                MyKeyboardHandlerService newKeyboardHandler = new MyKeyboardHandlerService(schedulerControl1, oldKeyboardHandler);
                schedulerControl1.RemoveService(typeof(IKeyboardHandlerService));
                schedulerControl1.AddService(typeof(IKeyboardHandlerService), newKeyboardHandler);
            }

            CustomizeScheduler();
        }

        private void schedulerControl1_InitAppointmentImages(object sender, AppointmentImagesEventArgs e) {
            AppointmentImageInfo info = new AppointmentImageInfo();
            info.Image = this.sportsImages.Images[e.Appointment.LabelId];
            e.ImageInfoList.Add(info);

        }

        #region Initial Settings

        public const string sportEventsResourceName = "ServicesMouseZoom.Data.sportevents.xml";
        ImageCollection sportsImages;
        ImageCollection channelsImages;

        public void CustomizeScheduler() {
            schedulerControl1.ActiveViewType = SchedulerViewType.Day;
            schedulerControl1.WeekView.Enabled = false;
            schedulerControl1.WorkWeekView.Enabled = false;
            schedulerControl1.MonthView.Enabled = false;
            schedulerControl1.TimelineView.Enabled = false;
            schedulerControl1.GroupType = SchedulerGroupType.Resource;
            schedulerControl1.DayView.ResourcesPerPage = 3;
            schedulerControl1.ActiveViewType = SchedulerViewType.Day;
            schedulerControl1.Views.DayView.DayCount = 1;
            schedulerControl1.Views.MonthView.ResourcesPerPage = 2;
            schedulerControl1.Views.MonthView.WeekCount = 2;
            schedulerControl1.Views.TimelineView.ResourcesPerPage = 2;
            schedulerControl1.Views.WeekView.ResourcesPerPage = 2;
            schedulerControl1.Views.WorkWeekView.ResourcesPerPage = 2;
            AddSportChannels();
            FillData();
            schedulerControl1.Start = new DateTime(2008, 03, 12);
            schedulerControl1.DayView.ShowWorkTimeOnly = true;
        }


        void FillData() {
            this.schedulerStorage1.EnableReminders = false;
            this.schedulerStorage1.Appointments.Mappings.End = "Finish";
            this.schedulerStorage1.Appointments.Mappings.Label = "SportID";
            this.schedulerStorage1.Appointments.Mappings.ResourceId = "ResourceID";
            this.schedulerStorage1.Appointments.Mappings.Start = "Start";
            this.schedulerStorage1.Appointments.Mappings.Subject = "Caption";
            this.schedulerStorage1.Appointments.Mappings.AllDay = "AllDay";

            schedulerStorage1.Appointments.DataSource = GetSportEventsData();
        }

        static Stream GetResourceStream(string resourceName) {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        public static DataTable GetSportEventsData() {
            DataSet sportEventDS = new DataSet();
            using (Stream stream = GetResourceStream(sportEventsResourceName)) {
                sportEventDS.ReadXml(stream);
                stream.Close();
            }
            return sportEventDS.Tables[0];
        }

        void AddSportChannels() {
            this.sportsImages = ImageHelper.CreateImageCollectionFromResources("ServicesMouseZoom.Images.sports.png", System.Reflection.Assembly.GetExecutingAssembly(), new Size(16, 16));
            this.channelsImages = ImageHelper.CreateImageCollectionFromResources("ServicesMouseZoom.Images.channels.png", System.Reflection.Assembly.GetExecutingAssembly(), new Size(60, 40));

            schedulerStorage1.Resources.BeginUpdate();
            AddResource(0, "Channel 1");
            AddResource(1, "Channel 2");
            AddResource(2, "Channel 3");
            AddResource(3, "Channel 4");
            AddResource(4, "Channel 5");
            AddResource(5, "Channel 6");
            AddResource(6, "Channel 7");
            AddResource(7, "Channel 8");
            schedulerStorage1.Resources.EndUpdate();
        }
        void AddResource(int index, string caption) {
            Resource r = new Resource(index.ToString(), caption);
            r.Image = this.channelsImages.Images[index];
            r.Color = schedulerControl1.ResourceColorSchemas.GetSchema(index).CellLight;
            schedulerStorage1.Resources.Add(r);
        }
        #endregion
    }

}

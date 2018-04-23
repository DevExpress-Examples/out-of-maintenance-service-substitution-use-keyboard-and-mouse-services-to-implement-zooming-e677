using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Services;

namespace ServicesMouseZoom {
    public class MyMouseHandlerService : MouseHandlerServiceWrapper {
        IServiceProvider provider;
        bool myzoomEnabled;

        public MyMouseHandlerService(IServiceProvider provider, IMouseHandlerService service)
            : base(service) {
            this.provider = provider;
        }

        public override void OnMouseWheel(MouseEventArgs e) {
            DayView curView = ((SchedulerControl)provider).DayView;
            int nSlots = curView.TimeSlots.Count;

            if (this.myzoomEnabled) {
                if (e.Delta < 0) {
                    for (int i = 0; i < nSlots; i++) {
                        if (curView.TimeSlots[i].Value < curView.TimeScale) {
                            curView.TimeScale = curView.TimeSlots[i].Value;
                            break;
                        }
                    }

                }
                else {
                    for (int i = nSlots - 1; i >= 0; i--) {
                        if (curView.TimeSlots[i].Value > curView.TimeScale) {
                            curView.TimeScale = curView.TimeSlots[i].Value;
                            break;
                        }
                    }
                }
            }
            else {
                base.OnMouseWheel(e);
            }
        }

        public bool myZoomEnabled { get { return this.myzoomEnabled; } set { this.myzoomEnabled = value; } }


    }

    public class MyKeyboardHandlerService : KeyboardHandlerServiceWrapper {
        IServiceProvider provider;

        public MyKeyboardHandlerService(IServiceProvider provider, IKeyboardHandlerService service)
            : base(service) {
            this.provider = provider;

        }

        public override void OnKeyDown(KeyEventArgs e) {

            if (e.Control) {
                MyMouseHandlerService mouseHandler = (MyMouseHandlerService)provider.GetService(typeof(IMouseHandlerService));
                if (mouseHandler != null) {
                    mouseHandler.myZoomEnabled = !mouseHandler.myZoomEnabled;
                }
            }
            base.OnKeyDown(e);
        }
    }

}

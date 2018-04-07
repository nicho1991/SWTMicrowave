using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Interfaces;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using NUnit.Framework;
using NSubstitute;
using System.Threading;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{

    class IntergrationTest3
    {
        private ICookController cookcontroller;

        private IButton PowerButton;
        private IButton TimeButton;
        private IButton startCancel;
        private IUserInterface UI;
        private ILight Light;
        private IDisplay Display;
        private IPowerTube powerTube;
        private ITimer timer;
        private IOutput output;
        private IDoor Door;
        private static Mutex mut = new Mutex();
        [SetUp]
        public void setup()
        {
            Door = new Door();
            PowerButton = new Button();
            TimeButton = new Button();
            startCancel = new Button();
            timer = new Timer();
            output = Substitute.For<IOutput>();
            Display = new Display(output);
            powerTube = new PowerTube(output);
            Light = new Light(output);
            cookcontroller = new CookController(timer, Display, powerTube, UI);

            UI = new UserInterface(PowerButton, TimeButton, startCancel, Door, Display, Light, cookcontroller);

        }

        #region Timer

        [Test]
        public void Timer_CookControlToTimer_outputShown()
        {
       //     mut.WaitOne();
            cookcontroller.StartCooking(50,2000);
            Thread.Sleep(1000);
            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("1")));
      //      mut.ReleaseMutex();
        }

        [TestCase(5000, 4)]
        [TestCase(3001, 3)]
        [TestCase(3000, 2)]
        public void Timer_TimerTicks_outputReceivesTicks(int time, int ticks)
        {
            cookcontroller.StartCooking(80, time);
            Thread.Sleep(time + 3);
            output.Received(ticks + 1); //shows power aswell as time count downs
        }

        [Test]
        public void TimerDisplay_TimerDone_Display_clear()
        {
        //    mut.WaitOne();
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            Thread.Sleep(61000);

            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("clear")));

       //     mut.ReleaseMutex();
        }

        [Test]
        public void Timer_OpenDoor_TimerOff()
        {
           // mut.WaitOne();
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();

            Door.Open();
            Thread.Sleep(2000); //wait to see if timer goes down
            Assert.That(timer.TimeRemaining, Is.EqualTo(60));
           
         //   mut.ReleaseMutex();
        }

        [Test]
        public void Timer_cancel_TimerOff()
        {
         //   mut.WaitOne();
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();

            startCancel.Press();
            Thread.Sleep(2000); //wait to see if timer goes down
            Assert.That(timer.TimeRemaining, Is.EqualTo(60));

          //  mut.ReleaseMutex();
        }

        #endregion

        #region Display
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void Display_PowerPressed_Outputs(int n)
        {
            for (int i = 0; i < n; i++)
            {
                PowerButton.Press();
            }

            string power = (n * 50).ToString();
            output.Received(1).OutputLine(Arg.Is<string>(x => x.Contains(power)));

        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void Display_TimePressed_Outputs(int n)
        {
            PowerButton.Press();
            for (int i = 0; i < n; i++)
            {
                TimeButton.Press();
            }

            string time = (n).ToString();
            output.Received(1).OutputLine(Arg.Is<string>(x => x.Contains(time)));
        }

        //on timer tick show time
        [Test]
        public void Display_TimerTicks_outputsTime()
        {
            //mut.WaitOne();
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();

            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("01:00")));
            Thread.Sleep(1100); //wait to see if timer goes down
            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("59")));

            //mut.ReleaseMutex();
        }

        [Test]
        public void Display_OpensDoor_outputsClear()
        {
         //   mut.WaitOne();
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();

            Door.Open();
            Thread.Sleep(2000); //wait to see if timer goes down
            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("clear")));

         //   mut.ReleaseMutex();
        }
        //cancel display clear
        [Test]
        public void Display_Cancel_outputsClear()
        {
         //   mut.WaitOne();
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();

            startCancel.Press();
            Thread.Sleep(2000); //wait to see if timer goes down
            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("clear")));

          //  mut.ReleaseMutex();
        }

        #endregion

        #region PowerTube
        //PowerTube_Start_OutputsOn
        [Test]
        public void PowerTube_start_OutputsOn()
        {

            cookcontroller.StartCooking(50, 999);
            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("works")));
  
        }

        //PowerTube_TimerExpired_outputsOff
        [Test]
        public void PowerTube_TimerExpired_outputsOff()
        {
            //mut.WaitOne();
            cookcontroller.StartCooking(50, 1000);
            Thread.Sleep(1000);
            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("off")));
            //mut.ReleaseMutex();
        }

        [Test]
        public void PowerTube_OpensDoorCooking_outputsOff()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            Door.Open();

            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("off")));
        }
        [Test]
        public void PowerTube_cancelCooking_outputsOff()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            startCancel.Press();

            output.Received().OutputLine(Arg.Is<string>(x => x.Contains("off")));
        }

        [Test]
        public void PowerTube_AllreadyOnException()
        {
            powerTube.TurnOn(80);
            Assert.That(() => cookcontroller.StartCooking(80, 3000), Throws.TypeOf<ApplicationException>());
        }

        [TestCase(0, 3000)]
        [TestCase(101, 3000)]
        [TestCase(701, 3000)]
        public void PowerTube_StartCookingException(int power, int time)
        {
            Assert.That(() => cookcontroller.StartCooking(power, time), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        #endregion

    }
}

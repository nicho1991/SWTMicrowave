using System;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using NUnit.Framework;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IntegrationTest1
    {
        private IButton PowerButton;
        private IButton TimeButton;
        private IButton startCancel;

        private IDoor Door;

        private IDisplay Display;
        private ICookController CookControl;
        private ILight Light;

        private IUserInterface UserInterface;

        [SetUp]
        public void setup()
        {
            PowerButton = new Button();
            TimeButton = new Button();
            startCancel = new Button();

            Door = new Door();

            Display = Substitute.For<IDisplay>();
            CookControl = Substitute.For<ICookController>();
            Light = Substitute.For<ILight>();

            UserInterface = new UserInterface(PowerButton, TimeButton, startCancel, Door, Display, Light, CookControl);
        }

        #region Door
        [Test]
        public void UserInterface_Door_Open()
        {
            Door.Open();
            Light.Received().TurnOn();
        }

        [Test]
        public void UserInterface_Door_CloseOpenDoor()
        {
            Door.Open();
            Door.Close();
            Light.Received().TurnOff();
        }

        [Test]
        public void UserInterface_Door_CloseClosedDoor()
        {
            Door.Close();
            Light.DidNotReceive().TurnOff();
        }

        #endregion

        #region Buttons
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(200)]
        public void UserInterface_PowerButton(int n)
        {
            for (int i = 0; i < n; i++)
            {
                PowerButton.Press();
            }
            Display.Received(n / (700 / 50)).ShowPower(700); //n / (max power / power inc)
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(200)]
        public void UserInterface_TimeButton(int n)
        {
            PowerButton.Press(); //user has to press power button before timer can be set
            for (int i = 0; i < n; i++)
            {
                TimeButton.Press();
            }

            Display.Received(1).ShowTime(n, 0);
        }
        [Test]
        public void UserInterface_StartCancelButton_start()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            Light.Received().TurnOn();
        }
        [Test]
        public void UserInterface_StartCancelButton_Cancel()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            startCancel.Press();
            Light.Received().TurnOff();
            CookControl.Received().Stop();
            Display.Received(2).Clear();

        }
        [Test]
        public void UserInterface_StartCancelButton_start_cook()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            CookControl.Received().StartCooking(50, 60);
            Light.Received().TurnOn();
        }

        [Test]
        public void UserInterface_StartCancelButton_cancel_Nocook()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            startCancel.Press();
            CookControl.Received().Stop();
            Light.Received().TurnOff();

        }

        [Test]
        public void UserInterface_CookDone()
        {
            PowerButton.Press();
            TimeButton.Press();
            startCancel.Press();
            //after a while cook is done (stub)
            Display.Received(1).Clear();

        }
        #endregion

    }
}
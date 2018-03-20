using System;
using MicrowaveOvenClasses.Boundary;

using NUnit.Framework;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IntegrationTest1
    {
        //billede 2
        [Test]
        public void UserInterfaceButton()
        {
            IButton PowerButton = new Button();
            var UserInterface = Substitute.For<IUserInterface>();
            PowerButton.Pressed += new EventHandler(UserInterface.OnPowerPressed);

            PowerButton.Press();
            UserInterface.Received()

        }
    }
}
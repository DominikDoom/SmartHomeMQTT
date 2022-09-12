using SmartHomeMQTT.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartHomeMqttTests
{
    public class AddSensorTests
    {
        private AddSensorViewModel VM;

        // Runs before each test
        public AddSensorTests()
        {
            VM = new();
        }

        [Fact(DisplayName = "CanSave true when fields are filled")]
        public void TestCanSaveTrueIfFilled()
        {
            VM.Room = "testroom";
            VM.Name = "testname";
            Assert.True(VM.SaveCommand.CanExecute(null));
        }

        [Fact(DisplayName = "CanSave false when fields are empty")]
        public void TestCanSaveFalseIfEmpty()
        {
            Assert.False(VM.SaveCommand.CanExecute(null));
        }
    }
}

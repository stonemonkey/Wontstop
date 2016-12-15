using System;

namespace Common.Uwp.StateTriggers
{
    public class DeviceFamilyTrigger : StateTriggerValueBase
    {
        private string _currentDeviceFamily;

        private string _queriedDeviceFamily;

        public string DeviceFamily
        {
            get
            {
                return _queriedDeviceFamily;
            }
            set
            {
                _queriedDeviceFamily = value;
                _currentDeviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                IsActive = string.Equals(_queriedDeviceFamily, _currentDeviceFamily, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}

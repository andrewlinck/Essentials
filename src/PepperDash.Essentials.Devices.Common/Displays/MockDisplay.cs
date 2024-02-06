﻿using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Routing;

namespace PepperDash.Essentials.Devices.Common.Displays
{
    public class MockDisplay : TwoWayDisplayBase, IBasicVolumeWithFeedback, IBridgeAdvanced

	{
		public RoutingInputPort HdmiIn1 { get; private set; }
		public RoutingInputPort HdmiIn2 { get; private set; }
		public RoutingInputPort HdmiIn3 { get; private set; }
		public RoutingInputPort ComponentIn1 { get; private set; }
		public RoutingInputPort VgaIn1 { get; private set; }

		bool _PowerIsOn;
		bool _IsWarmingUp;
		bool _IsCoolingDown;

        protected override Func<bool> PowerIsOnFeedbackFunc
        {
            get
            {
                return () =>
                    {
                        Debug.Console(2, this, "*************************************************** Display Power is {0}", _PowerIsOn ? "on" : "off");
                        return _PowerIsOn;
                    };
        } }
		protected override Func<bool> IsCoolingDownFeedbackFunc
        {
            get
            {
                return () =>
                {
                    Debug.Console(2, this, "*************************************************** {0}", _IsCoolingDown ? "Display is cooling down" : "Display has finished cooling down");
                    return _IsCoolingDown;
                };
            }
        }
		protected override Func<bool> IsWarmingUpFeedbackFunc
        {
            get
            {
                return () =>
                {
                    Debug.Console(2, this, "*************************************************** {0}", _IsWarmingUp ? "Display is warming up" : "Display has finished warming up");
                    return _IsWarmingUp;
                };
            }
        }
        protected override Func<string> CurrentInputFeedbackFunc { get { return () => "Not Implemented"; } }

        int VolumeHeldRepeatInterval = 200;
        ushort VolumeInterval = 655;
		ushort _FakeVolumeLevel = 31768;
		bool _IsMuted;

		public MockDisplay(string key, string name)
			: base(key, name)
		{
			HdmiIn1 = new RoutingInputPort(RoutingPortNames.HdmiIn1, eRoutingSignalType.Audio | eRoutingSignalType.Video,
				eRoutingPortConnectionType.Hdmi, null, this);
			HdmiIn2 = new RoutingInputPort(RoutingPortNames.HdmiIn2, eRoutingSignalType.Audio | eRoutingSignalType.Video,
				eRoutingPortConnectionType.Hdmi, null, this);
			HdmiIn3 = new RoutingInputPort(RoutingPortNames.HdmiIn3, eRoutingSignalType.Audio | eRoutingSignalType.Video,
				eRoutingPortConnectionType.Hdmi, null, this);
			ComponentIn1 = new RoutingInputPort(RoutingPortNames.ComponentIn, eRoutingSignalType.Video,
				eRoutingPortConnectionType.Component, null, this);
			VgaIn1 = new RoutingInputPort(RoutingPortNames.VgaIn, eRoutingSignalType.Video,
				eRoutingPortConnectionType.Composite, null, this);
			InputPorts.AddRange(new[] { HdmiIn1, HdmiIn2, HdmiIn3, ComponentIn1, VgaIn1 });

			VolumeLevelFeedback = new IntFeedback(() => { return _FakeVolumeLevel; });
			MuteFeedback = new BoolFeedback("MuteOn", () => _IsMuted);

            WarmupTime = 10000;
            CooldownTime = 10000;
		}

		public override void PowerOn()
		{
			if (!PowerIsOnFeedback.BoolValue && !_IsWarmingUp && !_IsCoolingDown)
			{
				_IsWarmingUp = true;
				IsWarmingUpFeedback.InvokeFireUpdate();
				// Fake power-up cycle
				WarmupTimer = new CTimer(o =>
					{
						_IsWarmingUp = false;
						_PowerIsOn = true;
						IsWarmingUpFeedback.InvokeFireUpdate();
						PowerIsOnFeedback.InvokeFireUpdate();
					}, WarmupTime);
			}
		}

		public override void PowerOff()
		{
			// If a display has unreliable-power off feedback, just override this and
			// remove this check.
			if (PowerIsOnFeedback.BoolValue && !_IsWarmingUp && !_IsCoolingDown)
			{
				_IsCoolingDown = true;
				IsCoolingDownFeedback.InvokeFireUpdate();
				// Fake cool-down cycle
				CooldownTimer = new CTimer(o =>
					{
						Debug.Console(2, this, "Cooldown timer ending");
						_IsCoolingDown = false;
						IsCoolingDownFeedback.InvokeFireUpdate();
                        _PowerIsOn = false;
                        PowerIsOnFeedback.InvokeFireUpdate();
					}, CooldownTime);
			}
		}		
		
		public override void PowerToggle()
		{
			if (PowerIsOnFeedback.BoolValue && !IsWarmingUpFeedback.BoolValue)
				PowerOff();
			else if (!PowerIsOnFeedback.BoolValue && !IsCoolingDownFeedback.BoolValue)
				PowerOn();
		}

		public override void ExecuteSwitch(object selector)
		{
			Debug.Console(2, this, "ExecuteSwitch: {0}", selector);

		    if (!_PowerIsOn)
		    {
		        PowerOn();
		    }
		}



		#region IBasicVolumeWithFeedback Members

		public IntFeedback VolumeLevelFeedback { get; private set; }

		public void SetVolume(ushort level)
		{
			_FakeVolumeLevel = level;
			VolumeLevelFeedback.InvokeFireUpdate();
		}

		public void MuteOn()
		{
			_IsMuted = true;
			MuteFeedback.InvokeFireUpdate();
		}

		public void MuteOff()
		{
			_IsMuted = false;
			MuteFeedback.InvokeFireUpdate();
		}

		public BoolFeedback MuteFeedback { get; private set; }

		#endregion

		#region IBasicVolumeControls Members

		public void VolumeUp(bool pressRelease)
		{
            //while (pressRelease)
            //{
                Debug.Console(2, this, "Volume Down {0}", pressRelease);
                if (pressRelease)
                {
                    var newLevel = _FakeVolumeLevel + VolumeInterval;
                    SetVolume((ushort)newLevel);
                    CrestronEnvironment.Sleep(VolumeHeldRepeatInterval);
                }
            //}
		}

		public void VolumeDown(bool pressRelease)
		{
            //while (pressRelease)
            //{
                Debug.Console(2, this, "Volume Up {0}", pressRelease);
                if (pressRelease)
                {
                    var newLevel = _FakeVolumeLevel - VolumeInterval;
                    SetVolume((ushort)newLevel);
                    CrestronEnvironment.Sleep(VolumeHeldRepeatInterval);
                }
            //}
		}

		public void MuteToggle()
		{
			_IsMuted = !_IsMuted;
			MuteFeedback.InvokeFireUpdate();
		}

		#endregion

	    public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
	    {
	        LinkDisplayToApi(this, trilist, joinStart, joinMapKey, bridge);
	    }
	}
}
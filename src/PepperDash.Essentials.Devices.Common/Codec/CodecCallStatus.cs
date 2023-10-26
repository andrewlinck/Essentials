﻿namespace PepperDash.Essentials.Devices.Common.Codec
{
    public class CodecCallStatus
    {

        /// <summary>
        /// Takes the Cisco call type and converts to the matching enum
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static eCodecCallStatus ConvertToStatusEnum(string s)
        {
            switch (s)
            {
                case "Connected":
                {
                    return eCodecCallStatus.Connected;
                }
                case "Connecting":
                {
                    return eCodecCallStatus.Connecting;
                }
                case "Dialling":
                {
                    return eCodecCallStatus.Dialing;
                }
                case "Disconnected":
                {
                    return eCodecCallStatus.Disconnected;
                }
                case "Disconnecting":
                {
                    return eCodecCallStatus.Disconnecting;
                }
                case "EarlyMedia":
                {
                    return eCodecCallStatus.EarlyMedia;
                }
                case "Idle":
                {
                    return eCodecCallStatus.Idle;
                }
                case "OnHold":
                {
                    return eCodecCallStatus.OnHold;
                }
                case "Ringing":
                {
                    return eCodecCallStatus.Ringing;
                }
                case "Preserved":
                {
                    return eCodecCallStatus.Preserved;
                }
                case "RemotePreserved":
                {
                    return eCodecCallStatus.RemotePreserved;
                }
                default:
                    return eCodecCallStatus.Unknown;
            }

        }

    }
}
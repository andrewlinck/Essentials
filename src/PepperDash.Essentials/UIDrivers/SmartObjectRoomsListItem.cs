﻿using System;
using PepperDash.Essentials.Core;

namespace PepperDash.Essentials
{
    public class SmartObjectRoomsListItem
    {
        public IEssentialsHuddleSpaceRoom Room { get; private set; }
        SmartObjectRoomsList Parent;
        public uint Index { get; private set; }

        public SmartObjectRoomsListItem(IEssentialsHuddleSpaceRoom room, uint index, SmartObjectRoomsList parent, 
            Action<bool> buttonAction)
        {
            Room   = room;
            Parent = parent;
            Index  = index;
            if (room == null) return;

            // Set "now" states
            parent.SetItemMainText(index, room.Name);
            UpdateItem(room.CurrentSourceInfo);
            // Watch for later changes
            room.CurrentSourceChange += new SourceInfoChangeHandler(room_CurrentSourceInfoChange);
            parent.SetItemButtonAction(index, buttonAction);
        }

        void room_CurrentSourceInfoChange(SourceListItem info, ChangeType type)
        {
            UpdateItem(info);
        }

        /// <summary>
        /// Helper to handle source events and startup syncing with room's current source
        /// </summary>
        /// <param name="info"></param>
        void UpdateItem(SourceListItem info)
        {
            if (info == null || info.Type == eSourceListItemType.Off)
            {
                Parent.SetItemStatusText(Index, "");
                Parent.SetItemIcon(Index, "Blank");
            }
            else
            {
                Parent.SetItemStatusText(Index, info.PreferredName);
                Parent.SetItemIcon(Index, info.AltIcon);
            }
        }
    }
}